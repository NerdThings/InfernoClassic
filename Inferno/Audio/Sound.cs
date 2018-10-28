using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Audio
{
    public partial class Sound : IDisposable
    {
        internal Sound(byte[] data, int samplerate, int channels, int bits)
        {
            Initialise(data, samplerate, channels, bits);
        }

        /// <summary>
        /// Play sound
        /// </summary>
        public void Play()
        {
            PlaySound();
            AudioDevice.MarkSound(this, true);
        }

        /// <summary>
        /// Pause sound
        /// </summary>
        public void Pause()
        {
            PauseSound();
            AudioDevice.MarkSound(this, false);
        }

        /// <summary>
        /// Stop sound
        /// </summary>
        public void Stop()
        {
            StopSound();
            AudioDevice.MarkSound(this, false);
        }

        /// <summary>
        /// Dispose sound
        /// </summary>
        public void Dispose()
        {
            AudioDevice.DisposeSound(this);
        }
    }
}
