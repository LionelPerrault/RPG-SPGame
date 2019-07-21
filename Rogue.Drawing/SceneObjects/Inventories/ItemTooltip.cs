﻿namespace Rogue.Drawing.SceneObjects.Inventories
{
    using Rogue.Drawing.GUI;
    using Rogue.Drawing.Impl;
    using Rogue.Items;
    using Rogue.Items.Enums;
    using Rogue.Types;
    using System.Collections.Generic;
    using System.Linq;

    public class ItemTooltip : Tooltip
    {
        public override bool CacheAvailable => false;

        public override bool Interface => true;

        private readonly Item item;
        private double textPosTop = 0;

        public ItemTooltip(Item item, Point position) : base("", position, new DrawColor(System.ConsoleColor.Black))
        {
            this.item = item;
            Opacity = 0.8;

            this.Children.ForEach(c => c.Destroy?.Invoke());
            
            MeasureSize();
            this.Width = maxMeasure.X / 32;
            this.Height = textPosTop;
            textPosTop = 0; //мы измерили размер, но нам всё равно это нужно для того что бы определять где рисовать следующую надпись

            Stats.ForEach(AddLine);
        }

        private IEnumerable<DrawText> Stats
        {
            get
            {
                List<DrawText> stats = new List<DrawText>();

                stats.Add(Title);
                stats.Add(SubType);
                stats.AddRange(BaseStats);
                stats.AddRange(Additional);
                stats.AddRange(ClassStats);
                stats.AddRange(ItemSet);

                return stats;
            }
        }

        private DrawText Title
        {
            get
            {
                var title = new DrawText(item.Name, item.Rare.Color()) { Size = 24 }.Triforce();
                Measure(title);
                return title;
            }
        }

        private DrawText SubType => item.SubType == null
            ? new DrawText(item.Kind.ToDisplay(), new DrawColor(130, 99, 7)).Montserrat()
            : new DrawText(item.SubType, new DrawColor(130, 99, 7)).Montserrat();

        private IEnumerable<DrawText> BaseStats => item.BaseStats.Select(MapEquipment);

        private IEnumerable<DrawText> Additional => item.Additional.Select(MapEquipment);

        private IEnumerable<DrawText> ClassStats => item.ClassStats.Select(MapEquipment);

        private IEnumerable<DrawText> ItemSet
        {
            get
            {
                List<DrawText> itemSet = new List<DrawText>();

                if (item.ItemSetName != null)
                {
                    itemSet.Add(new DrawText(item.ItemSetName, new DrawColor(System.ConsoleColor.Green)));
                    itemSet.AddRange(item.ItemSet.Select(MapEquipment));
                }

                return itemSet;
            }
        }

        private DrawText MapEquipment(Equipment eq)
        {
            var drawText = new DrawText(eq.Title, eq.Color).Montserrat();
            this.Measure(drawText);
            return drawText;
        }

        private void AddLine(DrawText drawText)
        {
            if (drawText == null)
                return;

            var txt = this.AddTextCenter(drawText, vertical: false);
            txt.Top = textPosTop;
            textPosTop += MeasureText(txt.Text).Y / 32;
        }

        private void MeasureSize()
        {
            void MeasureLocal(DrawText drawText)
            {
                if (drawText == null)
                    return;

                textPosTop += MeasureText(drawText).Y / 32;
            }

            Stats.ForEach(MeasureLocal);
        }

        private Point maxMeasure = new Point();

        private void Measure(DrawText txt)
        {
            var measure = this.MeasureText(txt);

            if(measure.X > maxMeasure.X)
            {
                maxMeasure = measure;
            }
        }
    }
}