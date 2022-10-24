﻿using Microsoft.Xna.Framework.Graphics;

namespace Dungeon.Monogame.Effects
{
    public interface IMonogameEffect
    {
        public bool Loaded { get; set; }

        public bool UseGlobalImageFilter { get; }

        public bool NotDrawOriginal { get; }

#if !Engine
        void Load(GameClient client);
#endif

        Texture2D Draw(RenderTarget2D input);
    }
}
