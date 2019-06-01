﻿namespace Rogue
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Rogue.Resources;
    using Rogue.Scenes.Manager;
    using Rogue.Scenes.Menus;
    using Rogue.Types;
    using Rogue.View.Interfaces;
    using Rect = Rogue.Types.Rectangle;

    public partial class XNADrawClient : Game, IDrawClient
    {
        public SceneManager SceneManager { get; set; }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private readonly HashSet<Direction> CameraMovings = new HashSet<Direction>();
        public void MoveCamera(Direction direction, bool stop = false)
        {
            if (!stop)
            {
                CameraMovings.Add(direction);
            }
            else
            {
                CameraMovings.Remove(direction);
            }
        }

        public void ResetCamera()
        {
            this.CameraMovings.Clear();
            this.CameraOffsetX = 0;
            this.CameraOffsetY = 0;
        }

        public void SetCameraSpeed(double speed) => cameraSpeed = speed;

        private double cameraSpeed = 2.5;

        private static float cell = 32;

        public double CameraOffsetX { get; set; }

        public double CameraOffsetY { get; set; }

        public double CameraOffsetLimitX { get; set; } = 3200000;

        public double CameraOffsetLimitY { get; set; } = 3200000;

        public XNADrawClient()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // >>>>>>> These two lines below >>>>>>>>>>
            this.IsFixedTimeStep = true;//false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60);
        }

        protected override void Initialize()
        {
            this.Window.Title = "Dungeon 12";
            Window.TextInput += OnTextInput;
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SceneManager = new SceneManager
            {
                DrawClient = this
            };
            SceneManager.Change<Start>();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateLoop();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// DEAD END
        /// </summary>
        /// <param name="drawSessions"></param>
        public void Draw(IEnumerable<IDrawSession> drawSessions)
        {
            throw new System.NotImplementedException();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            Draw(this.scene.Objects);
            DrawFrameInfo();

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void Draw(ISceneObject[] sceneObjects)
        {
            foreach (var sceneObject in sceneObjects)
            {
                DrawSceneObject(sceneObject);
            }

            //if (current != null)
            //{
            //    for (int i = 0; i < current.Objects.Count(); i++)
            //    {
            //        var obj = current.Objects[i];
            //        if (current.AbsolutePositionScene || obj.AbsolutePosition)
            //        {
            //        }
            //        else
            //        {
            //            using (context.PushPostTransform(Matrix.CreateTranslation(CameraOffsetX, CameraOffsetY)))
            //            {
            //                DrawSceneObject(context, obj);
            //            }
            //        }
            //    }
            //}
        }

        #region frameSettings

        private int _frame;
        private TimeSpan _lastFps;
        private int _lastFpsFrame;
        private double _fps;
        Stopwatch _st = Stopwatch.StartNew();

        public static bool frameInfo = true;

        #endregion

        private void DrawFrameInfo()
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

                var font = Content.Load<SpriteFont>("Arial");

                spriteBatch.DrawString(font, text, new Vector2(0, 0), Color.White);
            }

            _frame++;
        }


        private IScene scene;
        public void SetScene(IScene scene) => this.scene = scene;

        public Types.Point MeasureText(IDrawText drawText)
        {
            var font = Content.Load<SpriteFont>(drawText.FontName ?? "Triforce/Triforce30");

            var m =  font.MeasureString(drawText.StringData);

            return new Types.Point(m.X, m.Y);
        }

        public Types.Point MeasureImage(string image)
        {
            var img = TileSetByName(image);
            return new Types.Point()
            {
                X = img.Width,
                Y = img.Height
            };
        }

        public void SaveObject(ISceneObject sceneObject, string path, Types.Point offset, string runtimeCacheName = null)
        {
            throw new System.NotImplementedException();
        }

        public void Animate(IAnimationSession animationSession)
        {
            throw new System.NotImplementedException();
        }

        private readonly Dictionary<string, RenderTarget2D> BatchCache = new Dictionary<string, RenderTarget2D>();
        private Dictionary<string, Rect> TileSetCache = new Dictionary<string, Rect>();
        private Dictionary<string, Rect> PosCahce = new Dictionary<string, Rect>();
        private static readonly Dictionary<string, Texture2D> tilesetsCache = new Dictionary<string, Texture2D>();

        private Texture2D TileSetByName(string tilesetName)
        {
            if (!tilesetsCache.TryGetValue(tilesetName, out var bitmap))
            {
                var stream = ResourceLoader.Load(tilesetName, tilesetName);
                bitmap = Texture2D.FromStream(GraphicsDevice, stream);

                tilesetsCache.TryAdd(tilesetName, bitmap);
            }

            return bitmap;
        }

        private void DrawSceneObject(ISceneObject sceneObject, double xParent = 0, double yParent = 0, bool batching = false, bool force = false)
        {
            if (force && sceneObject.ForceInvisible)
                return;

            var y = sceneObject.Position.Y * cell + yParent;
            var x = sceneObject.Position.X * cell + xParent;

            if (sceneObject.IsBatch && !batching)
            {
                if (sceneObject.Expired || !BatchCache.TryGetValue(sceneObject.Uid, out var bitmap))
                {
                    int width = (int)Math.Round(sceneObject.Position.Width * cell);
                    int height = (int)Math.Round(sceneObject.Position.Height * cell);

                    bitmap = new RenderTarget2D(GraphicsDevice, width, height, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

                    GraphicsDevice.SetRenderTarget(bitmap);

                    DrawSceneObject(sceneObject, xParent, yParent, true);

                    TileSetCache[sceneObject.Uid] = new Rect(0, 0, width, height);
                    PosCahce[sceneObject.Uid] = new Rect(sceneObject.Position.X, sceneObject.Position.Y, width, height);

                    BatchCache[sceneObject.Uid] = bitmap;

                    GraphicsDevice.SetRenderTarget(null);
                }

                TileSetCache.TryGetValue(sceneObject.Uid, out var tilesetPos);
                PosCahce.TryGetValue(sceneObject.Uid, out var sceneObjPos);

                spriteBatch.Draw(bitmap, new Vector2(sceneObjPos.Xf, sceneObjPos.Yf), new Microsoft.Xna.Framework.Rectangle(tilesetPos.Xi, tilesetPos.Yi, tilesetPos.Widthi, tilesetPos.Heighti), Color.White);
            }
            else
            {
                if (!string.IsNullOrEmpty(sceneObject.Image))
                {
                    DrawSceneImage(sceneObject, y, x, force);
                }

                if (sceneObject.Path != null)
                {
                    DrawScenePath(sceneObject.Path, x, y);
                }

                if (sceneObject.Text != null)
                {
                    var text = sceneObject.Text;
                    var textX = x;

                    foreach (var range in text.Data)
                    {
                        DrawSceneText(text.Size, y, textX, range);
                        textX += range.Length * range.LetterSpacing;
                    }
                }

                var childrens = sceneObject.Children.OrderBy(c => c.Layer).ToArray();

                for (int i = 0; i < childrens.Length; i++)
                {
                    var child = childrens.ElementAtOrDefault(i);
                    if (child != null)
                    {
                        DrawSceneObject(child, x, y, batching, force);
                    }
                }
            }
        }

        private void DrawSceneImage(ISceneObject sceneObject, double y, double x, bool force)
        {
            var image = TileSetByName(sceneObject.Image);

            if (force || !TileSetCache.TryGetValue(sceneObject.Uid, out Rect tileRegion))
            {
                if (sceneObject.ImageRegion == null)
                {
                    tileRegion = new Rect(0, 0, image.Width, image.Height);
                }
                else
                {
                    var tilePos = sceneObject.ImageRegion;
                    tileRegion = new Rect(tilePos.X, tilePos.Y, tilePos.Width, tilePos.Height);
                }

                if (!force && sceneObject.CacheAvailable)
                {
                    TileSetCache.Add(sceneObject.Uid, tileRegion);
                }
            }

            if (force || !PosCahce.TryGetValue(sceneObject.Uid, out Rect pos))
            {
                double width = sceneObject.Position.Width;
                double height = sceneObject.Position.Height;

                if (width == 0 && height == 0)
                {
                    width = image.Width;
                    height = image.Height;
                }
                else
                {
                    width *= cell;
                    height *= cell;
                }

                pos = new Rect(x, y, width, height);

                if (!force && sceneObject.CacheAvailable)
                {
                    PosCahce.Add(sceneObject.Uid, pos);
                }
            }

            spriteBatch.Draw(image, new Vector2(pos.Xf, pos.Yf),
                new Microsoft.Xna.Framework.Rectangle(tileRegion.Xi, tileRegion.Yi,
                    tileRegion.Widthi, tileRegion.Heighti),
                Color.White);
        }

        private void DrawSceneText(float fontSize, double y, double x, IDrawText range)
        {
            bool fontWeight = range.Bold;

            SpriteFont spriteFont;
            if (string.IsNullOrEmpty(range.FontName))
            {
                spriteFont = Content.Load<SpriteFont>("Triforce/Triforce30");
            }
            else
            {
                if (string.IsNullOrEmpty(range.FontPath))
                {
                    spriteFont = Content.Load<SpriteFont>(range.FontName+range.Size);
                    //typeface = new Typeface(range.FontName, weight: fontWeight);
                }
                else
                {
                    spriteFont = Content.Load<SpriteFont>(range.FontName);
                    //typeface = new Typeface(Font.GetFontFamily(range.FontName, range.FontPath, range.FontAssembly), fontSize: fontSize);
                }
            }

            var txt = range.StringData;

            var color = new Color(range.ForegroundColor.R, range.ForegroundColor.G, range.ForegroundColor.B, range.ForegroundColor.A);

            spriteBatch.DrawString(spriteFont, txt, new Vector2((int)x, (int)y), color);
        }

        private void DrawScenePath(IDrawablePath drawablePath, double x, double y)
        {
            if (drawablePath.PathPredefined == View.Enums.PathPredefined.Rectangle)
            {
                var color = drawablePath.BackgroundColor;
                
                var drawColor = new Color(color.R, color.G, color.B, (float)color.Opacity);
                var pathReg = drawablePath.Region;

                var rect = new Microsoft.Xna.Framework.Rectangle((int)x, (int)y, (int)(pathReg.Width * cell), (int)(pathReg.Height * cell));
                var cornerRadius = drawablePath.Radius;

                if (drawablePath.Fill)
                {
                    var dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
                    dummyTexture.SetData(new Color[] { drawColor });
                    
                    spriteBatch.Draw(dummyTexture, rect, drawColor);
                }
                else
                {
                    DrawBorder(rect, 1, drawColor);
                }
            }
        }

        /// <summary>
        /// СПИЗЖЕНО
        /// <para>
        /// Will draw a border (hollow rectangle) of the given 'thicknessOfBorder' (in pixels)
        /// of the specified color.
        ///
        /// By Sean Colombo, from http://bluelinegamestudios.com/blog
        /// </para>
        /// </summary>
        /// <param name="rectangleToDraw"></param>
        /// <param name="thicknessOfBorder"></param>
        private void DrawBorder(Microsoft.Xna.Framework.Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            Texture2D pixel;

            // Somewhere in your LoadContent() method:
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White }); // so that we can draw whatever color we want on top of it

            // Draw top line
            spriteBatch.Draw(pixel, new Microsoft.Xna.Framework.Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            spriteBatch.Draw(pixel, new Microsoft.Xna.Framework.Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            spriteBatch.Draw(pixel, new Microsoft.Xna.Framework.Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            spriteBatch.Draw(pixel, new Microsoft.Xna.Framework.Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }
    }
}