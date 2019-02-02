﻿namespace Rogue.App.Visual
{
    using Avalonia;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Media.Immutable;
    using Avalonia.Visuals.Media.Imaging;
    using Avalonia.VisualTree;
    using Rogue.App.Utils;
    using Rogue.Resources;
    using Rogue.View.Interfaces;
    using SkiaSharp;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Timers;
    using Size = Avalonia.Size;

    public class AppVisual : Avalonia.Controls.Control, IRenderTimeCriticalVisual, IDrawClient
    {
        private static float cell = 32;

        public static IDrawClient AppVisualDrawClient = null;
        
        public AppVisual()
        {
            AppVisualDrawClient = this;
        }

        public bool HasRenderTimeCriticalContent => true;

        public bool ThreadSafeHasNewFrame => true;

        private readonly Rect DrawingDisplay = new Rect(0, 0, 1280, 720);

        #region frameSettings

        private int _frame;
        private TimeSpan _lastFps;
        private int _lastFpsFrame;
        private double _fps;
        Stopwatch _st = Stopwatch.StartNew();
        private Typeface _typeface = Typeface.Default;

        public static bool frameInfo = true;

        #endregion

        private Bitmap Display = null;

        private Bitmap Buffer = null;

        public void ThreadSafeRender(DrawingContext context, Size logicalSize, double scaling)
        {
            if (current != null)
            {
                for (int i = 0; i < current.Objects.Count(); i++)
                {
                    DrawSceneObject(context, current.Objects[i]);
                }
            }

            DrawFrameInfo(context);
        }
        
        private void DrawFrameInfo(DrawingContext drawingContext)
        {
            if (frameInfo)
            {
                var nowTs = _st.Elapsed;
                var now = DateTime.Now;
                var fpsTimeDiff = (nowTs - _lastFps).TotalSeconds;
                if (fpsTimeDiff > 1)
                {
                    _fps = (_frame - _lastFpsFrame) / fpsTimeDiff;
                    _lastFpsFrame = _frame;
                    _lastFps = nowTs;
                }

                var text = $"Frame: {_frame}\nFPS: {_fps}\nNow: {now}";
                //text += $"\nTransform{context.CurrentTransform}";
                //text += $"\nContainer Transform{context.CurrentContainerTransform}";
                var fmt = new FormattedText()
                {
                    Text = text,
                    Typeface = _typeface
                };
                var back = new ImmutableSolidColorBrush(Colors.LightGray);
                var textBrush = new ImmutableSolidColorBrush(Colors.Black);
                drawingContext.DrawText(textBrush, new Point(5, 5), fmt);
            }

            _frame++;
        }
        
        private void Splash(DrawingContext context)
        {
            var splash = ResourceLoader.Load("Rogue.Resources.Images.d12back.png");
            Buffer = Display = new Bitmap(splash);
        }

        private static Action<DrawingContext> DrawLoop;

        private static readonly ConcurrentDictionary<string, Bitmap> tilesetsCache = new ConcurrentDictionary<string, Bitmap>();

        private static Bitmap TileSetByName(string tilesetName)
        {
            if (!tilesetsCache.TryGetValue(tilesetName, out var bitmap))
            {
                var stream = ResourceLoader.Load(tilesetName, tilesetName);
                bitmap = new Bitmap(stream);

                tilesetsCache.TryAdd(tilesetName, bitmap);
            }

            return bitmap;
        }

        public void Draw(IEnumerable<IDrawSession> drawSessions)
        {
            if (drawSessions.Count() == 0)
                return;

            var bitmap = Buffer;

            float fontSize = 20f;
            var font = Font.Common;

            Action<DrawingContext> drawings = null;

            foreach (var session in drawSessions)
            {
                if (session.Drawables != null)
                {
                    drawings += DrawTiles(session.Drawables);
                }

                if (session.TextContent != null && session.TextContent.Any())
                {
                    drawings += DrawText(fontSize, font, session);
                }
            }
                        
            DrawLoop += drawings;
        }

        private static Action<DrawingContext> DrawText(float fontSize, FontFamily font, IDrawSession session)
        {
            Action<DrawingContext> drawings = null;

            float y = session.SessionRegion.Y * cell;
            float x = session.SessionRegion.X * cell;

            foreach (var line in session.TextContent)
            {
                if (line.Region != null)
                {
                    drawings+=DrawPositionalText(fontSize, font, line);
                }
                else
                {
                    drawings+=DrawNonPositionalText(fontSize, font, y, x, line);
                    y += line.Size;
                }
            }

            return drawings;
        }

        /// <summary>
        /// Рисование надписи без собственного позиционирования (как консоль)
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="fontSize"></param>
        /// <param name="font"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="line"></param>
        private static Action<DrawingContext> DrawNonPositionalText(float fontSize, FontFamily font, float y, float x, IDrawText line)
        {
            Action<DrawingContext> drawings = null;

            foreach (var lne in line.Data)
            {
                foreach (var range in lne.Data)
                {
                    drawings += DrawTextRanges(fontSize, font, y, x, range);
                }
            }

            return drawings;
        }

        /// <summary>
        /// Рисование надписи с собственным позиционированием
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="fontSize"></param>
        /// <param name="font"></param>
        /// <param name="drawText"></param>
        private static Action<DrawingContext> DrawPositionalText(float fontSize, FontFamily font, IDrawText drawText)
        {
            Action<DrawingContext> drawings = null;

            float y = drawText.Region.Y * cell;
            float x = drawText.Region.X * cell;

            foreach (var range in drawText.Data)
            {
                drawings+=DrawTextRanges(drawText.Size, font, y, x, range);
                x += range.Length * range.LetterSpacing;
            }

            return drawings;
        }

        /// <summary>
        /// Рисование внутренних отрезков в тексте (отрезки с собственным форматированием)
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="fontSize"></param>
        /// <param name="font"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="range"></param>
        private static Action<DrawingContext> DrawTextRanges(float fontSize, FontFamily font, float y, float x, IDrawText range)
        {
            Action<DrawingContext> drawings = null;

            var fmt = new FormattedText()
            {
                Text = range.StringData,
                Typeface = new Typeface(font, fontSize: fontSize)
            };

            var brush = new ImmutableSolidColorBrush(new Color(range.ForegroundColor.A, range.ForegroundColor.R, range.ForegroundColor.G, range.ForegroundColor.B), 1);

            x += (float)fmt.Measure().Height;

            drawings = ctx =>
            {
                ctx.DrawText(brush, new Point(x, y), fmt);                
            };

            return drawings;
        }

        /// <summary>
        /// Рисование тайлов
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="drawables"></param>
        private static Action<DrawingContext> DrawTiles(IEnumerable<IDrawable> drawables)
        {
            Action<DrawingContext> drawings = null;

            foreach (var drawable in drawables)
            {
                if (drawable.Container)
                    continue;

                var y = drawable.Region.Y * cell;
                var x = drawable.Region.X * cell;

                var tileset = TileSetByName(drawable.Tileset);

                var tilePos = new Rect(
                    drawable.TileSetRegion.X, drawable.TileSetRegion.Y,
                    (float)drawable.TileSetRegion.Width, (float)drawable.TileSetRegion.Height);

                drawings += ctx =>
                 {
                     ctx.DrawImage(
                         tileset,
                         1,
                         tilePos,
                         new Rect(x, y, drawable.Region.Width * cell, drawable.Region.Height * cell),
                         BitmapInterpolationMode.HighQuality);
                 };
            }

            return drawings;
        }

        public void Animate(IAnimationSession animationSession)
        {
            throw new NotImplementedException();
        }

        public IScene current = null;

        public void SetScene(IScene scene)
        {
            if (current != scene)
                current = scene;

            float fontSize = 20f;
            var font = Font.Common;

            Action<DrawingContext> drawings = null;
            drawings = DrawSceneObjects(scene, fontSize, font, drawings);

            DrawLoop += drawings;
        }

        private static Action<DrawingContext> DrawSceneObjects(IScene scene, float fontSize, FontFamily font, Action<DrawingContext> drawings)
        {
            foreach (var sceneObject in scene.Objects)
            {
                //drawings = DrawSceneObject(fontSize, font, drawings, sceneObject);
            }

            return drawings;
        }

        private void DrawSceneObject(DrawingContext ctx, ISceneObject sceneObject, float xParent=0, float yParent=0)
        {
            var y = sceneObject.Position.Y * cell + yParent;
            var x = sceneObject.Position.X * cell + xParent;

            if (!string.IsNullOrEmpty(sceneObject.Image))
            {
                DrawSceneImage(ctx, sceneObject, y, x);
            }

            if (sceneObject.Text != null)
            {
                var text = sceneObject.Text;
                var textX = x;

                foreach (var range in text.Data)
                {
                    DrawSceneText(ctx,text.Size, y, textX, range);
                    textX += range.Length * range.LetterSpacing;
                }
            }

            foreach (var child in sceneObject.Children)
            {
                DrawSceneObject(ctx, child, x, y);
            }
        }

        private void DrawSceneText(DrawingContext ctx, float fontSize, float y, float x, IDrawText range)
        {
            var fmt = new FormattedText()
            {
                Text = range.StringData,
                Typeface = new Typeface(Font.Common, fontSize: fontSize)
            };

            var brush = new ImmutableSolidColorBrush(new Color(range.ForegroundColor.A, range.ForegroundColor.R, range.ForegroundColor.G, range.ForegroundColor.B), 1);

            x += (float)fmt.Measure().Height;

            ctx.DrawText(brush, new Point(x, y), fmt);
        }

        private Dictionary<string, Rect> TileSetCache = new Dictionary<string, Rect>();
        private Dictionary<string, Rect> PosCahce = new Dictionary<string, Rect>();

        private void DrawSceneImage(DrawingContext ctx, ISceneObject sceneObject, float y, float x)
        {
            var image = TileSetByName(sceneObject.Image);


            if (!TileSetCache.TryGetValue(sceneObject.Uid, out Rect tileRegion))
            {
                if (sceneObject.ImageRegion == null)
                {
                    tileRegion = new Rect(0, 0, image.PixelSize.Width, image.PixelSize.Height);
                }
                else
                {
                    var tilePos = sceneObject.ImageRegion;
                    tileRegion = new Rect(tilePos.X, tilePos.Y, tilePos.Width, tilePos.Height);
                }

                TileSetCache.Add(sceneObject.Uid, tileRegion);
            }

            if (!PosCahce.TryGetValue(sceneObject.Uid, out Rect pos))
            {
                float width = sceneObject.Position.Width;
                float height = sceneObject.Position.Height;

                if (width == 0 && height == 0)
                {
                    width = image.PixelSize.Width;
                    height = image.PixelSize.Height;
                }
                else
                {
                    width *= cell;
                    height *= cell;
                }

                pos = new Rect(x, y, width, height);

                PosCahce.Add(sceneObject.Uid, pos);
            }

            ctx.DrawImage(
                image,
                1,
                tileRegion,
                pos,
                BitmapInterpolationMode.HighQuality);
        }
    }
}