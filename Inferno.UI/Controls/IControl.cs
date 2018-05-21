using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.UI.Controls
{
    /// <summary>
    /// This interface implements new manditory additions which aren't present in Instance
    /// </summary>
    public interface IControl
    {
        Color ForeColor { get; set; }
        Color BackColor { get; set; }
        Color BorderColor { get; set; }
        int BorderWidth { get; set; }
        Rectangle ControlBounds { get; set; }
        ControlState State { get; set; }
        Sprite BackgroundImage { get; set; }
    }
}
