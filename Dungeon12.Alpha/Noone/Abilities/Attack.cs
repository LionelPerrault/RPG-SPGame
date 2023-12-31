﻿namespace Dungeon12.Noone.Abilities
{
    using Dungeon;
    using Dungeon12.Abilities;
    using Dungeon12.Abilities.Enums;
    using Dungeon12.Abilities.Scaling;
    using Dungeon12.Entities.Alive;
    using Dungeon12.Map;
    using Dungeon12.Map.Objects;
    using Dungeon12.Noone.Talants;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class Attack : Ability<Noone,AbsorbingTalants>
    {
        public const string AttackCooldown = nameof(Noone) + nameof(Attack);

        public override int Position => 0;
        public override string Spend => "Использует: 2 очка действий";

        public override string Name => "Атака";

        public override AbilityTargetType TargetType => AbilityTargetType.TargetAndNonTarget;

        public override Cooldown Cooldown { get; } = new Cooldown(500, AttackCooldown);

        public override ScaleRate<Noone> Scale => new ScaleRate<Noone>(x => x.AttackDamage * 1.1);

        public override AbilityPosition AbilityPosition => AbilityPosition.Left;

        protected override bool CanUse(Noone @class)=> @class.Actions > 0;

        protected override double RangeMultipler => 2.5;

        public NPCMap AttackedEnemy { get; set; }

        protected override void Use(GameMap gameMap, Avatar avatar, Noone @class)
        {
            @class.InParry = true;
            Global.AudioPlayer.Effect("attack3.wav".AsmSoundRes());

            var rangeObject = avatar.Grow(2.5);

            var enemy = gameMap.Enemies(rangeObject).FirstOrDefault();

            if (enemy != null)
            {
                @class.Actions -= 2;
                var value = this.ScaledValue(@class, Value);

                enemy.Entity.Damage(@class, new Dungeon12.Entities.Alive.Damage()
                {
                    Amount = value,
                    Type = DamageType.Physical
                });
                
                AttackedEnemy = enemy;
            }

            Global.Time.Timer("nooneparry")
                .After(500)
                .Do(() => Global.GameState.Character.As<Noone>().InParry = false)
                .Trigger();
        }

        protected override void Dispose(GameMap gameMap, Avatar avatar, Noone @class) { }

        public override long Value => 2;

        public override AbilityActionAttribute ActionType => AbilityActionAttribute.EffectInstant;

        public override AbilityCastType CastType => AbilityCastType.Active;

        public override Location CastLocation => Location.Combat;

        public override string Description => $"Атакует врага нанося двойной урон {Environment.NewLine}оружием в правой руке. {Environment.NewLine}Может наносить критический урон.";
    }
}