﻿using Dungeon;
using Dungeon12.Entities.Enums;
using Dungeon12.Entities.FractionPolygons;

namespace Dungeon12.Attributes
{
    internal class FractionInfluenceAttribute : ValueAttribute
    {
        public FractionInfluenceAttribute(FractionInfluenceAbility ability) : base(ability)
        {
        }
    }
}
