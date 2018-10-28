using System.Collections.Generic;

namespace Inferno.Input
{
    /// <summary>
    /// State of a keyboard key
    /// </summary>
    public enum KeyState
    {
        /// <summary>
        /// The key is down
        /// </summary>
        Down,

        /// <summary>
        /// The key is up
        /// </summary>
        Up
    }

    /// <summary>
    /// The state of a keyboard
    /// </summary>
    public struct KeyboardState
    {
        static Key[] empty = new Key[0];

        #region Internal Stuff
        //Using Monogame 256bit keystate
        uint keys0, keys1, keys2, keys3, keys4, keys5, keys6, keys7;

        internal bool InternalGetKey(Key key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);

            uint element;
            switch (((int)key) >> 5)
            {
                case 0: element = keys0; break;
                case 1: element = keys1; break;
                case 2: element = keys2; break;
                case 3: element = keys3; break;
                case 4: element = keys4; break;
                case 5: element = keys5; break;
                case 6: element = keys6; break;
                case 7: element = keys7; break;
                default: element = 0; break;
            }

            return (element & mask) != 0;
        }

        internal void InternalSetKey(Key key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);
            switch (((int)key) >> 5)
            {
                case 0: keys0 |= mask; break;
                case 1: keys1 |= mask; break;
                case 2: keys2 |= mask; break;
                case 3: keys3 |= mask; break;
                case 4: keys4 |= mask; break;
                case 5: keys5 |= mask; break;
                case 6: keys6 |= mask; break;
                case 7: keys7 |= mask; break;
            }
        }

        internal void InternalClearKey(Key key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);
            switch (((int)key) >> 5)
            {
                case 0: keys0 &= ~mask; break;
                case 1: keys1 &= ~mask; break;
                case 2: keys2 &= ~mask; break;
                case 3: keys3 &= ~mask; break;
                case 4: keys4 &= ~mask; break;
                case 5: keys5 &= ~mask; break;
                case 6: keys6 &= ~mask; break;
                case 7: keys7 &= ~mask; break;
            }
        }

        internal void InternalClearAllKeys()
        {
            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;
        }

        #endregion

        public bool CapsLock { get; private set; }
        public bool NumLock { get; private set; }

        public KeyboardState(List<Key> keys, bool capsLock = false, bool numLock = false) : this()
        {
            CapsLock = capsLock;
            NumLock = numLock;

            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;

            if (keys != null)
                foreach (Key k in keys)
                    InternalSetKey(k);
        }

        public KeyState this[Key key] => InternalGetKey(key) ? KeyState.Down : KeyState.Up;

        public bool IsKeyDown(Key key)
        {
            return InternalGetKey(key);
        }

        public bool IsKeyUp(Key key)
        {
            return !InternalGetKey(key);
        }

        #region Operators

        public static bool operator ==(KeyboardState a, KeyboardState b)
        {
            return a.keys0 == b.keys0
                    && a.keys1 == b.keys1
                    && a.keys2 == b.keys2
                    && a.keys3 == b.keys3
                    && a.keys4 == b.keys4
                    && a.keys5 == b.keys5
                    && a.keys6 == b.keys6
                    && a.keys7 == b.keys7;
        }

        public static bool operator !=(KeyboardState a, KeyboardState b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is KeyboardState state && this == state;
        }

        public override int GetHashCode()
        {
            return (int)(keys0 ^ keys1 ^ keys2 ^ keys3 ^ keys4 ^ keys5 ^ keys6 ^ keys7);
        }

        #endregion  
    }
}
