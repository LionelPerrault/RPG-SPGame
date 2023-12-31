﻿using Dungeon;
using Dungeon.Drawing.SceneObjects;
using Dungeon.SceneObjects;
using Dungeon.Scenes;
using System.Linq;

namespace Dungeon12.Scenes.Game
{
    public class Loading : LoadingScene
    {
        public Loading() : this(default)
        {

        }

        private string loadingscreen;
        public Loading(string loadingscreen)
        {
            this.loadingscreen = loadingscreen;
        }

        public override void Initialize()
        {
            this.AddObject(new ImageObject($"Loading/{loadingscreen}.png".AsmImg()));

            var endText = new TextControl("ЗАГРУЗКА".AsDrawText().InSize(70).Triforce());
            endText.Left = 12;
            endText.Top = 9;
            this.AddObject(endText);
        }
    }
}