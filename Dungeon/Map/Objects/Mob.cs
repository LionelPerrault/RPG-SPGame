﻿namespace Dungeon.Map.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dungeon.Drawing.SceneObjects.Map;
    using Dungeon.Entities.Alive;
    using Dungeon.Entities.Enemy;
    using Dungeon.Game;
    using Dungeon.Loot;
    using Dungeon.Map.Infrastructure;
    using Dungeon.Physics;
    using Dungeon.Settings;
    using Dungeon.Types;
    using Dungeon.View.Interfaces;
    using Force.DeepCloner;
    using static Dungeon.Map.GameMap;

    [Template("*")]
    public class Mob : EntityMapObject<Enemy>
    {
        public Mob(Enemy component):base(component)
        {
            this.Destroy += Dying;
        }

        private void Dying()
        {
            List<MapObject> publishObjects = new List<MapObject>();

            var loot = LootGenerator.Generate();

            if (loot.Gold > 0)
            {
                var money = new Money() { Amount = loot.Gold };
                money.Location = Gamemap.RandomizeLocation(this.Location.DeepClone());
                money.Destroy += () => Gamemap.MapObject.Remove(money);
                Gamemap.MapObject.Add(money);

                publishObjects.Add(money);
            }

            foreach (var item in loot.Items)
            {
                var lootItem = new Loot()
                {
                    Item = item
                };

                lootItem.Location = Gamemap.RandomizeLocation(Location.DeepClone());
                lootItem.Destroy += () => Gamemap.MapObject.Remove(lootItem);

                publishObjects.Add(lootItem);
            }

            Gamemap.MapObject.Remove(this);

            publishObjects.ForEach(Gamemap.PublishObject);
        }

        public override string Icon { get => "*"; set { } }

        protected override MapObject Self => this;
        
        public override bool Obstruction => true;

        public bool IsChasing { get; set; }

        public override double MovementSpeed => base.MovementSpeed;

        public bool Moving { get; set; }

        public override PhysicalSize Size
        {
            get => new PhysicalSize
            {
                Height = 24,
                Width = 24
            };
            set { }
        }

        public override PhysicalPosition Position
        {
            get => new PhysicalPosition
            {
                X = base.Position.X + 4,
                Y = base.Position.Y + 4
            };
            set { }
        }

        public Point AttackRangeMultiples { get; set; }

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
        
        //[FlowMethod]
        //public void Damage(bool forward)
        //{
        //    if (forward)
        //    {
        //        Global.AudioPlayer.Effect(DamageSound ?? "bat");
        //    }
        //    else
        //    {
        //        bool enemyDied = GetFlowProperty<bool>("EnemyDied");
        //        if (enemyDied)
        //        {
        //            Die?.Invoke();
        //            //death sound
        //            //Global.AudioPlayer.Effect(DamageSound ?? "bat");
        //        }
        //    }
        //}

        public override ISceneObject Visual(GameState gameState)
        {
            return new EnemySceneObject(gameState.Player, gameState.Map, this, this.TileSetRegion);
        }

        public long Exp => 5;
    }
}