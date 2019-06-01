﻿namespace Rogue
{
    using Microsoft.Xna.Framework.Input;
    using Rogue.Control.Pointer;
    using Rogue.Scenes.Manager;
    using System;
    using System.Collections.Generic;

    public partial class XNADrawClient
    {
        private Microsoft.Xna.Framework.Point mousePosition;
        private int scrollWeelValue;
        private MouseState mouseState;

        private void UpdateMouseEvents()
        {
            mouseState = Mouse.GetState();

            if (mouseState.Position != mousePosition)
            {
                mousePosition = mouseState.Position;
                OnPointerMoved();
            }

            MouseClicksPipe
                (CheckMouseClick, MouseButton.Right, mouseState.RightButton)
                (CheckMouseClick, MouseButton.Left, mouseState.LeftButton)
                (CheckMouseClick, MouseButton.Middle, mouseState.MiddleButton);

            if (mouseState.ScrollWheelValue != scrollWeelValue)
            {
                OnPointerWheelChanged(mouseState.ScrollWheelValue > scrollWeelValue);
                scrollWeelValue = mouseState.ScrollWheelValue;
            }
        }

        private void OnPointerWheelChanged(bool isTop)
        {
            Console.WriteLine(isTop);

            SceneManager.Current.OnMouseWheel(isTop ? Control.Pointer.MouseWheelEnum.Up : Control.Pointer.MouseWheelEnum.Down);
        }

        private delegate ConditionalDelegate ConditionalDelegate(Func<MouseButton, ButtonState, bool> func, MouseButton arg, ButtonState arg2);

        private ConditionalDelegate MouseClicksPipe(Func<MouseButton, ButtonState, bool> action, MouseButton arg, ButtonState arg2)
        {
            if (action?.Invoke(arg,arg2) ?? false)
            {
                return MouseClicksPipeEmpty;
            }
            else
            {
                return MouseClicksPipe;
            }
        }

        private ConditionalDelegate MouseClicksPipeEmpty(Func<MouseButton, ButtonState, bool> action, MouseButton arg, ButtonState arg2)
        {
            return MouseClicksPipeEmpty;
        }

        private bool CheckMouseClick(MouseButton mouseButton, ButtonState buttonState)
        {
            var changes = buttonPressings[mouseButton] != buttonState;

            if (!changes)
                return false;
            
            if (buttonPressings[mouseButton] != buttonState)
            {
                if (buttonPressings[mouseButton] == ButtonState.Released)
                {
                    buttonPressings[mouseButton] = ButtonState.Pressed;
                    OnPointerPressed(mouseButton);
                }
                else
                {
                    buttonPressings[mouseButton] = ButtonState.Released;
                    OnPointerReleased(mouseButton);
                }
            }

            return true;
        }

        private void OnPointerMoved()
        {
            var currentScene = SceneManager.Current;

            //if (currentScene.CameraAffect)
            //{
            //    drawClient.MoveCamera(Types.Direction.Right, !(pos.X >= 1180));
            //    drawClient.MoveCamera(Types.Direction.Left, !(pos.X <= 100));
            //    drawClient.MoveCamera(Types.Direction.Down, !(pos.Y >= 620));
            //    drawClient.MoveCamera(Types.Direction.Up, !(pos.Y <= 100));
            //}
            
            MouseButton mb = MouseButton.None;

            if (mouseState.LeftButton == ButtonState.Pressed)
                mb = MouseButton.Left;

            if (mouseState.RightButton == ButtonState.Pressed)
                mb = MouseButton.Left;

            if (mouseState.MiddleButton == ButtonState.Pressed)
                mb = MouseButton.Middle;

            currentScene.OnMouseMove(new Control.Pointer.PointerArgs
            {
                ClickCount = 0,
                MouseButton = (Control.Pointer.MouseButton)mb,
                X = mousePosition.X,
                Y = mousePosition.Y
            }, new Types.Point(CameraOffsetX, CameraOffsetY));
        }

        private readonly Dictionary<MouseButton, ButtonState> buttonPressings = new Dictionary<MouseButton, ButtonState>()
        {
            { MouseButton.Left, ButtonState.Released },
            { MouseButton.Right, ButtonState.Released },
            { MouseButton.Middle, ButtonState.Released },
        };

        private void OnPointerPressed(MouseButton mouseButton)
        {
            var pos = mousePosition;
            var offset = new Types.Point(CameraOffsetX, CameraOffsetY);

            SceneManager.Current.OnMousePress(new Control.Pointer.PointerArgs
            {
                ClickCount = 1,
                MouseButton = mouseButton,
                X = pos.X,
                Y = pos.Y,
                Offset = offset
            }, offset);
        }

        private void OnPointerReleased(MouseButton mouseButton)
        {
            var pos = mousePosition;
            var offset = new Types.Point(CameraOffsetX, CameraOffsetY);

            SceneManager.Current.OnMouseRelease(new Control.Pointer.PointerArgs
            {
                ClickCount = 1,
                MouseButton = mouseButton,
                X = pos.X,
                Y = pos.Y,
                Offset = offset
            }, offset);
        }
    }
}