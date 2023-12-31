﻿using Dungeon.Scenes.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Dungeon12.Abilities
{
    public class Cooldown : ICooldownChain
    {
        private static readonly Dictionary<string, Cooldown> cooldowns = new Dictionary<string, Cooldown>();

        public Cooldown(double milliseconds, string name = null)
        {
            Milliseconds = milliseconds;
            Name = name ?? Guid.NewGuid().ToString();

            if (!cooldowns.ContainsKey(name))
            {
                this.Timer = new System.Timers.Timer(milliseconds);
                this.Timer.AutoReset = false;
                this.Timer.Elapsed += (x, y) => Done(name);

                cooldowns.Add(name, this);
            }
        }

        public void Reset() => Done(this.Name);

        public static void ResetAll()
        {
            cooldowns.Clear();
        }

        /// <summary>
        /// Сбросить куллдаун до нуля
        /// </summary>
        /// <param name="name"></param>
        public static void Done(string name)
        {
            cooldowns[name].Watch.Reset();
            cooldowns[name].Available = true;
            cooldowns[name].IsActive = false;
        }

        public static Cooldown Make(double milliseconds, string name = null) => new Cooldown(milliseconds, name);

        private Cooldown Next = null;
        private Cooldown Parent = null;

        private Stopwatch Watch { get; set; } = new Stopwatch();

        public string Name { get; }

        public double Milliseconds { get; }

        public float Percent => GetPercent(this);

        public double ElapsedSeconds
        {
            get
            {
                var plus = 0d;
                if (Next != null)
                {
                    plus = Next.ElapsedSeconds;
                }

                return plus+(Milliseconds - cooldowns[Name].Watch.ElapsedMilliseconds) / 1000;
            }
        }

        private float GetPercent(Cooldown cooldown)
        {
            if (cooldown.Next != null)
            {
                return GetPercent(cooldown.Next);
            }

            var name = cooldown.Name;
            return cooldowns[name].Watch.ElapsedMilliseconds / ((float)cooldowns[name].Milliseconds) * 100f;
        }

        private bool isActive = false;
        public bool IsActive
        {
            get => isActive || (this.Next?.IsActive ?? false);
            set => isActive = value;
        }

        internal System.Timers.Timer Timer { get; set; }

        private bool available = true;
        internal bool Available
        {
            get => available && (this.Next?.Available ?? true);
            set => available = value;
        }

        /// <summary>
        /// Проверка что нету кулдауна у способности
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            bool cooldownResult = true;

            if (cooldowns.TryGetValue(Name, out var cd))
            {
                cooldownResult = cd.Available;
            }

            if (Next != default)
            {
                return cooldownResult && Next.Check();
            }

            return cooldownResult;
        }

        /// <summary>
        /// Указывает что способность используется что бы нельзя было её вызвать
        /// </summary>
        public void Cast()
        {
            cooldowns[Name].IsActive = true;
            cooldowns[Name].Watch.Start();
            cooldowns[Name].Available = false;
            cooldowns[Name].Timer.Start();
            StartChain();
        }

        private void StartChain()
        {
            if (Next != default)
            {
                Next.Cast();
            }
        }

        public ICooldownChain Chain(double milliseconds, string name = null)
        {
            Next = new Cooldown(milliseconds, name);
            Next.Parent = this;
            return Next;
        }

        public Cooldown Build()
        {
            if (this.Parent != default)
            {
                return this.Parent.Build();
            }

            return this;
        }
    }

    public interface ICooldownChain
    {
        ICooldownChain Chain(double milliseconds, string name = null);

        Cooldown Build();
    }
}