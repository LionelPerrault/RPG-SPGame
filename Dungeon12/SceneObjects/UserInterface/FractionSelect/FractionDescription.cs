﻿using Dungeon;
using Dungeon.Drawing.SceneObjects;
using Dungeon.SceneObjects;
using Dungeon12.Attributes;
using Dungeon12.Entities.Enums;
using Dungeon12.Entities.Zones;
using Dungeon12.Functions.ObjectFunctions;
using Dungeon12.SceneObjects.UserInterface.Common;
using Dungeon12.SceneObjects.UserInterface.OriginSelect;
using System;
using System.Linq;

namespace Dungeon12.SceneObjects.UserInterface.FractionSelect
{
    public class FractionDescription : EmptySceneControl
    {
        public FractionDescription()
        {
            Left = 486 + 300;
            Top = 248 + 135.5;

            Image = "FracPanel/fracpanel.png".AsmImg();
            this.Width = 513;
            this.Height = 380;

            var btn = this.AddChildCenter(new MapButton() { OnClick = SelectFrac });
            btn.Top = 297;
        }


        private Fraction? fraction;

        public bool IsEmpty => fraction == null;

        public void Load(Fraction fraction)
        {
            this.fraction = fraction;

            this.Children.ToArray().ForEach(c =>
            {
                if (c is MapButton)
                    return;
                c.Destroy?.Invoke();
            });

            var title = this.AddTextCenter(fraction.ToDisplay().AsDrawText().Gabriela().InSize(24), vertical: false);
            title.Top = 13;

            var line1 = this.AddChildImageCenter(new ImageObject("FracPanel/line455.png".AsmImg()));
            line1.Top = 57;

            var text = this.AddTextCenter(fraction.ToValue<string>().AsDrawText().Gabriela().InSize(18).WithWordWrap(), vertical: false);
            text.Top = 72;

            var line2 = this.AddChildImageCenter(new ImageObject("FracPanel/line455.png".AsmImg()));
            line2.Top = 167;

            var availableroles = this.AddTextCenter("Доступные роли:".AsDrawText().Gabriela().InSize(20));
            availableroles.Left = 38 - 10;
            availableroles.Top = 190;

            var coordx = 40;

            fraction.ToValue<AvailableRolesAttribute, Roles[]>().ForEach(x =>
            {
                var badge = this.AddChild(new IconEnumBadge(x));
                badge.Top = 229;
                badge.Left = coordx;
                coordx += 59;
                this.Layer.AddControl(badge);
            });

            var availablespecs = this.AddTextCenter("Специализации:".AsDrawText().Gabriela().InSize(20));
            availablespecs.Left = 292 - 10;
            availablespecs.Top = 190;

            coordx = 282;

            fraction.ToValue<AvailableSpecsAttribute, Spec[]>().ForEach(x =>
            {
                var badge = this.AddChild(new IconEnumBadge(x));
                badge.Top = 229;
                badge.Left = coordx;
                coordx += 55;
                this.Layer.AddControl(badge);
            });
        }

        private void SelectFrac()
        {
            Global.Game.Party.Hero1.Fraction = fraction;
            FractionSelectSceneObject.Instance.Destroy();

            Global.Game.Location.Polygon.P4.Load(new Entities.Map.Polygon
            {
                Name = "Должность",
                Icon = "specscroll.png",
                Function = nameof(SelectFractionFunction)
            });
        }
    }
}