﻿using Dungeon.Abilities;
using Dungeon.Abilities.Enums;
using Dungeon.Abilities.Scaling;
using Dungeon12.Bowman.Effects;
using Dungeon.Map;
using Dungeon.Map.Objects;
using Dungeon.View.Interfaces;
using Dungeon;

namespace Dungeon12.Bowman.Abilities
{
    public class MightShot : BaseCooldownAbility<Bowman>
    {
        public override Cooldown Cooldown { get; } = BaseCooldown.Chain(3000, nameof(MightShot)).Build();

        public override AbilityPosition AbilityPosition => AbilityPosition.Right;

        public override AbilityActionAttribute ActionType => AbilityActionAttribute.DmgHealInstant;

        public override AbilityTargetType TargetType => AbilityTargetType.TargetAndNonTarget;

        public override string Name => "Сильный выстрел";

        public override ScaleRate Scale => ScaleRate.Build(Dungeon.Entites.Enums.Scale.AttackDamage);

        private Bowman rangeclass;
        protected override bool CanUse(Bowman @class)
        {
            rangeclass = @class;
            return @class.Energy.RightHand >= 15;
        }

        protected override double RangeMultipler => (4 + (rangeclass?.Range ?? 0)) * 2.5;

        protected override void Dispose(GameMap gameMap, Avatar avatar, Bowman @class)
        {
        }

        protected override void Use(GameMap gameMap, Avatar avatar, Bowman @class)
        {
            @class.Energy.RightHand -= 15;

            var baseSpeed = 0.045;
            var speed = baseSpeed;

            if (@class.AttackSpeed > 0)
            {
                speed += @class.AttackSpeed / 1000d;
            }
            var range = @class.Range / 15;

            var arrow = new ArrowObject(avatar.VisionDirection, 4 + range, 27, speed);

            this.UseEffects(new Arrow(@class,gameMap, arrow, avatar.VisionDirection, new Dungeon.Types.Point(avatar.Position.X / 32, avatar.Position.Y / 32),true).InList<ISceneObject>());
        }
    }
}
