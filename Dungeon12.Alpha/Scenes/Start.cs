﻿namespace Dungeon12.Scenes.Menus
{
    using Dungeon;
    using Dungeon.Control.Keys;
    using Dungeon.Drawing;
    using Dungeon.Drawing.SceneObjects;
    using Dungeon.Physics;
    using Dungeon.Resources;
    using Dungeon.SceneObjects;
    using Dungeon.Scenes;
    using Dungeon.Scenes.Manager;
    using Dungeon12;
    using Dungeon12.CardGame.Scene;
    using Dungeon12.Data.Region;
    using Dungeon12.Drawing.SceneObjects;
    using Dungeon12.Drawing.SceneObjects.UI;
    using Dungeon12.Map.Editor;
    using Dungeon12.SceneObjects;
    using Dungeon12.Scenes.Game;
    using Dungeon12.Scenes.SaveLoad;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class Start : GameScene<SoloDuoScene, MainScene, EditorScene, CardGameScene,Start, SaveLoadScene>
    {
        public override bool AbsolutePositionScene => true;

        public Start(SceneManager sceneManager) : base(sceneManager)
        {
        }

        public override bool Destroyable => true;

        private bool isGame;

        private void MigrateMapDataToTextures()
        {
            var data = ResourceLoader.Load("Data/Regions/map.json".AsmNameRes());
            var json = data.Stream.AsString();

            var settings = Global.GetSaveSerializeSettings();

            var r = JsonConvert.DeserializeObject<Region>(json, settings);
            var textures = r.Objects.Where(x => !x.Obstruct).Select(x =>
            {
                var size = x.Region == default
                    ? Global.DrawClient.MeasureImage(x.Image.Replace("Rogue.", "Dungeon12."))
                    : new Dungeon.Types.Point(x.Region.Width, x.Region.Height);
                var projection = new PhysicalObjectProjection()
                {
                    Size = new Dungeon.Physics.PhysicalSize()
                    {
                        Width = size.X,
                        Height = size.Y
                    },
                    Position = new Dungeon.Physics.PhysicalPosition()
                    {
                        X = x.Position.X,
                        Y = x.Position.Y
                    }
                };
                return projection;
            });

            var f = JsonConvert.SerializeObject(textures, settings);
            File.WriteAllText("FaithIsland.DeepCaveTextures.json", f);
            Debugger.Break();
        }

        public override void Initialize()
        {

#warning подорожник на все проблемы со слоями
            DragAndDropSceneControls.DraggableLayers = 0;
            isGame = Args?.ElementAtOrDefault(0) != default;

            if (!isGame)
            {
                Global.AudioPlayer.Music("Audio/Music/maintheme.ogg".AsmNameRes(), new Dungeon.Audio.AudioOptions()
                {
                    Repeat = true,
                    Volume = 0.3
                });
            }

            Global.GameState?.Player?.StopMovings();

            this.AddObject(new Background(true)
            {
                AbsolutePosition = true,
            });
            this.AddObject(new ImageObject("Dungeon12.Resources.Images.d12textM.png")
            {
                Top = 2f,
                Left = 10f,
                AbsolutePosition = true,
            });

            this.AddObject(new MetallButtonControl(isGame ? "Главное меню" : "Новая игра")
            {
                Left = 15.5f,
                Top = 8,
                OnClick = () =>
                {
                    if (isGame)
                    {
                        QuestionBox.Show(new QuestionBoxModel()
                        {
                            Text=$"Вы уверены что хотите выйти в главное меню?{Environment.NewLine}Весь не сохранённый прогресс пропадет.",
                            Yes = () =>
                            {
                                Global.RemoveSaveInMemmory();
                                this.ClearState();
                                Global.GameState = new Dungeon12.Game.GameState();
                                Global.SceneManager.Destroy<MainScene>();
                                this.Switch<Start>();
                            }
                        }, this.ShowEffectsBinding);
                    }
                    else
                    {
                        this.Switch<SoloDuoScene>();
                        Global.GameState.Reset();
                    }
                },
                AbsolutePosition=true,
            });

            this.AddObject(new MetallButtonControl("Загрузить")
            {
                Left = 15.5f,
                Top = 11,
                AbsolutePosition = true,
                OnClick = () =>
                {
                    this.Switch<SaveLoadScene>(isGame ? "game" : default);

                    //this.PlayerAvatar = new Avatar(new Dungeon12.Noone.Noone()
                    //{
                    //    Origin = Dungeon12.Entities.Alive.Enums.Origins.Adventurer
                    //});
                    //this.PlayerAvatar.Character.Name = "Ваш персонаж";

                    //Global.AudioPlayer.Music("town", new Dungeon.Audio.AudioOptions()
                    //{
                    //    Repeat = true,
                    //    Volume = 0.3
                    //});

                    //this.Switch<Game.Main>();
                }
            });

            this.AddObject(new SmallMetallButtonControl(new DrawText("#") { Size = 40 }.Montserrat())
            {
                Left = 24,
                Top = 11,
                AbsolutePosition = true,
                OnClick = () =>
                {
                    this.Switch<EditorScene>();
                }
            });

            this.AddObject(new MetallButtonControl(isGame ? "Сохранить" : "Создатели")
            {
                Left = 15.5f,
                Top = 14,
                AbsolutePosition = true,
                OnClick = () =>
                {
                    if (isGame)
                    {
                        this.Switch<SaveLoadScene>("game", "saving");
                    }
                    else
                    {
                        Toast.Show("Вы знаете кто");
                    }
                }
            });

            this.AddObject(new MetallButtonControl(isGame? "Назад" : "Выход")
            {
                Left = 15.5f,
                Top = 17,
                AbsolutePosition = true,
                OnClick = () =>
                {
                    if (isGame)
                        this.Switch<MainScene>();
                    else
                        Global.Exit?.Invoke();
                }
            });
        }

        protected override void KeyPress(Key keyPressed, KeyModifiers keyModifiers, bool hold)
        {
            if (isGame && keyPressed == Key.Escape)
                this.Switch<MainScene>();
        }
    }
}