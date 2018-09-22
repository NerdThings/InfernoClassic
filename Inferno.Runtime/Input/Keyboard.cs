namespace Inferno.Runtime.Input
{
    public static class Keyboard
    {
        public static KeyboardState GetState()
        {
            return PlatformKeyboard.GetState();
        }
    }
}
