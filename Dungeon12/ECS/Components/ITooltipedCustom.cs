﻿using Dungeon.View.Interfaces;
using Dungeon12.SceneObjects.Base;

namespace Dungeon12.ECS.Components
{
    internal interface ITooltipedCustom
    {
        bool ShowTooltip { get; }

        ISceneObject GetTooltip();
    }
}