﻿using Dungeon12.Data.Region;
using Dungeon.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Dungeon12.Data
{
    public class TransporterData : RegionPart
    {
        public string Name { get; set; }

        public string UnderlevelIdentify { get; set; }

        public string RegionIdentify { get; set; }

        public Point Destination { get; set; }

        public string LoadingScreenName { get; set; }
    }
}