﻿using Dungeon.Physics;
using Dungeon.Types;
using System.Collections.Generic;

namespace Dungeon12.Entities.Map
{
    public class Location
    {
        public Region Region { get; set; }

        public string Name { get; set; }

        public string BackgroundImage { get; set; }

        public string ObjectImage { get; set; }

        public string ObjectId { get; set; }

        public Polygon Polygon { get; set; }

        public int Index { get; set; }

        public int[] IndexLinks { get; set; }

        public List<Location> Links { get; set; }

        public Point Size { get; set; }

        public Point Position { get; set; }

        public bool IsOpen { get; set; }
    }
}