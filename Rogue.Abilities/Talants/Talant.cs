﻿namespace Rogue.Abilities.Talants
{
    using Rogue.View.Interfaces;

    public class Talant : IDrawable
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public IDrawColor BackgroundColor { get; set; }
        public IDrawColor ForegroundColor { get; set; }

        public string Description { get; set; }

        public int Tier { get; set; }

        public int Level { get; set; }

        public bool Available => Level > 0;
    }
}