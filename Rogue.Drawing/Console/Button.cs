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
        }
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

            var top = new DrawText(Window.Border.UpperLeftCorner + GetLine(this.Width - 2, Window.Border.HorizontalLine) + Window.Border.UpperRightCorner, Window.BorderColor);

            var mid = DrawText.Empty(this.Width);
            mid.ReplaceAt(0, new DrawText(Window.Border.VerticalLine.ToString(), Window.BorderColor));
            mid.ReplaceAt(1, new DrawText(this.Middle(this.Label), color));
            mid.ReplaceAt(this.Width - 1, new DrawText(Window.Border.VerticalLine.ToString(), Window.BorderColor));

            var bot = new DrawText(Window.Border.LowerLeftCorner + GetLine(this.Width - 2, Window.Border.HorizontalLine) + Window.Border.LowerRightCorner, Window.BorderColor);

            return new IDrawText[] { top, mid, bot };
        }

        public override IDrawSession Run()
        {

            var lines = constructed;
            foreach (List<ColouredChar> line in lines)
            {
                var linePos = lines.IndexOf(line);

                foreach (ColouredChar c in line)
                {
                    var charPos = line.IndexOf(c);

                    this.Write(linePos, charPos, c.Char.ToString(), (ConsoleColor)c.Color, (ConsoleColor)c.BackColor);
                }
            }

            return base.Run();
        }
    }
}
