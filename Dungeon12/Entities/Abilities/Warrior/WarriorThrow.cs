﻿using Dungeon12.Entities.Enums;

namespace Dungeon12.Entities.Abilities.Warrior
{
    public class WarriorThrow : Ability
    {
        public override Archetype Class => Archetype.Warrior;

        public override void Bind()
        {
            Name = Global.Strings.WarriorThrow;
            Description = Global.Strings.WarriorThrowDescT1;
            Area = new AbilityArea();
            Element = Element.Physical;
            Cooldown = 3;
            UseRange = AbilRange.Far;
        }

        public override string[] GetTextParams()
        {
            return new string[] {
                $"{Global.Strings.Damage}: {Value}",
                $"{Global.Strings.Type}: {Element.Display()}",
                Global.Strings.Taunt,
                $"{Global.Strings.Range}: {UseRange.Display()}",
            };
        }
    }
}
