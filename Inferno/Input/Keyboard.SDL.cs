#if SDL

using SDL2;

namespace Inferno.Input
{
    public static partial class Keyboard
    {
        /// <summary>
        /// Get the current state of the keyboard
        /// </summary>
        /// <returns></returns>
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