﻿using Dungeon.View.Interfaces;
using Dungeon12.SceneObjects.Base;

namespace Dungeon12
{
    static internal class NineSliceBorderExtensions
    {
        /// <summary>
        /// border is 5px
        /// </summary>
        /// <param name="sceneObject"></param>
        /// <param name="opacity"></param>
        public static Border AddBorderBack(this ISceneObject sceneObject, double opacity=.95)
        {
            var b = new Border(sceneObject.Width, sceneObject.Height, opacity);
            sceneObject.AddChild(b);
            return b;
        }

        /// <summary>
        /// border is 5px
        /// </summary>
        /// <param name="sceneObject"></param>
        /// <param name="opacity"></param>
        public static void AddBorder(this ISceneObject sceneObject)
        {
            sceneObject.AddChild(new Border(sceneObject.Width, sceneObject.Height, -1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneObject"></param>
        /// <param name="opacity"></param>
        public static void AddBorder(this ISceneObject sceneObject, NineSliceSettings settings)
        {
            settings.BindDefaults(sceneObject);

            sceneObject.AddChild(new Border(settings));
        }
    }
}
