﻿namespace Dungeon12.Drawing.SceneObjects.Main.CharacterBar
{
    using Dungeon.Control.Keys;
    using Dungeon12.Drawing.SceneObjects.Main.CharacterInfo;
    using Dungeon12.Drawing.SceneObjects.Map;
    using Dungeon.View.Interfaces;
    using System;
    using Dungeon;
    using Dungeon.Drawing.SceneObjects;
    using System.Collections.Generic;
    using Dungeon.Control;

    public class SkillsButton : SlideComponent
    {
        public override bool AbsolutePosition => true;

        public override bool CacheAvailable => false;

        private PlayerSceneObject playerSceneObject;

        public SkillsButton(PlayerSceneObject playerSceneObject) : base("Навыки (V)")
        {
            this.playerSceneObject = playerSceneObject;

            this.Height = 1.5;
            this.Width = 1.5;

            this.AddChild(new ImageObject("Dungeon12.Resources.Images.ui.player.skills.png")
            {
                CacheAvailable = false,
                Height = 1.5,
                Width = 1.5,
            });

            this.Image = SquareTexture(false);
        }

        private string SquareTexture(bool focus)
        {
            var f = focus
                ? "_f"
                : "";

            return $"Dungeon12.Resources.Images.ui.square{f}.png";
        }

        public override void Focus()
        {
            this.Image = SquareTexture(true);
            base.Focus();
        }

        public override void Unfocus()
        {
            this.Image = SquareTexture(false);
            base.Unfocus();
        }

        protected override Key[] KeyHandles => new Key[]
        {
            Key.V
        };

        public override void KeyDown(Key key, KeyModifiers modifier, bool hold) => ShowSkillsWindow();

        public override void Click(PointerArgs args) => ShowSkillsWindow();

        private SkillsWindow skillsWindow = null;

        private void ShowSkillsWindow()
        {
            if (skillsWindow != null)
                return;

            playerSceneObject.StopMovings();

            skillsWindow = new SkillsWindow(playerSceneObject);
            skillsWindow.Destroy += () => skillsWindow = null;

            this.ShowInScene(new List<ISceneObject>()
            {
                skillsWindow
            });
        }
    }
}