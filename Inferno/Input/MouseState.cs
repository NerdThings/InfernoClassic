using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Input
{
    /// <summary>
    /// Information about the mouse state
    /// </summary>
    public struct MouseState
    {
        /// <summary>
        /// X coordinate of the mouse on screen
        /// </summary>
        public int X;

        /// <summary>
        /// Y coordinate of the mouse on screen
        /// </summary>
        public int Y;

        /// <summary>
        /// The value of the scroll wheel
        /// </summary>
        public int ScrollWheelValue;

        /// <summary>
        /// Left click state
        /// </summary>
        public ButtonState LeftButton;

        /// <summary>
        /// Middle click state
        /// </summary>
        public ButtonState MiddleButton;

        /// <summary>
        /// Right click state
        /// </summary>
        public ButtonState RightButton;

        /// <summary>
        /// Extra button 1 state
        /// </summary>
        public ButtonState XButton1;

        /// <summary>
        /// Extra button 2 state
        /// </summary>
        public ButtonState XButton2;

        public MouseState(int x, int y, int scrollwheel, ButtonState left, ButtonState middle, ButtonState right, ButtonState x1, ButtonState x2)
        {
            X = x;
            Y = y;
            ScrollWheelValue = scrollwheel;
            LeftButton = left;
            MiddleButton = middle;
            RightButton = right;
            XButton1 = x1;
            XButton2 = x2;
        }
    }
}
