﻿namespace Dungeon12.Classes
{
    using Dungeon;
    using Dungeon.Data;
    using Dungeon.Types;
    using Dungeon.View.Interfaces;
    using Dungeon12.Conversations;
    using Dungeon12.Entities.Alive;
    using Dungeon12.Entities.Alive.Enums;
    using Dungeon12.Entities.Quests;
    using Dungeon12.Events.Events;
    using Dungeon12.Inventory;
    using Dungeon12.Scenes.Menus;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Абстрактный класс персонажа
    /// </summary>
    public abstract partial class Character : Moveable, IPersist
    {
        public Character()
        {
            this.BindClassStats();
            Reload();

            Global.Events.Subscribe<GameLoadedEvent>(ReloadQuestsData);
        }

        public Character(bool @new)
        {
            this.HitPoints = this.MaxHitPoints = this.InitialHP;
            this.BindClassStats();
            Reload();

            Global.Events.Subscribe<GameLoadedEvent>(ReloadQuestsData);
        }

        private void ReloadQuestsData(GameLoadedEvent @event)
        {
            this.Journal.Quests.ForEach(q =>
            {
                if(q.Quest is CollectQuest quest)
                {
                    quest.ReloadQuestLootDrops();
                }
            });

            Global.GameState.RestorableRespawns.ForEach(x =>
            {
                PassRespawnTrigger.PassRespawn(x);
            });
        }

        public void Reload()
        {
            this.Clothes.OnPutOn = PutOnItem;
            this.Clothes.OnPutOff = PutOffItem;

            this.OnDie += () =>
            {
                DungeonGlobal.SceneManager.Switch<End>();
            };
        }

        public virtual void Destroy()
        {
            this.Clothes.OnPutOn = default;
            this.Clothes.OnPutOff = default;
        }

        public override bool ExpGainer => true;

        public Race Race { get; set; }

        public Origins Origin { get; set; }

        public long Gold { get; set; } = 100;

        public virtual string ClassName { get; }

        public virtual string Resource => "";

        public virtual string ResourceName => "Мана";

        public virtual void AddToResource(double value) { }

        public virtual void RemoveToResource(double value) { }

        public virtual IDrawColor ClassColor { get; }

        public virtual void AddClassPerk() { }

        public virtual string Avatar => this.GetType().Name.AsmImg();

        /// <summary>
        /// это пиздец, выпили это нахуй
        /// <para>
        /// А вот и нет, полезная фича оказалась
        /// </para>
        /// </summary>
        /// <returns></returns>
        public virtual ConsoleColor ResourceColor => ConsoleColor.Blue;

        public List<ClassStat> ClassStats { get; } = new List<ClassStat>();

        public virtual IDrawText MainAbilityDamageView { get; set; }

        public virtual string MainAbilityDamageText { get; set; }

        public Backpack Backpack { get; set; } = new Backpack(6, 11);

        public Wear Clothes { get; set; } = new Wear();

        /// <summary>
        /// Пересчитывает все характеристики
        /// </summary>
        public void Recalculate()
        {
            FreeProxyProperties();
            this.RecalculateLevelHP();
        }

        public List<Pair<string, object>> Variables = new List<Pair<string, object>>();

        public T GetVariable<T>(string name) => Variables.FirstOrDefault(n => n.First == name).Second.As<T>();

        public T SetVariable<T>(string name, T value)
        {
            var v = Variables.FirstOrDefault(n => n.First == name);
            if (v.First != default)
            {
                v.Second = value;
            }
            else
            {
                Variables.Add(new Pair<string, object>()
                {
                    First = name,
                    Second = value
                });
            }
            return value;
        }

        public object this[string variable]
        {
            get => GetVariable<object>(variable);
            set => SetVariable(variable, variable);
        }
    }
}