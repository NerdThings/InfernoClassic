#if DESKTOP

using SDL2;

namespace Inferno.Runtime.Input
{
    /// <summary>
    /// SDL specific keyboard code
    /// </summary>
    internal static class PlatformKeyboard
    {
        public static KeyboardState GetState()
        {
            var modifiers = SDL.SDL_GetModState();
            return new KeyboardState(Game.Instance.Keys, 
                (modifiers & SDL.SDL_Keymod.KMOD_CAPS) == SDL.SDL_Keymod.KMOD_CAPS,
                (modifiers & SDL.SDL_Keymod.KMOD_NUM) == SDL.SDL_Keymod.KMOD_NUM);
        }
    }
}

#endif