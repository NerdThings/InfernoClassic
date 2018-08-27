using System;
using System.ComponentModel;
using System.Diagnostics;
using Inferno.Runtime.Input;

namespace Inferno.Runtime
{
    public abstract class GameWindow
    {

        #region Properties

        [DefaultValue(false)]
        public abstract bool AllowResize { get; set; }

        public abstract Rectangle Bounds { get; set; }

        public virtual bool AllowAltF4 { get; set; }

        public abstract Point Position { get; set; }

        //TODO: Orientation (Mobile??)

        public abstract IntPtr Handle { get; }

        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                if (value != null && _title != value)
                {
                    _title = value;
                }
            }
        }

        internal MouseState MouseState;

        #endregion

    }
}
