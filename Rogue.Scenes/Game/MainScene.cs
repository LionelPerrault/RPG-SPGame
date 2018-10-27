﻿namespace Rogue.Scenes.Game
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Rogue.Control.Keys;
    using Rogue.Control.Pointer;
    using Rogue.DataAccess;
    using Rogue.Drawing.Data;
    using Rogue.Drawing.GUI;
    using Rogue.Drawing.Impl;
    using Rogue.Drawing.Labirinth;
    using Rogue.Map;
    using Rogue.Scenes.Menus;
    using Rogue.Scenes.Scenes;
    using Rogue.Settings;
    using Rogue.Types;

    public class MainScene : GameScene<MainMenuScene>
    {
        private Point PlayerPosition = new Point { X = 27, Y = 8 };

        private readonly DrawingSize DrawingSize = new DrawingSize();

        public MainScene(SceneManager sceneManager) : base(sceneManager)
        {
        }

        public override bool Destroyable => false;

        public override void Draw()
        {
            if (this.Location == null)
                this.InitMap();

            if (this.Commands.Count == 0)
                this.FillCommands();

            Drawing.Draw/*.Session<GUIBorderDrawSession>()*/
                .Session<LabirinthDrawSession>(x => x.Location = this.Location)
                //.Then<CharMapDrawSession>(x => x.Commands = this.Commands.Where(c => c.UI).Select(c => $"[{c.Keys.First()}] - {c.Name}").ToArray())
                //.Then<CharacterDataDrawSession>(x => x.Player = this.Player)
                //.Then<MessageDrawSession>(x => x.Message = new DrawText($"{DateTime.Now.ToShortTimeString()}: Вы прибываете в столицу", ConsoleColor.DarkGray))
                .Publish();
        }

        private void FillCommands()
        {
            this.Commands.Add(new MoveCommand()
            {
                Location = this.Location,
                PlayerPosition = this.PlayerPosition,
                Player = this.Player
            });

            //this.Commands.Add(new Control.Commands.Command { Key = Key.E, Name = "Действие" });
            //this.Commands.Add(new Control.Commands.Command { Key = Key.F, Name = "Подобрать" });
            //this.Commands.Add(new Control.Commands.Command { Key = Key.C, Name = "Персонаж" });
            //this.Commands.Add(new Control.Commands.Command { Key = Key.I, Name = "Инвентарь" });
            //this.Commands.Add(new Control.Commands.Command { Key = Key.Q, Name = "Атаковать" });
            //this.Commands.Add(new Control.Commands.Command { Key = Key.Z, Name = "Осмотреться" });
            //this.Commands.Add(new Control.Commands.Command { Key = Key.R, Name = "Способности" });
            //this.Commands.Add(new Control.Commands.Command { Key = Key.Escape, Name = "Меню" });
        }

        private void InitMap()
        {
            this.Location = new Location
            {
                Biom = ConsoleColor.DarkGray
            };

            var persistMap = Database.Entity<Data.Maps.Map>(x => x.Identity == "Capital").First();

            this.Location.Name = persistMap.Name;

            foreach (var line in persistMap.Template.Split(Environment.NewLine).Take(19))
            {
                var listLine = new List<List<Map.MapObject>>();

                foreach (var @char in line.Substring(0, 35))
                {
                    listLine.Add(new List<MapObject>() { MapObject.Create(@char.ToString()) });
                }

                this.Location.Map.Add(listLine);
            }

            //перенести туда где location
            this.Location.Map[8][27].Add(new Map.Objects.Player
            {
                Character = this.Player
            });
        }

        protected override void KeyPress(Key keyPressed, KeyModifiers keyModifiers)
        {
            if (keyPressed == Key.Escape)
                this.Switch<MainMenuScene>();

#if DEBUG
            if (keyPressed == Key.U)
                drawMode = true;

            if(drawMode)
            {
                if(keyPressed== Key.W)
                {
                    this.drawChar = "#";
                }

                if (keyPressed == Key.F)
                {
                    this.drawChar = ".";
                }

                if (keyPressed == Key.E)
                {
                    var export = string.Empty;
                    foreach (var line in this.Location.Map)
                    {
                        foreach (var @char in line)
                        {
                            export += @char.First().Icon;
                        }
                        export += Environment.NewLine;
                    }
                    Debugger.Break();
                }
            }
#endif
        }


#if DEBUG
        private bool drawMode = false;
        private string drawChar = ".";
#endif

        protected override void MousePress(PointerArgs pointerPressedEventArgs)
        {
#if DEBUG
            if (drawMode)
            {
                var trulyX = pointerPressedEventArgs.X - 20.125;
                var trulyY = pointerPressedEventArgs.Y - 27;

                int x = (int)Math.Round(trulyX / 25, MidpointRounding.ToEven);
                int y = (int)Math.Round(trulyY / 25, MidpointRounding.ToEven);

                this.Location.Map[y][x].RemoveAt(0);
                this.Location.Map[y][x].Insert(0, MapObject.Create(drawChar));

                this.Draw();
                this.Redraw();
            }
#endif
        }
    }
}
