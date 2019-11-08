﻿using Dungeon;
using Dungeon.Abilities;
using Dungeon.Classes;
using Dungeon.Conversations;
using Dungeon.Drawing;
using Dungeon.Drawing.SceneObjects.Map;
using Dungeon.Events;
using Dungeon.Map;
using Dungeon.View.Interfaces;
using System.Linq;

namespace Dungeon12
{
    public class ClassChangeTrigger : IConversationTrigger
    {
        public object PlayerSceneObject { get; set; }

        public object Gamemap { get; set; }

        public IDrawText Execute(string[] args)
        {
            var SceneObject = this.PlayerSceneObject.As<PlayerSceneObject>();

            Character from = SceneObject.Avatar.Character;

            var newClass = args[0];
            var newClassAssembly = args[1];

            // создаём новый экземпляр класса
            var to = newClass.GetInstanceFromAssembly<Character>(newClassAssembly);

            //отключаем все пассивные способности
            from.PropertiesOfType<Ability>()
                .Where(a => a.CastType == Dungeon.Abilities.Enums.AbilityCastType.Passive)
                .ToList()
                .ForEach(a => a.Release(Gamemap.As<GameMap>(), SceneObject.Avatar));

            // убираем все перки которые имеют отношение к классу
            from.RemoveAll(p => p.ClassDependent);

            to.Backpack = from.Backpack;
            to.Clothes = from.Clothes;
            to.EXP = from.EXP;
            to.Gold = from.Gold;
            to.HitPoints = from.HitPoints;
            to.MaxHitPoints = from.MaxHitPoints;
            to.AbilityPower = from.AbilityPower;
            to.AttackPower = from.AttackPower;
            to.Barrier = from.Barrier;
            to.Defence = from.Defence;
            to.Idle = from.Idle;
            to.MinDMG = from.MinDMG;
            to.MaxDMG = from.MaxDMG;
            to.Origin = from.Origin;

            to.Race = from.Race;
            to.Name = from.Name;
            to.Level = from.Level;

            to.Recalculate();

            SceneObject.Avatar.Character = to;
            SceneObject.Avatar.ReEntity(to);

            Global.Events.Raise(new ClassChangeEvent()
            {
                PlayerSceneObject = SceneObject,
                GameMap = Gamemap.As<GameMap>(),
                Character = SceneObject.Avatar.Character
            });

            return new DrawText("Класс поменяли");
        }
    }
}