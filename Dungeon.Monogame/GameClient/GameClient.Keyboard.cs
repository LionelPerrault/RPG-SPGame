﻿namespace Dungeon.Monogame
{
    using Dungeon.Control.Keys;
    using Dungeon.Scenes.Manager;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using MonoGame.Extended;
    using System.Collections.Generic;
    using System.IO;

    public partial class GameClient
    {
        private KeyboardState keyboardState;
        private Keys[] pressed;
        private HashSet<Keys> keysState = new HashSet<Keys>();
        private static HashSet<Keys> keysHolds = new HashSet<Keys>();

        private void UpdateKeyboardEvents(Microsoft.Xna.Framework.GameTime gameTime)
        {
            var elapsed = gameTimePrev - gameTime.TotalGameTime;
            if (elapsed.Seconds<1)
                screenCounter++;
            else
                screenCounter=0;

            keyboardState = Keyboard.GetState();

            var pressed = keyboardState.GetPressedKeys();

            //detect that keys in keyboard different, so, one of keys was pressed or released
            if (!keysState.SetEquals(pressed))
            {
                var currentHashset = new HashSet<Keys>(keysState);
                var pressedHashset = new HashSet<Keys>(pressed);

                keysState.ExceptWith(pressedHashset);
                foreach (var key in keysState)
                {
                    OnKeyUp(key, gameTime);
                }

                pressedHashset.ExceptWith(currentHashset);
                foreach (var key in pressedHashset)
                {
                    OnKeyDown(key,gameTime);
                }

                keysState = new HashSet<Keys>(pressed);
            }

            //detect that keyboard took new array of keys (it means, keyboard interacting)
            if (this.pressed != pressed)
            {
                var pressedHashset = new HashSet<Keys>(pressed);
                pressedHashset.IntersectWith(keysState);
                keysHolds = pressedHashset;
            }

            foreach (var keyHold in keysHolds)
            {
                OnKeyDown(keyHold, gameTime);
            }

            this.pressed = pressed;
        }

        private void OnTextInput(object sender, TextInputEventArgs e)
        {
            SceneManager.Current?.OnText(e.Character.ToString());
        }

        private GameTime screenTime = new GameTime();

        private void OnKeyDown(Keys key, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (key == Keys.P)
                makingscreenshot=true;

            var hold = keysHolds.Contains(key);

            SceneManager.Current?.OnKeyDown(new Dungeon.Control.Keys.KeyArgs
            {
                Key = (Key)key,
                Modifiers = GetModifier(),
                Hold = hold
            });
        }


        Vector2 shadowMaskPosition = Vector2.Zero;

        private int screenCounter = 0;
        private bool _screenshotSaving;
        private bool makingscreenshot;

        private void OnKeyUp(Keys key, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (key == Keys.P)
            {
                makingscreenshot=false;
                _screenshotSaving=false;
            }

            this.StopMoveCamera();

            keysHolds.Remove(key);

            SceneManager.Current?.OnKeyUp(new Dungeon.Control.Keys.KeyArgs
            {
                Key = (Key)key,
                Modifiers = GetModifier()
            });
        }

        private KeyModifiers GetModifier()
        {
            if(keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt))
            {
                return KeyModifiers.Alt;
            }

            if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl))
            {
                return KeyModifiers.Control;
            }

            if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
            {
                return KeyModifiers.Shift;
            }

            if (keyboardState.IsKeyDown(Keys.LeftWindows) || keyboardState.IsKeyDown(Keys.RightWindows))
            {
                return KeyModifiers.Windows;
            }

            return KeyModifiers.None;
        }
    }
}