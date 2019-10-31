﻿using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using MoreLinq;
using Dungeon.Settings;
using Dungeon.Types;
using Dungeon.View.Interfaces;

namespace Dungeon.Drawing.Impl
{
    public class DrawText : IDrawText
    {
        /// <summary>
        /// ЭТО БЛЯДЬ ЛЕНТА ЁБАНЫЙ ТЫ ДУРАК
        /// </summary>
        private readonly List<IDrawText> InnerText = new List<IDrawText>();

        public DrawText(string value)
        {
            stringData = value;
        }

        public DrawText(string value, DrawColor foregroundColor) : this(value)
        {
            this.ForegroundColor = foregroundColor;
        }

        public DrawText(string value, DrawColor foregroundColor, float x = 0, float y = 0) : this(value)
        {
            this.ForegroundColor = foregroundColor;
            this.Region = new Rectangle
            {
                X = x,
                Y = y
            };
        }

        public DrawText(string value, DrawColor foregroundColor, double x = 0, double y = 0) : this(value)
        {
            this.ForegroundColor = foregroundColor;
            this.Region = new Rectangle
            {
                X = x,
                Y = y
            };
        }

        public DrawText(string value, IDrawColor foregroundColor, IDrawColor backgroundColor = null) : this(value)
        {
            this.BackgroundColor = backgroundColor;
            this.ForegroundColor = foregroundColor;
        }

        public void SetText(string value)
        {
            this.stringData = value;
        }

        private string stringData;
        public string StringData
        {
            get
            {
                if (this.InnerText.Count == 0)
                    return this.stringData;
                else
                    return this.stringData + string.Join("", this.InnerText.Select(x => x.StringData));
            }
        }

        public IEnumerable<IDrawText> Data
        {
            get
            {
                if (this.InnerText.Count == 0)
                {
                    return new IDrawText[] { this };
                }

                var current = this.DeepClone();
                current.InnerText.Clear();

                return new IDrawText[] { current }.Concat(this.InnerText);
            }
        }

        public int Length
        {
            get
            {
                var flatLength = this.Flat().Sum(x => x.StringData.Length);

                return flatLength + (this.stringData?.Length ?? 0);
            }
        }

        private IDrawColor foregroundColor;

        public IDrawColor BackgroundColor { get; set; }
        public IDrawColor ForegroundColor { get => foregroundColor ?? new DrawColor(ConsoleColor.White); set => this.foregroundColor = value; }

        public IDrawText This => this;

        public IEnumerable<IDrawText> Nodes => this.InnerText;

        public bool IsEmptyInside => this.InnerText.Count == 0;

        public Rectangle Region { get; set; }

        public virtual float Size { get; set; } = 12f;

        public virtual float LetterSpacing { get; set; } = 11.5625f;

        private double _opacity = 0;
        private bool opacitySetted = false;
        public double Opacity
        {
            get => opacitySetted ? _opacity : 1;
            set
            {
                opacitySetted = true;
                _opacity = value;
            }
        }

        public string FontName { get; set; }
        public bool Bold { get; set; }
        public string FontPath { get; set; }
        public string FontAssembly { get; set; }
        public bool CenterAlign { get; set; }

        public void Append(IDrawText drawText) => Append(drawText, true);

        public void Append(IDrawText drawText, bool inherit = true)
        {
            if (inherit)
            {
                drawText.FontAssembly = this.FontAssembly;
                drawText.FontName = this.FontName;
                drawText.FontPath = this.FontPath;
                drawText.Size = this.Size;
            }

            this.InnerText.Add(drawText);
        }

        public void AppendNewLine(bool inherit=true)
        {
            var txt = new DrawText(Environment.NewLine);
            if(inherit)
            {
                txt.FontAssembly = this.FontAssembly;
                txt.FontName = this.FontName;
                txt.FontPath = this.FontPath;
                txt.Size = this.Size;
            }

            this.InnerText.Add(txt);
        }

