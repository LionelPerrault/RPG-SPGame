﻿namespace Rogue.Map.Objects
{
    using System;
    using Rogue.Map.Infrastructure;
    using Rogue.Physics;
    using Rogue.Settings;
    using Rogue.Types;

    [Template("@")]
    public class Avatar : MapObject
    {
        public override double MovementSpeed { get; set; } = 0.04;

        public override bool CameraAffect => true;

        public Entites.Alive.Character.Player Character { get; set; }

        public override string Tileset => Character.Tileset;

        public override Rectangle TileSetRegion => Character.TileSetRegion;

        public override string Icon { get => "@"; set { } }

        public override bool Obstruction => true;

        public Avatar()
        {
            this.ForegroundColor = new MapObjectColor
            {
                R = 255,
                A = 255
            };
        }

        public override PhysicalSize Size
        {
            get => new PhysicalSize
            {
                Height = 16,
                Width = 16
            };
            set { }
        }

        public override PhysicalPosition Position
        {
            get => new PhysicalPosition
            {
                X = base.Position.X + 8,
                Y = base.Position.Y + 14
            };
            set { }
        }

        protected override MapObject Self => this;

        public override void Interact(GameMap gameMap)
        {
        }
    }
}