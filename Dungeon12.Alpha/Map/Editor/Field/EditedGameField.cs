﻿namespace Dungeon12.Map.Editor.Field
{
    using Dungeon.Control;
    using Dungeon.Control.Pointer;
    using Dungeon12.Data.Region;
    using Dungeon.Drawing.SceneObjects;
    using Dungeon.Resources;
    using Dungeon12.SceneObjects; using Dungeon.SceneObjects;
    using Dungeon.Types;
    using Dungeon12.Map.Editor.Objects;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class EditedGameField : EmptySceneControl
    {
        public override bool CacheAvailable => false;
        private int lvl = 1;
        private bool obstruct = false;
        private bool fulltile = false;
        
        public EditedGameField()
        {
            this.Width = 100;
            this.Height = 100;

            History = new GameFieldHistory(this);
            LoadFile();
        }

        private ImageObject current;

        private GameFieldHistory History;

        public void Cancel() => History.Back();

        public void Selecting(ImageObject imageControl) => current = imageControl;

        public void SetLevel(int lvl) => this.lvl = lvl;

        public void SetObstruct(bool obstruct) => this.obstruct = obstruct;

        public void SetFullTile(bool fulltile) => this.fulltile = fulltile;

        public readonly DesignField Field = new DesignField();

        private bool deletion = false;

        public override void Click(PointerArgs args)
        {
            hold = true;

            if (current == null)
                return;

            int x = (int)Math.Truncate((args.X / 32) - (args.Offset.X / 32) - this.Left - (args.Offset.X / 32));
            int y = (int)Math.Truncate((args.Y / 32) - (args.Offset.Y / 32) - this.Top - (args.Offset.Y / 32));

            Console.WriteLine(args.MouseButton);

            if (args.MouseButton == MouseButton.Right)
            {
                deletion = true;
                var exists = Field[lvl][x][y];
                if (exists != null)
                {
                    History.Remove(exists);
                    this.RemoveChild(exists);
                    Field[lvl][x][y] = null;
                }
                return;
            }

            var canPut = Field[lvl][x][y] == null;

            if (!canPut)
            {
                canPut = Field[lvl][x][y].ImageRegion != current.ImageRegion;
                if (canPut)
                {
                    History.Remove(Field[lvl][x][y]);
                    this.RemoveChild(Field[lvl][x][y]);
                    Field[lvl][x][y] = null;
                }
            }            

            if (current != null && canPut)
            {

                double height = 1;
                double width = 1;

                if (fulltile)
                {
                    var measure = MeasureImage(current.Image);
                    width = measure.X;
                    height = measure.Y;
                }
                
                Field[lvl][x][y] = new DesignCell(current.Image,this.obstruct)
                {
                    ImageRegion = current.ImageRegion,
                    Left = x,
                    Top = y,
                    Height = height,
                    Width = width,
                    LayerLevel = lvl
                };
                this.AddChild(Field[lvl][x][y]);
                History.Add(Field[lvl][x][y]);
            }
        }

        public void Save(string save, bool measure = true)
        {
            this.Field.Save();
            this.Children.Clear();

            if (measure)
            {
                var size = MeasureImage(save);
                this.AddChild(new ImageObject(save)
                {
                    Width = size.X,
                    Height = size.Y,
                });
            }
            else
            {
                this.AddChild(new ImageObject(save));
            }
        }

        private void LoadFile()
        {
            if (!File.Exists("map.json"))
                return;

            var file = JsonConvert.DeserializeObject<List<RegionPart>>(File.ReadAllText("map.json"));
            foreach (var item in file)
            {
                var lvl = item.Layer;
                int x = (int)item.Position.X;
                int y = (int)item.Position.Y;

                try
                {
                    Field[lvl][x][y] = new DesignCell(item.Image, item.Obstruct)
                    {
                        ImageRegion = item?.Region,
                        Left = x,
                        Top = y,
                        Height = item?.Region.Height ??0,
                        Width = item?.Region.Width??0,
                        LayerLevel = lvl
                    };
                }
                catch
                {
                    Console.WriteLine();
                }
            }
             

            var ms = new MemoryStream(File.ReadAllBytes("map.png"));
            ResourceLoader.SaveStream(ms.ToArray(), "map");
            this.Save($"map",false);
        }

        public override Rectangle CropPosition => new Rectangle
        {
            X = this.BoundPosition.X,
            Y = this.BoundPosition.Y,
            Height = this.Children.Skip(1).Max(c => c.BoundPosition.Y + c.BoundPosition.Height),
            Width = this.Children.Skip(1).Max(c => c.BoundPosition.X + c.BoundPosition.Width)
        };

        public override void GlobalClickRelease(PointerArgs args)
        {
            hold = false;
        }

        public override void ClickRelease(PointerArgs args)
        {
            hold = false;
        }

        public override void MouseMove(PointerArgs args)
        {
            if (hold)
                Click(args);
        }

        bool hold = false;


        private ControlEventType[] HoldHandles => new ControlEventType[]
        {
            ControlEventType.ClickRelease,
            ControlEventType.Click,
            ControlEventType.GlobalClickRelease,
                ControlEventType.MouseMove
        };

        protected override ControlEventType[] Handles => HoldHandles;
    }
}
