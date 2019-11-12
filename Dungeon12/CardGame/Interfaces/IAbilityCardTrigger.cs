﻿using Dungeon12.CardGame.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dungeon12.CardGame.Interfaces
{
    public interface IAbilityCardTrigger
    {
        void Activate(Card card, CardGamePlayer enemy, CardGamePlayer player, AreaCard areaCard);
    }
}