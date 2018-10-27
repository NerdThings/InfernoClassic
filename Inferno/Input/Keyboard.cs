using System;

namespace Inferno.Input
{
    /// <summary>
    /// Keyboard input
    /// </summary>
    public static partial class Keyboard
    {
        /// <summary>
        /// Event triggered when a key is pressed
        /// </summary>
        public static EventHandler<KeyEventArgs> KeyPressed = (sender, args) => { };
        
        /// <summary>
        /// Event triggered when a key is released
        /// </summary>
        public static EventHandler<KeyEventArgs> KeyReleased = (sender, args) => { };
        
        public class KeyEventArgs : EventArgs
        {
            public readonly Key Key;

            public KeyEventArgs(Key key)
            {
                Key = key;
            }
        }
    }
}
