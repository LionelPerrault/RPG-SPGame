﻿namespace Dungeon12.Drawing.SceneObjects.UI
{
    using Dungeon12.Classes;
    using Dungeon.Drawing;
    using Dungeon12.Entities.Alive;
    using Dungeon.View.Interfaces;
    using System;
    using Dungeon.Drawing.SceneObjects;
    using Dungeon12;

    public class ResourceBarHP : ResourceBar
    {
        private readonly ImageObject HpBar;

        public ResourceBarHP(Character avatar)
        { 
            HpBar = new InteractiveHPBar(avatar)
            {
                Left = 0.031,
                Top = 0.031,
                Height = 0.46875
            };

            this.AddChild(HpBar);
        }

        protected override string BarTile => "Dungeon12.Resources.Images.ui.player.hp_back.png";
        
        private class InteractiveHPBar : Dungeon.Drawing.SceneObjects.ImageObject
        {
            private Character player;

            private IDrawText hpText;

            public InteractiveHPBar(Character player) : base("Dungeon12.Resources.Images.ui.player.hp.png")
            {
                this.player = player;

                hpText = new DrawText($"{player.HitPoints}/{player.MaxHitPoints}", ConsoleColor.White)
                {
                    Size = 14
                }.Montserrat();

                var text = this.AddTextCenter(hpText);
                text.Top += 0.2;
                text.Left += 0.2;
            }

            public override double Width
            {
                get
                {
                    hpText.SetText($"{player.HitPoints}/{player.MaxHitPoints}");
                    return 4.75 * (((double)player.HitPoints / player.MaxHitPoints * 100) / 100);
                }
                set { }
            }

            public override bool CacheAvailable => false;
        }

    }
}