﻿using Dungeon.Control.Commands;
using Dungeon.Control.Keys;
using Dungeon12.Map;
using Dungeon12.Map.Objects;
using Dungeon.Types;
using System.Collections.Generic;

namespace Dungeon12.Scenes.Game
{
    public class MoveCommand : Command
    {
        public GameMap Location { get; set; }

        public Avatar Player { get; set; }

        public Point PlayerPosition { get; set; }

        public override IEnumerable<Key> Keys => new Key[]
            {
                Key.W,
                Key.S,
                Key.A,
                Key.D,
                Key.Up,
                Key.Down,
                Key.Left,
                Key.Right
            };

        public override string Name => "Move";

        public override bool UI => false;

        public override void Run(Key keyPressed)
        {
            //var newPos = new Point()
            //{
            //    X = PlayerPosition.X,
            //    Y = PlayerPosition.Y
            //};

            //AnimationMap animationMap = default;
            //Direction direction = Direction.Down;

            //if (keyPressed == Key.A || keyPressed == Key.Left)
            //{
            //    newPos.X -= 1;
            //    animationMap = Player.MoveLeft;
            //    direction = Direction.Left;
            //}
            //if (keyPressed == Key.D || keyPressed == Key.Right)
            //{
            //    newPos.X += 1;
            //    animationMap = Player.MoveRight;
            //    direction = Direction.Right;
            //}
            //if (keyPressed == Key.W || keyPressed == Key.Up)
            //{
            //    newPos.Y -= 1;
            //    animationMap = Player.MoveUp;
            //    direction = Direction.Up;
            //}
            //if (keyPressed == Key.S || keyPressed == Key.Down)
            //{
            //    newPos.Y += 1;
            //    animationMap = Player.MoveDown;
            //    direction = Direction.Down;
            //}

            //newPos = this.Location.MoveObject(PlayerPosition, 1, newPos);

            //if (newPos != PlayerPosition)
            //{
            //    Drawing.Draw.Animation<LabirinthAnimationSession>(x =>
            //    {
            //        x.BlockingAnimation = true;
            //        x.BasePosition = PlayerPosition;
            //        x.NextPosition = newPos;
            //        x.Location = this.Location;
            //        x.ObjectAnimationMap = animationMap;
            //        x.Direction = direction;
            //    });

            //    PlayerPosition = newPos;
            //}
        }
    }
}
