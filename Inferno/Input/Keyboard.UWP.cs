using System;

#if WINDOWS_UWP

namespace Inferno.Input
{
    /// <summary>
    /// UWP specific keyboard code
    /// </summary>
    internal static class PlatformKeyboard
    {
        public static KeyboardState GetState()
        {
            throw new NotImplementedException();
        }
    }
}

#endif