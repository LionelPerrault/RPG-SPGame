﻿namespace Dungeon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        /// <summary>
        /// Создать новый объект такого типа
        /// <para>
        /// Только вот не надо использовать это на типах <see cref="Type"/>
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public static T New<T>(this object @object) => NewAs<T>(@object.GetType());

        /// <summary>
        /// Instantiate new object through expression tree with first ctor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static T New<T>(this Type type, params object[] argsObj)
            => New<T>(type, typeof(T).GetConstructors().FirstOrDefault(), argsObj);

        /// <summary> 
        /// Instantiate new object through expression tree with first ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static object New(this Type type, params object[] argsObj)
            => New<object>(type, type.GetConstructors().FirstOrDefault(), argsObj);

        public static object New(this Type type, bool onlyParameterLess, params object[] argsObj)
        {
            var ctors = type.GetConstructors();
            ConstructorInfo ctor = default;
            if (onlyParameterLess)
            {
                ctor = ctors.FirstOrDefault(c => c.GetParameters().Length == 0);
            }
            else
            {
                ctor = ctors.FirstOrDefault();
            }

            return New<object>(type, ctor, argsObj);
        }

        /// <summary>
        /// Instantiate new object through expression tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="ctor"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static T New<T>(this Type type, ConstructorInfo ctor, params object[] argsObj)
        {
            if (ctor == default)
                return default;

            ParameterInfo[] par = ctor.GetParameters();
            Expression[] args = new Expression[par.Length];
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), par[i].ParameterType);
            }
            var expression = Expression.Lambda<Func<object[], T>>(
                Expression.New(ctor, args), param
            );

            var func = expression.Compile();

            return func(argsObj);
        }
        
        /// <summary>
         /// Instantiate new object through expression tree
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="type"></param>
         /// <param name="ctor"></param>
         /// <param name="argsObj"></param>
         /// <returns></returns>
        public static T NewAs<T>(this Type type, int ctorCount, params object[] argsObj)
        {
            ConstructorInfo ctor = type.GetConstructors().ElementAtOrDefault(ctorCount-1);

            if (ctor == default)
                return default;

            ParameterInfo[] par = ctor.GetParameters();
            Expression[] args = new Expression[par.Length];
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), par[i].ParameterType);
            }
            var expression = Expression.Lambda<Func<object[], T>>(
                Expression.New(ctor, args), param
            );

            var func = expression.Compile();

            return func.Invoke(argsObj).As<T>();
        }

        /// <summary>
        /// Инстанциирует объект как object, а затем приводит к T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static T NewAs<T>(this Type type, params object[] argsObj)
            => (T)New<object>(type, type.GetConstructors().FirstOrDefault(), argsObj);

        public static object Call(this object @object, string method, params object[] argsObj)
        {
            var methodInfo = @object.GetType().GetMethods().FirstOrDefault(m => m.Name == method);
            if (methodInfo != default)
            {
                if (methodInfo.ReturnType != typeof(void))
                {
                    return Expression.Lambda(Expression.Call(Expression.Constant(@object), methodInfo)).Compile().DynamicInvoke(argsObj);
                }
                else
                {
                    Expression.Lambda(Expression.Call(Expression.Constant(@object), methodInfo)).Compile().DynamicInvoke(argsObj);
                }
            }

            return default;
        }

        public static object CallGeneric(this object @object, string method, Type[] generics, params object[] argsObj)
        {
            var methodInfo = @object.GetType().GetMethods().LastOrDefault(m => m.Name == method && m.IsGenericMethod);
            if (methodInfo != default)
            {
                methodInfo = methodInfo.MakeGenericMethod(generics);

                return Expression.Lambda(Expression.Call(Expression.Constant(@object), methodInfo)).Compile().DynamicInvoke(argsObj);
            }

            return default;
        }

        public static TResult Call<TResult>(this object @object, string method, TResult @default, params object[] argsObj)
        {
            var methodInfo = @object.GetType().GetMethods().FirstOrDefault(m => m.Name == method);
            if (methodInfo == default)
            {
                methodInfo = @object.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == method);
            }
            if (methodInfo != default)
            {
                var from = Expression.Constant(@object);
                var @params = argsObj.Select(a => Expression.Parameter(a.GetType())).ToArray();
                var methodCall = Expression.Call(from, methodInfo, @params);
                return Expression.Lambda(methodCall,@params).Compile().DynamicInvoke(argsObj).As<TResult>();
            }

            return @default;
        }

        public static TResult Call<TResult,TFrom>(this object @object, string method, TResult @default, params object[] argsObj)
        {
            var methodInfo = typeof(TFrom).GetMethods().FirstOrDefault(m => m.Name == method);
            if(methodInfo==default)
            {
                methodInfo = typeof(TFrom).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == method);
            }
            if (methodInfo != default)
            {
                var from = Expression.Constant(@object);
                var @params = argsObj.Select(a => Expression.Parameter(a.GetType()));
                var methodCall = Expression.Call(from, methodInfo, @params);
                return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(argsObj).As<TResult>();
            }

            return @default;
        }
    }
}