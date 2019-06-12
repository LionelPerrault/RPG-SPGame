﻿namespace Rogue.Map
{
    using Rogue.Map.Objects;
    using Rogue.Physics;
    using Rogue.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class GameMap
    {
        public Action<MapObject> PublishObject;

        public bool First = true;

        public string Name;

        public string _Name, _Affics;

        public ConsoleColor Biom;

        public int Level = 1;

        public GameMapObject Map = new GameMapObject();

        public HashSet<MapObject> Objects = new HashSet<MapObject>();

        public List<List<List<MapObject>>> MapOld = new List<List<List<MapObject>>>();
        
        public Action<MapObject, Direction,bool> OnMoving;

        public bool Move(MapObject @object, Direction direction)
        {
            var moveAvailable = true;

            var moveAreas = Map.Query(@object,true);
            if (moveAreas.Count > 0)
            {
                var mapObjs = moveAreas.ToArray()
                    .SelectMany(x => x.Nodes.ToArray())
                    .Where(node => node != @object)
                    .Where(node => @object.IntersectsWith(node))
                    .ToArray();

                moveAvailable = !mapObjs.Any(x => x.Obstruction);
            }

            if (moveAvailable)
            {
                var wasAreas = Map.QueryReference(@object);

                bool eq = wasAreas.SequenceEqual(moveAreas);
                
                if (!eq)
                {
                    Map.Remove(@object);
                    Map.Add(@object);
                }
            }

            OnMoving(@object,direction, moveAvailable);

            return moveAvailable;
        }
        
        public IEnumerable<Mob> Enemies(MapObject @object)
        {
            IEnumerable<Mob> mobs = Enumerable.Empty<Mob>();

            var moveArea = Map.Query(@object);
            if (moveArea != null)
            {
                mobs = moveArea.Nodes.Where(node => node is Mob).Select(node => node as Mob)
                    .Where(node => @object.IntersectsWith(node))
                    .ToArray();
            }

            return mobs.ToArray();
        }

        public IEnumerable<Сonversational> Conversations(MapObject @object)
        {
            MapObject rangeObject = PlayerRangeObject(@object);

            IEnumerable<Сonversational> npcs = Enumerable.Empty<Сonversational>();

            var moveArea = Map.Query(rangeObject);
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
                Position = new Physics.PhysicalPosition
                {
                    X = @object.Position.X - ((@object.Size.Width * 2.5) / 2),
                    Y = @object.Position.Y - ((@object.Size.Height * 2.5) / 2)
                },
                Size = @object.Size
            };

            rangeObject.Size.Height *= 2.5;
            rangeObject.Size.Width *= 2.5;
            return rangeObject;
        }

        public IEnumerable<MapObject> Interactions(MapObject @object)
        {
            var rangeObject = PlayerRangeObject(@object);

            IEnumerable<MapObject> interactable = Enumerable.Empty<MapObject>();

            var moveArea = Map.Query(rangeObject);
            if (moveArea != null)
            {
                interactable = moveArea.Nodes.Where(node => rangeObject.IntersectsWith(node))
                    .Where(node => node.Interactable)
                    .ToArray();
            }

            return interactable;
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
        public GameMapObject()
        {
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
    }

    public class GameMapContainerObject : MapObject
    {
        protected override bool Containable => true;
    }

}
