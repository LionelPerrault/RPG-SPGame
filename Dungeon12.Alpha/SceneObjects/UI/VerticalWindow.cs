﻿namespace Dungeon12.Drawing.SceneObjects.UI
{
    using Dungeon.Drawing.Impl;
    using System;

    public class VerticalWindow : Dungeon.Drawing.SceneObjects.ImageObject
    {
        public VerticalWindow() : base("Dungeon12.Resources.Images.ui.vertical(17x12).png")
        {
            this.Height = 17;
            this.Width = 12;
        }
    }
}