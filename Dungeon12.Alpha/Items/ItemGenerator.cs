﻿using Dungeon12.Classes;
using Dungeon.Drawing;
using Dungeon12.Items.Enums;
using Dungeon12.Items.Types;
using Dungeon12.Loot;
using Dungeon.Types;
using Force.DeepCloner;
using System;
using System.Collections.Generic;
using System.Text;
using Dungeon;
using Dungeon.Resources;
using System.Linq;

namespace Dungeon12.Items
{
    public class ItemGenerator : LootGenerator
    {
        public override Item Generate()
        {
            var additionalEquipments = Global.GameState.Equipment.AdditionalEquipments;

            var generationOpts = GenerateType();
            if (generationOpts == default)
                return default;

            var rarityopts = GenerateRarity();

            var item = generationOpts.ItemType.NewAs<Item>();
            item.Rare = rarityopts.Rarity;

            var available = generationOpts.AvailableStats.Where(x => rarityopts.AvailableStats.Contains(x));

            var statopts = available
                .Select(x =>
                {
                    var attr = x.ToValue<GenerationAttribute>();
                    attr.Stat = x;
                    return attr;
                });

            var stats = new List<Equipment>();

            var lvl = Global.GameState.Character.Level;

            long CalculateStat(int generationMultiplerStat, int generationMultiplerRare)
            {
                return (generationMultiplerStat * (lvl/2)) + (generationMultiplerRare * lvl);
            }

            foreach (var statopt in statopts)
            {
                if (Dungeon.Random.Chance(statopt.Chance))
                {
                    switch (statopt.Stat)
                    {
                        case Stats.Health:
                            stats.Add(new BaseStatEquip()
                            {
                                StatName = "Здоровье",
                                StatProperties = new List<string>() { "MaxHitPoints" },
                                StatValues = new List<long>() { CalculateStat(statopt.GenerationMultipler, rarityopts.GenerationMultipler) },
                                Color = statopt.Stat.Color()
                            });
                            break;
                        case Stats.Resource:
                            break;
                        case Stats.AbilityPower:
                        case Stats.AttackDamage:
                        case Stats.Defence:
                        case Stats.Barrier:
                            stats.Add(new BaseStatEquip()
                            {
                                StatName = statopt.Stat.ToDisplay(),
                                StatProperties = new List<string>() { statopt.Stat.ToString() },
                                StatValues = new List<long>() { CalculateStat(statopt.GenerationMultipler, rarityopts.GenerationMultipler) },
                                Color = statopt.Stat.Color()
                            });
                            break;
                        case Stats.Class:
                            var statEqip = additionalEquipments[Dungeon.Random.Range(0, Global.GameState.Equipment.AdditionalEquipments.Count - 1)].DeepClone();
                            var values = statEqip.StatValues.Values;
                            statEqip.StatProperties.ForEach((x, i) =>
                            {
                                values.Add(CalculateStat(statopt.GenerationMultipler, rarityopts.GenerationMultipler));
                            });
                            stats.Add(statEqip);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (stats.Count == 0)
            {
                var hp = Stats.Health;
                stats.Add(new BaseStatEquip()
                {
                    StatName = "Здоровье",
                    StatProperties = new List<string>() { "MaxHitPoints" },
                    StatValues = new List<long>() { CalculateStat(1, rarityopts.GenerationMultipler) },
                    Color = hp.Color()
                });
            }

            item.BaseStats = stats;
            item.Name = $"{item.Rare.ToDisplay()} {item.Kind.ToDisplay()}";
            item.Tileset = $"Items/{item.Kind.ToString()}s/{Dungeon.Random.Range(1, 3)}.gif".AsmImg();

            return item;
        }

        private GenerationAttribute GenerateRarity()
        {
            var generations = GetGeneratorsFromRarity();
            foreach (var generator in generations.OrderBy(g => g.Chance))
            {
                if (Dungeon.Random.Chance(generator.Chance))
                {
                    return generator;
                }
            }

            return new GenerationAttribute(1, 1, Stats.Health);
        }

        /// <summary>
        /// 
        /// <para>
        /// [Кэшируемый]
        /// </para>
        /// </summary>
        public List<GenerationAttribute> GetGeneratorsFromRarity()
        {
            if (___GetGeneratorsFromRarytyCache.Count == 0)
            {

                ___GetGeneratorsFromRarytyCache = typeof(Rarity).All<Rarity>()
                    .Select(x =>
                    {
                        var attr = x.ToValue<GenerationAttribute>();
                        if (attr != default)
                        {
                            attr.Rarity = x;
                        }
                        return attr;
                    })
                    .Where(x => x != default)
                    .ToList();
            }

            return ___GetGeneratorsFromRarytyCache
                .Where(x => Global.GameState.Character.Level >= x.MinimumLevel)
                .ToList();
        }

        private static List<GenerationAttribute> ___GetGeneratorsFromRarytyCache = new List<GenerationAttribute>();

        private GenerationAttribute GenerateType() => GetGeneratorsFromItems().Random();

        /// <summary>
        /// 
        /// <para>
        /// [Кэшируемый]
        /// </para>
        /// </summary>
        public List<GenerationAttribute> GetGeneratorsFromItems()
        {
            if (___GetGeneratorsFromItemsCache.Count == 0)
            {
                ___GetGeneratorsFromItemsCache = typeof(Item).AllAssignedFrom()
                    .Select(x =>
                    {
                        var attr = (GenerationAttribute)Attribute.GetCustomAttribute(x, typeof(GenerationAttribute), true);
                        if (attr != default)
                        {
                            attr.ItemType = x;
                        }
                        return attr;
                    })
                    .Where(x => x != default)
                    .ToList();
            }

            return ___GetGeneratorsFromItemsCache
                .Where(x => Global.GameState.Character.Level >= x.MinimumLevel)
                .ToList();
        }

        private static List<GenerationAttribute> ___GetGeneratorsFromItemsCache = new List<GenerationAttribute>();

    }
}