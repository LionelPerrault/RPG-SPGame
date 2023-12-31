﻿namespace Dungeon12.Items
{
    using System.Collections.Generic;
    using Dungeon.Data;
    using Dungeon.Entities;
    using Dungeon12.Entities.Alive;
    using Dungeon12.Entities.Alive.Proxies;
    using Dungeon.GameObjects;
    using Dungeon12.Items.Enums;
    using Dungeon12.Loot;
    using Dungeon.Transactions;
    using Dungeon.Types;
    using Dungeon.View.Interfaces;
    using LiteDB;
    using Dungeon;
    using System;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// вещи могут быть сетами -не забыть
    /// //Вес = (УровеньПредмета — КачествоПредмета) * МультипликаторКачества * МультипликаторВидаПредмета;
    /// формирование цен бладжад
    /// </summary>
    public partial class Item : Drawable, IPersist, ILootable
    {
        public virtual bool Stackable { get; set; }

        public bool StackFull => Quantity == QuantityMax;

        /// <summary>
        /// [Лимит]
        /// </summary>
        [Dungeon.Proxied(typeof(Limit))]
        public virtual int Quantity { get; set; } = 1;

        public virtual int QuantityMax { get; set; } = 20;

        public virtual int QuantityRemove(int quantity)
        {
            var overflow = Quantity - quantity;
            if(overflow<0)
            {
                Quantity = 0;
                return overflow;
            }
            else if (overflow==0)
            {
                return 0;
            }

            Quantity -= quantity;
            return 1;
        }

        public virtual int QuantityAdd(int quantity)
        {
            var max = (Quantity + quantity) - QuantityMax;
            if (max > 0)
            {
                Quantity = QuantityMax;
                return max;
            }
            Quantity += quantity;
            return 0;
        }

        public string Description { get; set; }

        public List<Applicable> Modifiers { get; set; } = new List<Applicable>();

        /// <summary>
        /// Уровень вещи
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Редкость вещи
        /// </summary>
        public virtual Rarity Rare { get; set; }

        /// <summary>
        /// очередное дохуя спорное решение во имя запускаемости
        /// </summary>
        public static Item Empty => new EmptyItem();

        public virtual ItemKind Kind { get; set; }

        [BsonIgnore]
        public int Cost
        {
            get
            {
                var lvl = Global.GameState.Character.Level;
                var baseMultipler = (int)this.Rare * 1.25 + (int)this.Kind * 2.37 + (Dungeon.Random.Range(lvl, lvl + 10) * 1.89);
                return (int)Math.Round(this.BaseStats.Sum(s => baseMultipler));
            }
        }

        public Point InventoryPosition { get; set; }

        public virtual Point InventorySize => new Point(1, 1);

        public string LootTableName { get; set; }

        [BsonIgnore]
        public LootTable LootTable => LootTable.GetLootTable(this.LootTableName ?? this.IdentifyName);

        public virtual void Use() { }

        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string IdentifyName { get; set; }
        public string Assembly { get; set; }

        private class EmptyItem : Item
        {
        }
    }
}