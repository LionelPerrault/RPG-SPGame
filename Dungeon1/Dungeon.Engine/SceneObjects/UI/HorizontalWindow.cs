﻿namespace Dungeon.Drawing.SceneObjects.UI
{
    public class HorizontalWindow : ImageControl
    {
        public HorizontalWindow(string img=null) : base(img ?? "Dungeon.Resources.Images.ui.horizontal(20x13).png")
        {
            //this.Height = 13;
            //this.Width = 20;
        }
    }
}