        /// <summary>
        /// write line с установкой картеки (по сути)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="drawText"></param>
        public void ReplaceAt(int index, IDrawText drawText)
        {
            var drawingRange = new DrawTextPosition()
            {
                StartIndex = index,
                Text = drawText
            };

            // если мы заменяем что-то, значит пути назад нет, 
            // затираем простое значение, и добавляем внутрь
            // часть себя что бы превратить в составное
            if (!string.IsNullOrEmpty(this.stringData))
            {
                this.InnerText.Add(new DrawText(this.stringData, this.ForegroundColor, this.BackgroundColor) { Size=Size,LetterSpacing=LetterSpacing});
                this.stringData = null;
            }

            //получаем все отрезки которые затрагивает новый
            var existed = ExistedElements(index, drawText);

            var first = existed.First();

            //если мы вставляем такую же длину как была в то же место - просто заменяем
            if (first.StartIndex == index && first.Text.Length == drawText.Length)
            {
                var inLineIndex = this.InnerText.IndexOf(first.Text);
                this.InnerText.RemoveAt(inLineIndex);
                this.InnerText.Insert(inLineIndex, drawText);
                return;
            }

            //проверяем надо ли отрезать слева
            DrawText newLeft = null;
            DrawText newRight = null;
            if (first.StartIndex != index)
            {
                newLeft = new DrawText(first.Text.StringData.Substring(0, index-first.StartIndex), first.Text.ForegroundColor, first.Text.BackgroundColor) { Size = Size, LetterSpacing = LetterSpacing };
            }

            //проверяем надо ли отрезать справа
            if (first.EndIndex > (index + drawText.Length))
            {

                var cuttingFrom = first.EndIndex - drawingRange.EndIndex;

                //элемент заканчивается дальше чем отрезок, надо отрезать правую часть
                //отрезаем от конца (нового) вставляемого элемента до конца строки
                newRight = new DrawText(first.Text.StringData.Substring(first.Text.Length - cuttingFrom, cuttingFrom), first.Text.ForegroundColor, first.Text.BackgroundColor) { Size = Size, LetterSpacing = LetterSpacing };

                var indexInListOriginalElement = this.InnerText.IndexOf(first.Text);
                this.InnerText.Remove(first.Text);

                var offset = 0;

                if (newLeft != null && newLeft.Length > 0)
                {
                    this.InnerText.Insert(indexInListOriginalElement, newLeft);
                    offset += 1;
                }
                this.InnerText.Insert(indexInListOriginalElement + (offset), drawText);
                this.InnerText.Insert(indexInListOriginalElement + (++offset), newRight);

                // если мы отрезали справа, значит дальше нас не интересуют элементы, 
                // хотя они могли попасть из-за того что при проверке на существующий
                // мы обязаны смежные элементы включить в коллекцию (хуйзнает зачем)
                return;
            }

            //итак, стадия пиздеца когда у нас возможно есть кусок слева, и ещё хуева тонна претендентов на правую часть, или замену

            foreach (var item in existed)
            {
                // проверяем можно ли полностью поглотить кусок
                if (item.EndIndex < drawingRange.EndIndex)
                {
                    //можно: нахуй его из внутренней коллекции, мы знаем индекс первого элемента, просто уёбем его и ничего не потеряем
                    this.InnerText.Remove(item.Text);
                }
                else if (item.EndIndex > drawingRange.EndIndex)
                {
                    //нельзся: ну, заебись, мы нашли конец, теперь надо проверить на существование конца при обрезке

                    var cuttingIndex = item.EndIndex - drawingRange.EndIndex;

                    newRight = new DrawText(first.Text.StringData.Substring(item.Text.Length - 1, cuttingIndex), first.Text.ForegroundColor, first.Text.BackgroundColor) { Size = Size, LetterSpacing = LetterSpacing };

                    var offset = 0;

                    var indexInListOriginalElement = this.InnerText.IndexOf(first.Text);
                    this.InnerText.Remove(first.Text);

                    if (newLeft != null && newLeft.Length > 0)
                    {
                        this.InnerText.Insert(indexInListOriginalElement, newLeft);
                        offset += 1;
                    }
                    this.InnerText.Insert(indexInListOriginalElement + (offset), drawText);
                    this.InnerText.Insert(indexInListOriginalElement + (++offset), newRight);

                    break;
                }
                else if (item.EndIndex==drawingRange.EndIndex)
                {                    
                    var offset = 0;

                    var indexInListOriginalElement = this.InnerText.IndexOf(first.Text);
                    this.InnerText.Remove(first.Text);

                    if (newLeft != null && newLeft.Length > 0)
                    {
                        this.InnerText.Insert(indexInListOriginalElement, newLeft);
                        offset += 1;
                    }
                    this.InnerText.Insert(indexInListOriginalElement + (offset), drawText);
                }
            }

            // что будем делать:
            // проверять можно ли полностью поглотить кусок
            //          если 
            //               можно: нахуй его из внутренней коллекции, мы знаем индекс первого элемента, просто уёбем его и ничего не потеряем
            //               нельзся: ну, заебись, мы нашли конец, теперь надо проверить на существование конца при обрезке
            //
            // ну всё, мы получили все начальные части, и конечные части, и убрали ненужные
            // теперь так же по условию существования начала и конца запушим всё в ленту
            //
            // и пиздец.
        }

