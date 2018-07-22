using Inferno.Runtime.Core;

namespace Inferno.UWP.XAML.Template
{
    /// <summary>
    /// This is the first screen in your game
    /// </summary>
    public class State1 : State
    {
        public State1(Runtime.Game parent) : base(parent)
        {
            //Associate load and unload events
            OnStateLoad += State1_OnStateLoad;
            OnStateUnLoad += State1_OnStateUnLoad;

            //Add your state construction logic here
        }

        private void State1_OnStateUnLoad(object sender, System.EventArgs e)
        {
            //Add UnLoad logic
        }

        private void State1_OnStateLoad(object sender, System.EventArgs e)
        {
            //Add Load logic
        }
    }
}
