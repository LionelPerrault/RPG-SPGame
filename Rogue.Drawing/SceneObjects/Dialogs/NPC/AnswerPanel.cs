﻿namespace Rogue.Drawing.SceneObjects.Dialogs.NPC
{
    using Rogue.Control.Keys;
    using Rogue.Control.Pointer;
    using Rogue.Conversations;
    using Rogue.Drawing.Impl;
    using Rogue.Drawing.SceneObjects.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AnswerPanel : DarkRectangle
    {
        public override bool CacheAvailable => false;

        public override bool AbsolutePosition => true;
        
        public AnswerPanel()
        {
            this.Opacity = 0.8;

            this.Top = 15;
            this.Left = 0;

            this.Height = 7.5;
            this.Width = 31;
        }

        private double space;

        private List<AnswerClickable> currentAnswers = new List<AnswerClickable>();

        private Subject lastSubject;

        public void Select(Subject subject) => SelectSubject(subject);

        private void SelectSubject(Subject subject, string prevText = null)
        {
            lastSubject = subject;
            currentAnswers.ForEach(a =>
            {
                this.RemoveChild(a);
                a.Destroy?.Invoke();
            });

            var text = new DrawText(prevText ?? subject.Text).Montserrat();
            space = this.MeasureText(text).Y / 32;
            this.Text = text;

            var y = space + 1;

            var shownReplics = subject.Replics.Where(x => x.Shown).ToArray();

            for (int i = 0; i < shownReplics.Length; i++)
            {
                var replica = shownReplics[i];
                var answer = new AnswerClickable(i + 1, replica, this.Select)
                {
                    Left = 1,
                    Top = y
                };

                this.AddChild(answer);

                currentAnswers.Add(answer);
                y++;
            }
        }

        public void Select(Replica replica)
        {
            if (replica.Replics.Count == 0)
            {
                SelectSubject(lastSubject,replica.Text);
                return;
            }

            currentAnswers.ForEach(a =>
            {
                this.RemoveChild(a);
                a.Destroy?.Invoke();
            });

            var text = new DrawText(replica.Text).Montserrat();
            space = this.MeasureText(text).Y / 32;
            this.Text = text;

            var y = space + 1;

            var shownReplics = replica.Replics.ToArray();

            for (int i = 0; i < shownReplics.Length; i++)
            {
                var nextReplica = shownReplics[i];
                var answer = new AnswerClickable(i + 1, nextReplica, this.Select)
                {
                    Left = 1,
                    Top = y
                };

                this.AddChild(answer);

                currentAnswers.Add(answer);
                y++;
            }
        }

        private class AnswerClickable : HandleSceneControl
        {
            public override bool AbsolutePosition => true;

            public override bool CacheAvailable => false;

            private Action<Replica> select;
            private Replica repl;

            private int count;

            public AnswerClickable(int count, Replica replica, Action<Replica> select)
            {
                this.count = count;
                this.select = select;
                var txt = new DrawText($"{count}. {replica.Answer}").Montserrat();

                this.Text = txt;

                this.repl = replica;

                this.Width = MeasureText(txt).X / 32;
                this.Height = 1;

                Key key = Key.None;

                switch (count)
                {
                    case 1: key = Key.D1; break;
                    case 2: key = Key.D2; break;
                    case 3: key = Key.D3; break;
                    case 4: key = Key.D4; break;
                    case 5: key = Key.D5; break;
                    case 6: key = Key.D6; break;
                    case 7: key = Key.D7; break;
                    case 8: key = Key.D8; break;
                    case 9: key = Key.D9; break;
                    default:
                        break;
                }

                this.KeyHandles = new Key[]
                {
                    key
                };
            }

            public override void Focus()
            {
                this.Text.ForegroundColor = new DrawColor(System.ConsoleColor.Yellow);
                base.Focus();
            }

            public override void Unfocus()
            {
                this.Text.ForegroundColor = new DrawColor(System.ConsoleColor.White);
                base.Unfocus();
            }

            public override void Click(PointerArgs args)
            {
                select?.Invoke(this.repl);
            }

            public override void KeyDown(Key key, KeyModifiers modifier, bool hold)
            {
                if (!hold)
                {
                    //почему блядь дважды?
                    Console.WriteLine("selected");
                }
            }
        }
    }
}