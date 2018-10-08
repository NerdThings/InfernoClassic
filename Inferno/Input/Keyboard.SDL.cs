#if DESKTOP

using SDL2;

namespace Inferno.Input
{
    /// <summary>
    /// SDL specific keyboard code
    /// </summary>
    internal static class PlatformKeyboard
    {
        public static KeyboardState GetState()
        {
            var modifiers = SDL2.SDL.SDL_GetModState();
            return new KeyboardState(Game.Instance.Keys,
                (modifiers & SDL2.SDL.SDL_Keymod.KMOD_CAPS) == SDL2.SDL.SDL_Keymod.KMOD_CAPS,
                (modifiers & SDL2.SDL.SDL_Keymod.KMOD_NUM) == SDL2.SDL.SDL_Keymod.KMOD_NUM);
        }
    }
}

#endif