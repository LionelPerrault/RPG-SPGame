﻿namespace Dungeon.Data.Fractions
{
    public class FractionData : Persist
    {
        public string Name { get; set; }

        /// <summary>
        /// Признак репутации которая может быть использована игроком
        /// </summary>
        public bool Playable { get; set; }

        public string[] Enemies { get; set; }
    }
}