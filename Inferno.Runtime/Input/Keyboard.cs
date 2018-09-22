namespace Inferno.Runtime.Input
{
    /// <summary>
    /// Keyboard input
    /// </summary>
    public static class Keyboard
    {
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
