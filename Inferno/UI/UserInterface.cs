using System;
using System.Collections.Generic;
using System.Text;
using Inferno.Core;
using Inferno.Graphics;
using Inferno.UI.Controls;

namespace Inferno.UI
{
    /// <summary>
    /// This is a controller for UI Controls.
    /// This will contain all UI elements for a state.
    /// </summary>
    public class UserInterface
    {
        public readonly List<Control> Controls;
        private readonly State _parentState;

        public UserInterface(State parentState)
        {
            Controls = new List<Control>();
            _parentState = parentState;
        }

        public void AddControl(Control control)
        {
            Controls.Add(control);
        }

        public void RemoveControl(Control control)
        {
            Controls.Remove(control);
        }

        public void Draw(Renderer renderer)
        {
            foreach (var control in Controls)
            {
                control.Draw(renderer);
            }
        }

        public void Update()
        {
            var boundary = _parentState.Camera.ViewportWorldBoundry;
            var hudOffset = new Vector2(boundary.X, boundary.Y);

            foreach (var control in Controls)
            {
                control.UIOffset = hudOffset;
                control.Update();
            }
        }
    }
}
