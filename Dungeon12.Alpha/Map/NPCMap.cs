﻿namespace Dungeon12.Map.Objects
{
    using Dungeon;
    using Dungeon.Data.Attributes;
    using Dungeon.Physics;
    using Dungeon.Types;
    using Dungeon.View.Interfaces;
    using Dungeon12.Data.Npcs;
    using Dungeon12.Data.Region;
    using Dungeon12.Drawing.SceneObjects.Map;
    using Dungeon12.Entities;
    using Dungeon12.Entities.Fractions;
    using Dungeon12.Map;
    using Dungeon12.Map.Infrastructure;
    using Force.DeepCloner;
    using System;
    using System.Linq;

    [Template("Npc")]
    [DataClass(typeof(NPCData))]
    public class NPCMap : EntityMapObject<NPC>
    {
        public NPCMap() : this(default)
        {
        }

        public NPCMap(NPC entity) : base(entity)
        {
            this.Destroy += Dying;
        }

        public override bool Saveable => true;

        public override string Icon { get => "N"; set { } }

        protected override MapObject Self => this;

        public override bool Obstruction => true;

        public bool NoInteract { get; set; }

        public string NoInteractText { get; set; }

        public bool Moving { get; set; }

        public long Exp => 5;

        public bool IsChasing { get; set; }

        private PhysicalSize _size;

        public override PhysicalSize Size
        {
            get
            {
                if (_size == default)
                {
                    _size = new PhysicalSize
                    {
                        Height = 24,
                        Width = 24
                    };
                }
                return _size;
            }
                
            set { }
        }

        private PhysicalPosition _position;

        public override PhysicalPosition Position
        {
            get
            {
                if (_position == default)
                {
                    _position = new PhysicalPosition
                    {
                        X = base.Position.X + 4,
                        Y = base.Position.Y + 4
                    };
                }
                else
                {
                    _position.X = base.Position.X + 4;
                    _position.Y = base.Position.Y + 4;
                }
                return _position;
            }
            set { }
        }

        public override ISceneObject Visual()
        {
            var so = new NPCSceneObject(Global.GameState.Player, Global.GameState.Map, this, this.TileSetRegion);
            this.SceneObject = so;
            return so;
        }

        public string IdentifyName { get; set; }

        public Point AttackRangeMultiples { get; set; }

        public bool IsEnemy { get; set; }

        protected override void Load(RegionPart npcData)
        {
            this.IdentifyName = npcData.IdentifyName;

            var data = Dungeon.Store.Entity<NPCData>(x => x.IdentifyName == npcData.IdentifyName).FirstOrDefault();

            this.ReEntity(data.NPC.DeepClone());

            this.Entity.IdentifyName = data.IdentifyName;
            data.NPC.Level = data.Level;
            if (data.NPC.Level > 2)
            {
                if (Dungeon.Random.Chance(25))
                {
                    data.NPC.Level--;
                }
                if (Dungeon.Random.Chance(40))
                {
                    data.NPC.Level++;
                }
            }

            this.Entity.Level = data.NPC.Level;

            this.Entity.MaxHitPoints = data.NPC.HitPoints * data.NPC.Level;
            this.Entity.HitPoints = this.Entity.MaxHitPoints;

            if (data.VisionMultiples != default)
            {
                VisionMultiple = data.VisionMultiples;
            }
            if (data.AttackRangeMultiples != default)
            {
                AttackRangeMultiples = data.AttackRangeMultiples;
            }
            else
            {
                AttackRangeMultiples = VisionMultiple;
            }

            this.IsEnemy = data.IsEnemy;

            this.Moving = data.Moveable;
            this.Tileset = data.Tileset;
            this.FaceImage = data.Face;
            this.TileSetRegion = data.TileSetRegion;
            this.Name = data.Name;
            this.Size = new PhysicalSize()
            {
                Width = data.Size.X * 32,
                Height = data.Size.Y * 32
            };
            this.MovementSpeed = data.MovementSpeed;
            this.Location = npcData.Position;

            this.NoInteract = data.NoInteract;
            this.NoInteractText = data.NoInteractText;

            if (!IsEnemy)
            {
                this._size = new PhysicalSize() { Height = .1, Width = .1 };
                if (data.Merchant)
                {
                    this.Merchant = new Dungeon12.Merchants.Merchant();
                    this.Merchant.FillBackpacks();
                }
                this.BuildConversations(data);

                if (Entity.MoveRegion != null)
                {
                    Entity.MoveRegion = Entity.MoveRegion * 32;
                }
            }

            if (data.FractionIdentify != default)
            {
                Entity.Fraction = FractionView.Load(data.FractionIdentify).ToFraction();
            }
        }

        public override void Reload()
        {
            var data = Dungeon.Store.Entity<NPCData>(x => x.IdentifyName == IdentifyName).FirstOrDefault();
            this.BuildConversations(data);

            if (this.Merchant!=default)
            {
                this.Merchant.FillBackpacks();
            }

            this.ReEntity(this.Entity);
            this.Destroy += Dying;
        }

        private void Dying()
        {
            DropLoot(this.Entity.LootTable);

            if (!Gamemap.MapObject.Remove(this))
            {
                throw new System.Exception("Объект не удаляется!");
            }

            Gamemap.Objects.Remove(this);
        }

        public string DamageSound { get; set; }

        public MapObject AttackRange => new MapObject
        {
            Position = new PhysicalPosition
            {
                X = this.Position.X - ((this.Size.Width * this.AttackRangeMultiples.X) - this.Size.Width) / 2,
                Y = this.Position.Y - ((this.Size.Height * this.AttackRangeMultiples.Y) - this.Size.Height) / 2
            },
            Size = new PhysicalSize
            {
                Width = this.Size.Width * AttackRangeMultiples.X,
                Height = this.Size.Height * AttackRangeMultiples.Y
            }
        };
    }
}