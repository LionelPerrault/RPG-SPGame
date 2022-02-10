﻿namespace Dungeon.Drawing.SceneObjects
{
    using Dungeon.SceneObjects;
    using System;
    using System.Reflection;

    /// <summary>
    /// По умолчанию не реагирует на события
    /// </summary>
    public class ImageObject : EmptySceneObject
    {
        public override bool Events => false;

        public ImageObject(string imagePath)
        {
            if (!imagePath.Contains(".Resources.Images."))
            {
                imagePath = Assembly.GetCallingAssembly().GetName().Name + ".Resources.Images." + imagePath.Embedded();
            }

            Image = imagePath;
        }

        public ImageObject(Func<string> imagePath)
        {
            imageDelegate = imagePath;
        }

        private Func<string> imageDelegate;

        public override string Image
        {
            get
            {
                if (imageDelegate != null)
                    return imageDelegate();
                return base.Image;
            }
            set => base.Image = value;
        }

        protected override void CallOnEvent(dynamic obj)
        {
            OnEvent(obj);
        }
    }
}
