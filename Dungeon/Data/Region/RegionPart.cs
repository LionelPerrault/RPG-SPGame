﻿using Dungeon.Types;

namespace Dungeon.Data.Region
{

    public class RegionPart : Persist
    {
        public string IdentifyName { get; set; }

        public string Icon { get; set; }

        public string Image { get; set; }

        public Rectangle Region { get; set; }

        public bool Obstruct { get; set; }

        public Point Position { get; set; }

        public int Layer { get; set; }
    }
}