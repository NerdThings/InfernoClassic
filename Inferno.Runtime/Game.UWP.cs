#if WINDOWS_UWP

using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime
{
    /// <summary>
    /// UWP Specific game code
    /// </summary>
    internal class PlatformGame
    {
        public bool RunEvents()
        {
            throw new NotImplementedException();
        }
    }
}

#endif