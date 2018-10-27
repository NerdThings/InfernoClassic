using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Audio
{
    public partial class AudioDevice : IDisposable
    {
        private List<Sound> _playingSounds;
        private List<Sound> _toDispose;

        internal AudioDevice()
        {
            Initialise();
        }

        public void Update()
        {
            //Scan over playing sounds

            //Dispose sounds that need disposing
        }
    }
}
