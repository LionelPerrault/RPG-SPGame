﻿using Dungeon.SceneObjects.Grouping;
using Dungeon.View;
using Dungeon12.Entities.Abilities;
using Dungeon12.Entities.Enums;
using Dungeon12.Entities.Perks;
using Dungeon12.Entities.Turning;
using System;
using System.Collections.Generic;

namespace Dungeon12.Entities
{
    internal class Hero : Battler
    {
        public ObjectGroupProperty IsActive { get; set; } = new ObjectGroupProperty();

        public int FreePoints { get; set; } = 0;

        public int Strength { get; set; } = 10;

        public int Agility { get; set; } = 10;

        public int Intellegence { get; set; } = 10;

        public int Stamina { get; set; } = 10;

        public bool CanFight { get; set; } = true;

        public Classes Class { get; set; }

        private Archetype _class;
        public Archetype Archetype
        {
            get
            {
                return _class;
            }
            set
            {
                ClassChange?.Invoke(_class, value);
                _class = value;
            }
        }

        public void BindSkills()
        {
            switch (Archetype)
            {
                case Archetype.Warrior:
                    Skills = new Skill[] { Skill.Landscape, Skill.Eating, Skill.Repair, Skill.Smithing };
                    break;
                case Archetype.Mage:
                    Skills = new Skill[] { Skill.Portals, Skill.Attension, Skill.Enchantment, Skill.Alchemy };
                    break;
                case Archetype.Thief:
                    Skills = new Skill[] { Skill.Traps, Skill.Lockpicking, Skill.Stealing, Skill.Leatherwork };
                    break;
                case Archetype.Priest:
                    Skills = new Skill[] { Skill.Prayers, Skill.FoodStoring, Skill.Trade, Skill.Tailoring };
                    break;
                default:
                    break;
            }
        }

        public Skill[] Skills { get; set; }

        public int SkillValue(Skill skill) => skill switch
        {
            Skill.Landscape => Landscape,
            Skill.Eating => Eating,
            Skill.Repair => Repair,
            Skill.Smithing => Smithing,
            Skill.Portals => Portals,
            Skill.Attension => Attension,
            Skill.Enchantment => Enchantment,
            Skill.Alchemy => Alchemy,
            Skill.Traps => Traps,
            Skill.Lockpicking => Lockpicking,
            Skill.Stealing => Stealing,
            Skill.Leatherwork => Leatherwork,
            Skill.Prayers => Prayers,
            Skill.FoodStoring => FoodStoring,
            Skill.Trade => Trade,
            Skill.Tailoring => Tailoring,
            _ => 0,
        };

        public Action<Archetype, Archetype> ClassChange;
        public Sex Sex { get; set; }

        /// <summary>
        /// Усталость в %
        /// </summary>
        public int Tire { get; set; }

        /// <summary>
        /// Утомить
        /// </summary>
        public void Tires(int percent)
        {
            if (Tire + percent > 50)
                Tire = 50;
            else
                Tire += percent;
        }

        public override string Image => Avatar;

        public string Avatar { get; set; }

        public List<Ability> Abilities { get; set; } = new List<Ability>();

        public List<Ability> Effects { get; set; } = new List<Ability>();

        public void Heal(int hp)
        {
            Hp.Add(hp);
        }

        public SpriteSheet WalkSpriteSheet { get; set; }

        public List<Perk> Perks { get; set; } = new List<Perk>();

        public Crafts? Profession { get; set; }

        public Fraction? Fraction { get; set; }

        public Spec? Spec { get; set; }

        //skills

        public int Landscape { get; set; }
        public int Eating { get; set; }
        public int Repair { get; set; }
        public int Smithing { get; set; }

        public int Portals { get; set; }
        public int Attension { get; set; }
        public int Enchantment { get; set; }
        public int Alchemy { get; set; }

        public int Traps { get; set; }
        public int Lockpicking { get; set; }
        public int Stealing { get; set; }
        public int Leatherwork { get; set; }

        public int Prayers { get; set; }
        public int FoodStoring { get; set; }
        public int Trade { get; set; }
        public int Tailoring { get; set; }

        public Inventory Inventory { get; set; } = new Inventory();

        private Action _selectEvent = () => { };
        public void OnSelect(Action select) => _selectEvent+=select;

        public void Select()
        {
            _selectEvent?.Invoke();
        }

        public override TurnType DoTurn()
        {
            return TurnType.Await;
        }
    }
}