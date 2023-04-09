﻿using MathNet.Numerics.Distributions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Dungeon.Monogame
{
    public partial class GameClient
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr LoadLibraryW(string lpszLib);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetNumVideoDisplays();

        public delegate void GetDisplayBounds(int index, out SDL_Rect rect);

        public struct SDL_Rect
        {
            public int x { get; set; }

            public int y { get; set; }

            public int w { get; set; }

            public int h { get; set; }
        }

        private static List<SDL_Rect> MonitorBounds = new List<SDL_Rect>();
        private static bool SDLLoaded=false;

        private void SDL_InitMonitors()
        {
            if (SDLLoaded)
            {
                // that means counter already +1 on SDL library (monogame import internal same dll)
                // So, we not need dispose it, but it will be very good not increment counter
                // acutally, if GameClient will be reinited, but application is not - it can be potential increment
                return;
            }

            var entrydll = Assembly.GetExecutingAssembly().Location;
            var root = Path.GetDirectoryName(entrydll);
#warning SDL runtime platform check
            var sdlPath = Path.Combine(root, @"runtimes\win-x64\native\SDL2.dll");

            if (!File.Exists(sdlPath))
                sdlPath = Path.Combine(root, "SDL2.dll");

            if (!File.Exists(sdlPath))
            {
                Console.WriteLine("Cant find SDL2.dll, monitor choosing is not available!");
                return;
            }

            var SDL = LoadLibraryW(sdlPath);
            SDLLoaded=true; 

            var SDL_GetNumVideoDisplays = GetProcAddress(SDL, "SDL_GetNumVideoDisplays");
            var SDL_GetNumVideoDisplaysFunc = Marshal.GetDelegateForFunctionPointer<GetNumVideoDisplays>(SDL_GetNumVideoDisplays);

            var dCount = SDL_GetNumVideoDisplaysFunc();

            var SDL_GetDisplayBounds = GetProcAddress(SDL, "SDL_GetDisplayBounds");
            var SDL_GetDisplayBoundsFunc = Marshal.GetDelegateForFunctionPointer<GetDisplayBounds>(SDL_GetDisplayBounds);

            for (int i = 0; i < dCount; i++)
            {
                SDL_Rect r = new SDL_Rect();
                SDL_GetDisplayBoundsFunc(i, out r);
                MonitorBounds.Add(r);
            }
        }

        private bool SetMonitor(int index)
        {
            var bounds = MonitorBounds.ElementAtOrDefault(index);
            if (MonitorBounds.Count == 1)
                bounds = MonitorBounds[0];
            if (bounds.x != 0) {
                var isfullscreen = _settings.WindowMode == Settings.WindowMode.FullScreenHardware || _settings.WindowMode == Settings.WindowMode.FullScreenSoftware;
                Window.Position = new Point(bounds.x, isfullscreen ? 0 : 50);
            }

            var resolution = DungeonGlobal.Resolution;

            if (resolution.Width!=bounds.w || resolution.Height!=bounds.h)
            {
                if (bounds.w > resolution.Width || bounds.h>resolution.Height)
                {
                    Window.Position = new Point
                    {
                        X  = bounds.w / 2 - resolution.Width / 2,
                        Y  = bounds.h / 2 - resolution.Height / 2
                    };
                }
            }

            return false;
        }

        private void FitBounds(SDL_Rect bounds)
        {
            graphics.PreferredBackBufferWidth = bounds.w;
            graphics.PreferredBackBufferHeight = bounds.h;
            graphics.ApplyChanges();
            DungeonGlobal.Resolution = new View.PossibleResolution(bounds.w, bounds.h);

            Types.Dot left = Types.Dot.Zero;
            Types.Dot right = Types.Dot.Zero;

            var size = new Types.Dot(bounds.w, bounds.h);

            if (originSize.X > bounds.w)
            {
                left = size;
                right = originSize;
            }
            else if (originSize.X < bounds.w)
            {
                left = originSize;
                right = size;
            }

            var scaleX = left.Xf / right.Xf;
            var scaleY = left.Yf / right.Yf;

            var scale = new Vector3(scaleX, scaleY, 1);

            ResolutionMatrix = Matrix.CreateScale(scale);

            DungeonGlobal.ResolutionScaleMatrix =
                new System.Numerics.Matrix4x4(
                    ResolutionMatrix.M11,
                    ResolutionMatrix.M12,
                    ResolutionMatrix.M13,
                    ResolutionMatrix.M14,
                    ResolutionMatrix.M21,
                    ResolutionMatrix.M22,
                    ResolutionMatrix.M23,
                    ResolutionMatrix.M24,
                    ResolutionMatrix.M31,
                    ResolutionMatrix.M32,
                    ResolutionMatrix.M33,
                    ResolutionMatrix.M34,
                    ResolutionMatrix.M41,
                    ResolutionMatrix.M42,
                    ResolutionMatrix.M43,
                    ResolutionMatrix.M44);
        }
    }
}
