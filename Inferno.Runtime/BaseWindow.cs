using System;
using System.ComponentModel;
using System.Diagnostics;
using Inferno.Runtime.Input;

namespace Inferno.Runtime
{
    /// <summary>
    /// The part of the gamewindow that is generic to all platforms
    /// </summary>
    public abstract class BaseGameWindow
    {
        #region Properties

        [DefaultValue(false)]
        public abstract bool AllowResize { get; set; }

        public abstract Rectangle Bounds { get; set; }

        public abstract int Width { get; set; }

        public abstract int Height { get; set; }

        public abstract bool AllowAltF4 { get; set; }

        public abstract Point Position { get; set; }

        //TODO: Orientation (Mobile??)

        protected IntPtr Handle { get; set; }

        public abstract string Title { get; set; }

        internal MouseState MouseState;

        #endregion
    }
}