        private class DrawTextPosition
        {
            public int StartIndex { get; set; }

            public IDrawText Text { get; set; }

            public int EndIndex { get => StartIndex + Text.StringData.Length; }
        }

        private bool CanCutRight(DrawTextPosition cutting, DrawTextPosition insertion)
        {
            //элемент заканчивается дальше чем отрезок, надо отрезать правую часть
            //отрезаем от конца (нового) вставляемого элемента до конца строки
            var newRight = new DrawText(cutting.Text.StringData.Substring((insertion.StartIndex + insertion.Text.Length)), cutting.Text.ForegroundColor, cutting.Text.BackgroundColor);

            var indexInListOriginalElement = this.InnerText.IndexOf(cutting.Text);
            this.InnerText.Remove(cutting.Text);

            var offset = 0;

            DrawText newLeft = null;
            if (newLeft != null)
            {
                this.InnerText.Insert(indexInListOriginalElement, newLeft);
                offset += 1;
            }
            this.InnerText.Insert(indexInListOriginalElement + (offset), insertion.Text);
            this.InnerText.Insert(indexInListOriginalElement + (offset * 2), newRight);

            // если мы отрезали справа, значит дальше нас не интересуют элементы, 
            // хотя они могли попасть из-за того что при проверке на существующий
            // мы обязаны смежные элементы включить в коллекцию (хуйзнает зачем)
            return true;
        }


        private IEnumerable<DrawTextPosition> ExistedElements(int index, IDrawText inserted)
        {
            List<DrawTextPosition> elements = new List<DrawTextPosition>();

            var futureElement = new DrawTextPosition
            {
                StartIndex = index,
                Text = inserted
            };

            var carry = 0;
            bool startFinded = false;
            foreach (var item in this.InnerText)
            {
                carry += item.Length;

                if (!startFinded && carry > index)
                {
                    startFinded = true;
                    elements.Add(new DrawTextPosition
                    {
                        StartIndex = carry - item.Length,
                        Text = item
                    });
                }

                if (startFinded && carry <= inserted.Length)
                {
                    elements.Add(new DrawTextPosition
                    {
                        StartIndex = carry - item.Length,
                        Text = item
                    });
                }
            }

            return elements.DistinctBy(x => x.Text).ToList();
        }

        private (IDrawText segment, int positionInLine) ExistedSegment(int index)
        {
            int currentCharInLine = 0;

            foreach (var item in this.InnerText)
            {
                currentCharInLine += item.StringData.Length;

                if (currentCharInLine > index)
                    return (item, currentCharInLine);


                ////опять же, костыль девелопмент
                //if (currentCharInLine >= index)
                //    return (item, currentCharInLine);

            }

            throw new Exception("Индекс такой вроде как есть, а вот найти его не получается.");
        }

        private bool IndexExists(int index)
        {
            var line = this.InnerText.Sum(x => x.StringData.Length);
            return line >= index;
        }

        private int LastIndex()
        {
            return this.InnerText.Sum(x => x.StringData.Length);
        }

        private void FillBeforeIndex(int index)
        {
            var last = this.LastIndex();
            var line = new string(Enumerable.Range(0, index - last).Select(x => ' ').ToArray());
            this.InnerText.Add(new DrawText(line));
        }

        public void Prepend(IDrawText drawText)
        {
            throw new NotImplementedException();
        }

        public static implicit operator DrawText(string value) => new DrawText(value);

        /// <summary>
        /// from container
        /// </summary>
        private static DrawingSize DrawingSize = null;

        /// <summary>
        /// Creates new drawing text with ' ' chars of full length
        /// </summary>
        /// <param name="length">if not set, then - <see cref="Dungeon.Settings.DrawingSize.WindowChars"/> </param>
        /// <returns></returns>
        public static DrawText Empty(int length = 0, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            if (length == 0)
                length = DrawingSize.WindowChars;

            return new DrawText(new string(Enumerable.Range(0, length).Select(x => ' ').ToArray()), foregroundColor);

        }

        public void Paint(IDrawColor drawColor, bool recursive=false)
        {
            this.ForegroundColor = drawColor;
            if (recursive)
            {
                ColorText(this, drawColor);
            }
        }

        private void ColorText(IDrawText text, IDrawColor color)
        {
            text.ForegroundColor = color;
            if (!text.IsEmptyInside)
            {
                foreach (var innerText in text.Data)
                {
                    ColorText(innerText, color);
                }
            }
        }

        public override string ToString()
        {
            return this.StringData;
        }

    }
}