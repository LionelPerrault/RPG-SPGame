﻿namespace Dungeon.Map
{
    using Dungeon.Events.Events;
    using Dungeon.GameObjects;
    using Dungeon.Map.Objects;
    using Dungeon.Physics;
    using Dungeon.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class GameMap : GameComponent
    {
        public GameMap()
        {
            MapObject = new GameMapObject(this);
        }

        public Action<MapObject> PublishObject;

        public bool First = true;

        public string Name;

        public string _Name, _Affics;

        public ConsoleColor Biom;

        public int Level = 1;

        public GameMapObject MapObject;

        public HashSet<MapObject> Objects = new HashSet<MapObject>();

        public List<List<List<MapObject>>> MapOld = new List<List<List<MapObject>>>();
        
        public Action<MapObject, Direction,bool> OnMoving;

        public bool Move(MapObject @object, Direction direction)
        {
            var moveAvailable = true;

            var moveAreas = MapObject.Query(@object,true);
            if (moveAreas.Count > 0)
            {
                foreach (var moveArea in moveAreas)
                {
                    foreach (var node in moveArea.Nodes)
                    {
                        if (node != @object && node.IntersectsWith(@object))
                        {
                            if (node.Obstruction)
                            {
                                moveAvailable = false;
                                goto moveDetected;
                            }
                        }
                    }
                }
            }

            moveDetected:

            if (moveAvailable)
            {
                var wasAreas = MapObject.QueryReference(@object);

                bool eq = wasAreas.SequenceEqual(moveAreas);
                
                if (!eq)
                {
                    MapObject.Remove(@object);
                    MapObject.Add(@object);
                }
            }

            OnMoving(@object,direction, moveAvailable);

            return moveAvailable;
        }
        
        public IEnumerable<Mob> Enemies(MapObject @object)
        {
            IEnumerable<Mob> mobs = Enumerable.Empty<Mob>();

            var moveArea = MapObject.Query(@object);
            if (moveArea != null)
            {
                mobs = moveArea.Nodes.Where(node => node is Mob).Select(node => node as Mob)
                    .Where(node => @object.IntersectsWith(node))
                    .ToArray();
            }

            return mobs.ToArray();
        }
        
        /// <summary>
        /// Получить информацию о том что объекты такого типа есть
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public bool Any<T>(MapObject @object)
            where T : PhysicalObject
        {
            var moveArea = MapObject.Query(@object);
            if (moveArea != null)
            {
                return moveArea.Nodes.Where(node => node is T)
                   .Select(node => node as T)
                   .Any(node => @object.IntersectsWith(node));
            }

            return false;
        }

        /// <summary>
        /// Возвращает всех найденных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public IEnumerable<T> All<T>(MapObject @object)
            where T : MapObject
        {
            IEnumerable<T> all = Enumerable.Empty<T>();

            var moveArea = MapObject.Query(@object);
            if (moveArea != null)
            {
                all = moveArea.Nodes.Where(node => node is T)
                    .Select(node => node as T)
                    .Where(node => @object.IntersectsWith(node))
                    .ToArray();
            }

            return all;
        }

        /// <summary>
        /// Возвращает первого найденого T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public T One<T>(MapObject @object)
            where T : PhysicalObject
        {
            var moveArea = MapObject.Query(@object);
            if (moveArea != null)
            {
                return moveArea.Nodes.FirstOrDefault(node =>
                {
                    if (node is T nodeT)
                    {
                        return nodeT.IntersectsWith(@object);
                    }
                    return false;
                }) as T;
            }

            return default;
        }

        public IEnumerable<Сonversational> Conversations(MapObject @object)
        {
            MapObject rangeObject = PlayerRangeObject(@object);

            IEnumerable<Сonversational> npcs = Enumerable.Empty<Сonversational>();

            var moveArea = MapObject.Query(rangeObject);
            if (moveArea != null)
            {
                npcs = moveArea.Nodes.Where(node => node is Сonversational)
                    .Select(node => node as Сonversational)
                    .Where(node => rangeObject.IntersectsWith(node))
                    .ToArray();
            }

            return npcs;
        }

        private static MapObject PlayerRangeObject(MapObject @object)
        {
            var rangeObject = new MapObject
            {
                Position = new Dungeon.Physics.PhysicalPosition
                {
                    X = @object.Position.X - ((@object.Size.Width * 2.5) / 2),
                    Y = @object.Position.Y - ((@object.Size.Height * 2.5) / 2)
                },
                Size = new PhysicalSize()
                {
                    Height = @object.Size.Height,
                    Width = @object.Size.Width
                }
            };

            rangeObject.Size.Height *= 2.5;
            rangeObject.Size.Width *= 2.5;
            return rangeObject;
        }

        private bool needReloadCache = false;

        public bool ReloadCache
        {
            get
            {
                if (needReloadCache)
                {
                    needReloadCache = false;
                    return true;
                }
                return false;
            }
        }
    }

    public class GameMapObject : MapObject
    {
        private GameMap _gameMap;

        public GameMapObject(GameMap gameMap)
        {
            _gameMap = gameMap;
            Position = new PhysicalPosition
            {
                X = 0,
                Y = 0
            };

            Size = new PhysicalSize
            {
                Height = 3200,
                Width = 3200
            };
            
            Nodes = new List<MapObject>();


            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Nodes.Add(new GameMapContainerObject()
                    {
                        Size = new PhysicalSize
                        {
                            Width = 1600,
                            Height = 1600
                        },
                        Position = new PhysicalPosition
                        {
                            X = i * 1600,
                            Y = j * 1600
                        }
                    });
                }
            }

            foreach (var item in this.Nodes)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        item.Nodes.Add(new GameMapContainerObject()
                        {
                            Size = new PhysicalSize
                            {
                                Width = 800,
                                Height = 800
                            },
                            Position = new PhysicalPosition
                            {
                                X = item.Position.X + i * 800,
                                Y = item.Position.Y + j * 800
                            }
                        });
                    }
                }
            }
        }

        protected override bool Containable => true;

        public override PhysicalPosition Position { get; set; }

        public override PhysicalSize Size { get; set; }

        protected override MapObject Self => this;

        public override void Add(MapObject physicalObject)
        {
            physicalObject.Gamemap = _gameMap;
            base.Add(physicalObject);
            if (physicalObject is Totem totem)
            {
                Global.Events.Raise(new TotemArrivedEvent(totem));
            }
        }
    }

    public class GameMapContainerObject : MapObject
    {
        protected override bool Containable => true;
    }

}
