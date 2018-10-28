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

        public void Play()
        {
            PlaySound();
            AudioDevice.MarkSound(this, true);
        }

        public void Pause()
        {
            PauseSound();
            AudioDevice.MarkSound(this, false);
        }

        public void Stop()
        {
            StopSound();
            AudioDevice.MarkSound(this, false);
        }
    }
}
