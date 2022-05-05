﻿using Dungeon;
using Dungeon.Drawing.SceneObjects;
using Dungeon.SceneObjects;
using Dungeon.View.Interfaces;
using Dungeon12.ECS.Components;
using Dungeon12.Entities;
using Dungeon12.Entities.Abilities;
using Dungeon12.Entities.Enums;
using Dungeon12.SceneObjects.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dungeon12.SceneObjects.HeroPanelObjs
{
    public class AbilityItem : SceneControl<Hero>, ITooltiped, IMouseHint, ICursored
    {
        Ability _ability;

        public AbilityItem(Hero component, Ability ability) : base(component)
        {
            _ability = ability;
            this.Width = 60;
            this.Height = 60;

            this.Image = "UI/start/icon.png".AsmImg();

            this.AddChild(new ImageObject($"Abilities/{ability.ClassName}.png")
            {
                Width = 56,
                Height = 56,
                Left = 2,
                Top = 2
            });
        }

        public override void Focus()
        {
            Image = "UI/start/classselector.png".AsmImg();
            base.Focus();
        }

        public override void Unfocus()
        {
            Image = "UI/start/icon.png".AsmImg();
            base.Unfocus();
        }

        public override bool Visible => Component.Class == _ability.Class;

        public IDrawText TooltipText => $"{Global.Strings.ByProperty(_ability.ClassName)}".AsDrawText().Gabriela();

        public bool ShowTooltip => true;

        public CursorImage Cursor => CursorImage.Question;

        public GraphicsTooltip CreateMouseHint()
            => new GraphicsTooltip(_ability.Name, _ability.Description, GraphicsTooltipSize.AutoByParams, _ability.Area, _ability.Cooldown, _ability.GetTextParams());
    }
}