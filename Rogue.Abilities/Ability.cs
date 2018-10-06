﻿namespace Rogue.Abilities
{
    using Rogue.Abilities.Scaling;
    using Rogue.Abilities.Talants;
    using Rogue.Entites.Alive.Character;
    using Rogue.View.Interfaces;

    /// <summary>
    /// тут кто-то явно сэкономил на времени и въебал виртуальные свойства вместо абстрактных
    /// </summary>
    public abstract class Ability
    {
        /// <summary>
        /// Позиция способности от 1 до 4
        /// </summary>
        public abstract int Position { get; }

        /// <summary>
        /// вот блядь виртуал почти всегда лень ебаная
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// Наименование навыка
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Скалирование навыка от характеристики
        /// </summary>
        public abstract ScaleRate Scale { get; }

        public virtual string Icon { get; set; }

        public virtual IDrawColor BackgroundColor { get; set; }

        public virtual IDrawColor ForegroundColor { get; set; }

        /// <summary>
        /// Коэффицент полезности навыка
        /// </summary>
        public int COE => 0;
    }


    /// <summary>
    /// Навык принадлежащий классу
    /// </summary>
    /// <typeparam name="TClass">Класс которому принадлежит способность</typeparam>
    /// <typeparam name="TTalants">Тип талантов этой способности</typeparam>
    public abstract class Ability<TClass, TTalants> : Ability
        where TClass: Player
        where TTalants : TalantTree
    {
        /// <summary>
        /// Скастовать способность
        /// </summary>
        /// <param name="class"></param>
        /// <param name="talants"></param>
        public void Cast(TClass @class, TTalants talants)
        {
            if (CastAvailable(@class, talants))
                InternalCast(@class, talants);
        }

        protected abstract void InternalCast(TClass @class, TTalants talants);

        /// <summary>
        /// Проверить может ли способность выполнена
        /// </summary>
        /// <param name="class"></param>
        /// <param name="talants"></param>
        /// <returns></returns>
        public abstract bool CastAvailable(TClass @class, TTalants talants);
    }
}