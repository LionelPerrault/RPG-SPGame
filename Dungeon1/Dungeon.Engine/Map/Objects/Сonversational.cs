﻿namespace Dungeon.Map.Objects
{
    using Dungeon.Conversations;
    using System.Collections.Generic;

    public abstract class Сonversational : MapObject
    {
        public List<Conversation> Conversations { get; set; }

        public string ScreenImage { get; set; }

        public int Frames { get; set; }
    }
}
