﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dungeon.Drawing
{
    public partial class DrawColor
    {
        /// <summary>
        /// 
        /// <para>
        /// [Кэшируемый]
        /// </para>
        /// </summary>
        public static DrawColor GetByName(string name)
        {
            if (!___GetByNameCache.TryGetValue(name, out var value))
            {
                value = typeof(DrawColor).GetField(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                ___GetByNameCache.Add(name, value);
            }

            return value.GetValue(null).As<DrawColor>();
        }
        private static readonly Dictionary<string, FieldInfo> ___GetByNameCache = new Dictionary<string, FieldInfo>();

        public DrawColor Lighter(int plusAlpha)
        {
            return new DrawColor(R, G, B, (byte)(A+plusAlpha));
        }

        public static DrawColor AliceBlue => new DrawColor(240, 248, 255);
        public static DrawColor LightSalmon => new DrawColor(255, 160, 122);
        public static DrawColor AntiqueWhite => new DrawColor(250, 235, 215);
        public static DrawColor LightSeaGreen => new DrawColor(32, 178, 170);
        public static DrawColor Aqua => new DrawColor(0, 255, 255);
        public static DrawColor LightSkyBlue => new DrawColor(135, 206, 250);
        public static DrawColor Aquamarine => new DrawColor(127, 255, 212);
        public static DrawColor LightSlateGray => new DrawColor(119, 136, 153);
        public static DrawColor Azure => new DrawColor(240, 255, 255);
        public static DrawColor LightSteelBlue => new DrawColor(176, 196, 222);
        public static DrawColor Beige => new DrawColor(245, 245, 220);
        public static DrawColor LightYellow => new DrawColor(255, 255, 224);
        public static DrawColor Bisque => new DrawColor(255, 228, 196);
        public static DrawColor Lime => new DrawColor(0, 255, 0);
        public static DrawColor Black => new DrawColor(0, 0, 0);
        public static DrawColor LimeGreen => new DrawColor(50, 205, 50);
        public static DrawColor BlanchedAlmond => new DrawColor(255, 255, 205);
        public static DrawColor Linen => new DrawColor(250, 240, 230);
        public static DrawColor Blue => new DrawColor(0, 0, 255);
        public static DrawColor Magenta => new DrawColor(255, 0, 255);
        public static DrawColor BlueViolet => new DrawColor(138, 43, 226);
        public static DrawColor Maroon => new DrawColor(128, 0, 0);
        public static DrawColor Brown => new DrawColor(165, 42, 42);
        public static DrawColor MediumAquamarine => new DrawColor(102, 205, 170);
        public static DrawColor BurlyWood => new DrawColor(222, 184, 135);
        public static DrawColor MediumBlue => new DrawColor(0, 0, 205);
        public static DrawColor CadetBlue => new DrawColor(95, 158, 160);
        public static DrawColor MediumOrchid => new DrawColor(186, 85, 211);
        public static DrawColor Chartreuse => new DrawColor(127, 255, 0);
        public static DrawColor MediumPurple => new DrawColor(147, 112, 219);
        public static DrawColor Chocolate => new DrawColor(210, 105, 30);
        public static DrawColor MediumSeaGreen => new DrawColor(60, 179, 113);
        public static DrawColor Coral => new DrawColor(255, 127, 80);
        public static DrawColor MediumSlateBlue => new DrawColor(123, 104, 238);
        public static DrawColor CornflowerBlue => new DrawColor(100, 149, 237);
        public static DrawColor MediumSpringGreen => new DrawColor(0, 250, 154);
        public static DrawColor Cornsilk => new DrawColor(255, 248, 220);
        public static DrawColor MediumTurquoise => new DrawColor(72, 209, 204);
        public static DrawColor Crimson => new DrawColor(220, 20, 60);
        public static DrawColor MediumVioletRed => new DrawColor(199, 21, 112);
        public static DrawColor Cyan => new DrawColor(0, 255, 255);
        public static DrawColor MidnightBlue => new DrawColor(25, 25, 112);
        public static DrawColor DarkBlue => new DrawColor(0, 0, 139);
        public static DrawColor MintCream => new DrawColor(245, 255, 250);
        public static DrawColor DarkCyan => new DrawColor(0, 139, 139);
        public static DrawColor MistyRose => new DrawColor(255, 228, 225);
        public static DrawColor DarkGoldenrod => new DrawColor(184, 134, 11);
        public static DrawColor Moccasin => new DrawColor(255, 228, 181);
        public static DrawColor DarkGray => new DrawColor(169, 169, 169);
        public static DrawColor NavajoWhite => new DrawColor(255, 222, 173);
        public static DrawColor DarkGreen => new DrawColor(0, 100, 0);
        public static DrawColor Navy => new DrawColor(0, 0, 128);
        public static DrawColor DarkKhaki => new DrawColor(189, 183, 107);
        public static DrawColor OldLace => new DrawColor(253, 245, 230);
        public static DrawColor DarkMagenta => new DrawColor(139, 0, 139);
        public static DrawColor Olive => new DrawColor(128, 128, 0);
        public static DrawColor DarkOliveGreen => new DrawColor(85, 107, 47);
        public static DrawColor OliveDrab => new DrawColor(107, 142, 45);
        public static DrawColor DarkOrange => new DrawColor(255, 140, 0);
        public static DrawColor Orange => new DrawColor(255, 165, 0);
        public static DrawColor DarkOrchid => new DrawColor(153, 50, 204);
        public static DrawColor OrangeRed => new DrawColor(255, 69, 0);
        public static DrawColor DarkRed => new DrawColor(139, 0, 0);
        public static DrawColor Orchid => new DrawColor(218, 112, 214);
        public static DrawColor DarkSalmon => new DrawColor(233, 150, 122);
        public static DrawColor PaleGoldenrod => new DrawColor(238, 232, 170);
        public static DrawColor DarkSeaGreen => new DrawColor(143, 188, 143);
        public static DrawColor PaleGreen => new DrawColor(152, 251, 152);
        public static DrawColor DarkSlateBlue => new DrawColor(72, 61, 139);
        public static DrawColor PaleTurquoise => new DrawColor(175, 238, 238);
        public static DrawColor DarkSlateGray => new DrawColor(40, 79, 79);
        public static DrawColor PaleVioletRed => new DrawColor(219, 112, 147);
        public static DrawColor DarkTurquoise => new DrawColor(0, 206, 209);
        public static DrawColor PapayaWhip => new DrawColor(255, 239, 213);
        public static DrawColor DarkViolet => new DrawColor(148, 0, 211);
        public static DrawColor PeachPuff => new DrawColor(255, 218, 155);
        public static DrawColor DeepPink => new DrawColor(255, 20, 147);
        public static DrawColor Peru => new DrawColor(205, 133, 63);
        public static DrawColor DeepSkyBlue => new DrawColor(0, 191, 255);
        public static DrawColor Pink => new DrawColor(255, 192, 203);
        public static DrawColor DimGray => new DrawColor(105, 105, 105);
        public static DrawColor Plum => new DrawColor(221, 160, 221);
        public static DrawColor DodgerBlue => new DrawColor(30, 144, 255);
        public static DrawColor PowderBlue => new DrawColor(176, 224, 230);
        public static DrawColor Firebrick => new DrawColor(178, 34, 34);
        public static DrawColor Purple => new DrawColor(128, 0, 128);
        public static DrawColor FloralWhite => new DrawColor(255, 250, 240);
        public static DrawColor Red => new DrawColor(255, 0, 0);
        public static DrawColor ForestGreen => new DrawColor(34, 139, 34);
        public static DrawColor RosyBrown => new DrawColor(188, 143, 143);
        public static DrawColor Fuschia => new DrawColor(255, 0, 255);
        public static DrawColor RoyalBlue => new DrawColor(65, 105, 225);
        public static DrawColor Gainsboro => new DrawColor(220, 220, 220);
        public static DrawColor SaddleBrown => new DrawColor(139, 69, 19);
        public static DrawColor GhostWhite => new DrawColor(248, 248, 255);
        public static DrawColor Salmon => new DrawColor(250, 128, 114);
        public static DrawColor Gold => new DrawColor(255, 215, 0);
        public static DrawColor SandyBrown => new DrawColor(244, 164, 96);
        public static DrawColor Goldenrod => new DrawColor(218, 165, 32);
        public static DrawColor SeaGreen => new DrawColor(46, 139, 87);
        public static DrawColor Gray => new DrawColor(128, 128, 128);
        public static DrawColor Seashell => new DrawColor(255, 245, 238);
        public static DrawColor Green => new DrawColor(0, 128, 0);
        public static DrawColor Sienna => new DrawColor(160, 82, 45);
        public static DrawColor GreenYellow => new DrawColor(173, 255, 47);
        public static DrawColor Silver => new DrawColor(192, 192, 192);
        public static DrawColor Honeydew => new DrawColor(240, 255, 240);
        public static DrawColor SkyBlue => new DrawColor(135, 206, 235);
        public static DrawColor HotPink => new DrawColor(255, 105, 180);
        public static DrawColor SlateBlue => new DrawColor(106, 90, 205);
        public static DrawColor IndianRed => new DrawColor(205, 92, 92);
        public static DrawColor SlateGray => new DrawColor(112, 128, 144);
        public static DrawColor Indigo => new DrawColor(75, 0, 130);
        public static DrawColor Snow => new DrawColor(255, 250, 250);
        public static DrawColor Ivory => new DrawColor(255, 240, 240);
        public static DrawColor SpringGreen => new DrawColor(0, 255, 127);
        public static DrawColor Khaki => new DrawColor(240, 230, 140);
        public static DrawColor SteelBlue => new DrawColor(70, 130, 180);
        public static DrawColor Lavender => new DrawColor(230, 230, 250);
        public static DrawColor Tan => new DrawColor(210, 180, 140);
        public static DrawColor LavenderBlush => new DrawColor(255, 240, 245);
        public static DrawColor Teal => new DrawColor(0, 128, 128);
        public static DrawColor LawnGreen => new DrawColor(124, 252, 0);
        public static DrawColor Thistle => new DrawColor(216, 191, 216);
        public static DrawColor LemonChiffon => new DrawColor(255, 250, 205);
        public static DrawColor Tomato => new DrawColor(253, 99, 71);
        public static DrawColor LightBlue => new DrawColor(173, 216, 230);
        public static DrawColor Turquoise => new DrawColor(64, 224, 208);
        public static DrawColor LightCoral => new DrawColor(240, 128, 128);
        public static DrawColor Violet => new DrawColor(238, 130, 238);
        public static DrawColor LightCyan => new DrawColor(224, 255, 255);
        public static DrawColor Wheat => new DrawColor(245, 222, 179);
        public static DrawColor LightGoldenrodYellow => new DrawColor(250, 250, 210);
        public static DrawColor White => new DrawColor(255, 255, 255);
        public static DrawColor LightGreen => new DrawColor(144, 238, 144);
        public static DrawColor WhiteSmoke => new DrawColor(245, 245, 245);
        public static DrawColor LightGray => new DrawColor(211, 211, 211);
        public static DrawColor Yellow => new DrawColor(255, 255, 0);
        public static DrawColor LightPink => new DrawColor(255, 182, 193);
        public static DrawColor YellowGreen => new DrawColor(154, 205, 50);
    }
}