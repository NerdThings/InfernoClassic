using System;

namespace Inferno.Input
{
    /// <summary>
    /// Keyboard input
    /// </summary>
    public static class Keyboard
    {
        public static EventHandler<KeyEventArgs> KeyPressed = (sender, args) => { };
        public static EventHandler<KeyEventArgs> KeyReleased = (sender, args) => { };
        
        public class KeyEventArgs : EventArgs
        {
            public Key Key;

            public KeyEventArgs(Key key)
            {
                Key = key;
            }
        }
        
        /// <summary>
        /// Get the current state of the keyboard
        /// </summary>
        /// <returns></returns>
        public static KeyboardState GetState()
        {
            return PlatformKeyboard.GetState();
        }
    }
}
