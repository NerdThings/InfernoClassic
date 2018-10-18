using System.Collections.Generic;
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
        private readonly List<Control> _controls;
        private readonly GameState _parentState;

        internal UserInterface(GameState parentState)
        {
            _controls = new List<Control>();
            _parentState = parentState;
        }

        /// <summary>
        /// Add a Control to the UI Manager
        /// </summary>
        /// <param name="control">Control to add</param>
        public void AddControl(Control control)
        {
            _controls.Add(control);
        }

        /// <summary>
        /// Remove a Control from the UI Manager
        /// </summary>
        /// <param name="control">Control to remove</param>
        public void RemoveControl(Control control)
        {
            _controls.Remove(control);
        }

        /// <summary>
        /// Remove all Controls from the UI Manager
        /// </summary>
        public void RemoveAllControls()
        {
            _controls.Clear();
        }

        /// <summary>
        /// Draw all of the Controls in the UI Manager
        /// </summary>
        /// <param name="renderer"></param>
        public void Draw(Renderer renderer)
        {
            foreach (var control in _controls)
            {
                control.Draw(renderer);
            }
        }

        /// <summary>
        /// Update all of the Controls in the UI Manager
        /// </summary>
        public void Update()
        {
            var boundary = _parentState.Camera.ViewportWorldBoundry;
            var hudOffset = new Vector2(boundary.X, boundary.Y);

            foreach (var control in _controls)
            {
                control.UIOffset = hudOffset;
                control.Update();
            }
        }
    }
}
