﻿namespace Rogue
{
    using FastMember;
    using System.Linq.Expressions;
    using System;
    using System.Linq;
    using Dungeon.Types;
    using System.Reflection;
    using System.Collections.Generic;

    public static class PropertyAccessor
    {
        public static T As<T>(this object obj)
        {
            if (obj is T tObj)
            {
                return tObj;
            }

            if(obj is null)
            {
                throw new System.ArgumentNullException("Property is null!");
            }

            throw new System.Exception("Property had wrong type!");
        }

        public static void Dispatch<T>(this T obj, Expression<Action<T, object>> method, object arg)
        {
            var name = (method.Body as MethodCallExpression).Method.Name.Replace("Call", "");

            if (DispatchExists(obj.GetType(), name, arg.GetType()))
            {
                method.Compile().Invoke((T)obj, arg);
            }
        }

        public static TResult Dispatch<T, TResult, TArg>(this T obj, Expression<Func<T, TArg, TResult>> method, TArg arg)
        {
            string name = ExtractMethodName(method);

            if (DispatchExists<T>(name, typeof(TArg)))
            {
                return method.Compile().Invoke(obj, arg);
            }

            return default;
        }

        private static string ExtractMethodName(LambdaExpression method)
        {
            return (method.Body as MethodCallExpression).Method.Name;
        }

        public static void Flow<T>(this T obj, Expression<Action<T>> method, object args=null, bool up = true)
            where T : IFlowable
        {
            var caller = up ? UpperFlowable(obj) : obj;

            var methodName = ExtractMethodName(method);
            object ctx = CreateContextObject(caller, methodName);

            if (args != null)
            {
                MergeObjects(ctx, args);
            }

            var firstFlow = DoFlow(ctx, methodName, caller);
            var flow = new List<Action<bool>>()
            {
                firstFlow
            };

            MakeFlow(caller, ctx, methodName, flow);

            flow.Reverse();

            foreach (var flowEntry in flow)
            {
                flowEntry?.Invoke(false);
            }
        }

        private static object CreateContextObject(IFlowable caller, string methodName)
        {
            try
            {
                return (Attribute.GetCustomAttribute(
                        GetFlowMethodInfo(caller, methodName),
                        typeof(FlowMethodAttribute)) as FlowMethodAttribute)
                        .ContextType.New();
            }
            catch (Exception inner)
            {
                throw new Exception("Ошибка при выполнении flow метода - возможно не указан Parent для вызываемого объекта?", inner);
            }
        }

        private static IFlowable UpperFlowable(IFlowable flowable)
        {
            var parent = flowable.GetParentFlow();
            if (parent != null)
            {
                return UpperFlowable(parent);
            }
            return flowable;
        }

        private static void MakeFlow(object obj, object ctx, string methodName, List<Action<bool>> flow)
        {
            if (obj == default)
                return;

            var flowable = FindFlowable(obj);
            foreach (var innerFlow in flowable)
            {
                flow.Add(DoFlow(ctx, methodName, innerFlow));
            }

            foreach (var innerFlow in flowable)
            {
                MakeFlow(innerFlow, ctx, methodName, flow);
            }
        }

        private static Action<bool> DoFlow(object ctx, string methodName, IFlowable innerFlow, bool forward = true)
        {
            innerFlow.SetFlowContext(ctx);
            var flowMethod = CallFlowable(innerFlow, methodName, ctx, forward);
            MergeObjects(ctx, innerFlow.GetFlowContext());

            return flowMethod;
        }

        private static IFlowable[] FindFlowable(object obj)
        {
            var accessor = TypeAccessor.Create(obj.GetType());
            return accessor
                .GetMembers()
                .Where(m => typeof(IFlowable).IsAssignableFrom(m.Type))
                .Select(x => 
                {
                    try
                    {
                        var value = accessor[obj, x.Name];
                        if (value != obj)
                        {
                            return value;
                        }
                        return default;
                    }
                    catch
                    {
                        // потому что fastmember валится когда хочешь Item свойство обработать
                        return default;
                    }
                })
                .Where(x => x != default)
                .Cast<IFlowable>()
                .ToArray();
        }

        private static Action<bool> CallFlowable(object next, string method, object args, bool forward)
        {
            MethodInfo methodInfo = GetFlowMethodInfo(next, method);
            if (methodInfo != null)
            {
                var param = Expression.Parameter(typeof(bool));

                var call = Expression.Call(Expression.Constant(next), methodInfo, param);
                var lambd = Expression.Lambda<Action<bool>>(call, param);

                var flowMethod = lambd.Compile();
                flowMethod.Invoke(forward);
                return flowMethod;
            }

            return default;
        }

        private static MethodInfo GetFlowMethodInfo(object next, string method)
        {
            return next.GetType().GetMethods().FirstOrDefault(x => x.Name == method);
        }

        private static object MergeObjects(object to, object from)
        {
            var accessorTo = TypeAccessor.Create(to.GetType());
            var accessorFrom = TypeAccessor.Create(from.GetType());

            foreach (var prop in accessorTo.GetMembers())
            {
                var propertyForSet = accessorFrom.GetMembers().FirstOrDefault(x => x.Name == prop.Name);
                if (propertyForSet == null)
                    continue;

                try
                {
                    accessorTo[to, prop.Name] = accessorFrom[from, prop.Name];
                }
                catch
                {
                    Console.WriteLine("Попытка установить свойство с неподходящим типом");
                }
            }

            return to;
        }

        private static bool DispatchExists<TType>(string methodName, Type argType)=> DispatchExists(typeof(TType), methodName, argType);

        private static bool DispatchExists(Type type, string methodName, Type argType)
        {
            return type.GetMethods().Any(m => m.Name == methodName && m.GetParameters()[0].ParameterType == argType);
        }

        public static object GetProperty(this object @object, string property)
        {
            var accessor = TypeAccessor.Create(@object.GetType(), true);
            return accessor[@object, property];
        }

        public static object GetStaticProperty(this Type type,string property)
        {
            var accessor = TypeAccessor.Create(type, true);
            return accessor[null, property];
        }

        public static TValue GetProperty<TValue>(this object @object, string property, TValue @default=default)
        {
            var accessor = TypeAccessor.Create(@object.GetType(), true);
            try
            {
                return (TValue)accessor[@object, property];
            }
            catch
            {
                return @default;
            }
        }

        public static TObject SetProperty<TObject, TValue>(this TObject @object, string property, TValue value)
        {
            var accessor = TypeAccessor.Create(@object.GetType(), true);
            accessor[@object, property] = value;
            return @object;
        }

        public static void SetProperty<TValue>(this object @object, string property, TValue value)
        {
            var accessor = TypeAccessor.Create(@object.GetType(), true);
            accessor[@object, property] = value;
        }
    }
}