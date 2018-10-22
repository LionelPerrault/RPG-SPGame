﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rogue.Drawing.Impl;
using Rogue.View.Interfaces;

namespace Rogue.Drawing.Console
{
    /// <summary>
    /// Interface: Button; Clickable;
    /// </summary>
    public class Button : Interface
    {
        public Button(Window Window)
        {
            //this.AutoClear = false;
            this.Window = Window;
            this.OnFocus = () => { this.Active = true; };
        }

        public bool Border = true;

        public override bool Activatable => true;

        public Action OnClick;
        private List<List<ColouredChar>> constructed = new List<List<ColouredChar>>();
        public override IEnumerable<IDrawText> Construct(bool Active)
        {
            this.DrawRegion = new Types.Rectangle
            {
                X = this.Window.Left + this.Left,
                Y = this.Window.Top + this.Top,
                Width = this.Width,
                Height = this.Height
            };

            var color = Active
                ? this.ActiveColor
                : this.InactiveColor;
            
            var top = new DrawText((Border ? Window.Border.UpperLeftCorner : ' ') + GetLine((int)this.Width - 2, (Border ? Window.Border.HorizontalLine : ' ')) + (Border ? Window.Border.UpperRightCorner : ' '), Window.BorderColor);

            var mid = DrawText.Empty((int)this.Width,Window.BorderColor);
            mid.ReplaceAt(0, new DrawText((Border ? Window.Border.VerticalLine.ToString() : " "), Window.BorderColor));
            mid.ReplaceAt(1, new DrawText(this.Middle(this.Label), color));
            mid.ReplaceAt((int)this.Width - 1, new DrawText((Border ? Window.Border.VerticalLine.ToString() : " "), Window.BorderColor));

            var bot = new DrawText((Border ? Window.Border.LowerLeftCorner : ' ') + GetLine((int)this.Width - 2, (Border ? Window.Border.HorizontalLine : ' ')) + (Border ? Window.Border.LowerRightCorner : ' '), Window.BorderColor);

            return new IDrawText[] { top, mid, bot };
        }

        public override IDrawSession Run()
        {
            var y = 0;
            var lines = this.Construct(this.Active);
            foreach (var line in lines)
            {
                this.Write(y, 0, line);
                y++;
            }

            return base.Run();
        }
    }
}
