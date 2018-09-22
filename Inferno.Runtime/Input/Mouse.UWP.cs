#if WINDOWS_UWP

using System;
using Inferno.Runtime.Core;

namespace Inferno.Runtime.Input
{
    /// <summary>
    /// UWP specific mouse code
    /// </summary>
    public class PlatformMouse
    {
        /// <summary>
        /// Gets mouse state.
        /// </summary>
        /// <param name="currentState">The current game state</param>
        /// <returns>The Mouse State Information</returns>
        public static MouseState GetState(State currentState)
        {
            throw new NotImplementedException();
        }
    }
}

#endif