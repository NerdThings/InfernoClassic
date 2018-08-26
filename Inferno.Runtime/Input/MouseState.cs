using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Input
{
    public struct MouseState
    {
        public int X;
        public int Y;
        public int ScrollWheelValue;
        public ButtonState LeftButton;
        public ButtonState MiddleButton;
        public ButtonState RightButton;
        public ButtonState XButton1;
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
