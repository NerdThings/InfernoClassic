using System;
using Inferno.Formats.Audio;

namespace Inferno.Audio
{
    public partial class Sound : IDisposable
    {
        internal Sound(byte[] data, int samplerate, int channels, int bits)
        {
            Initialise(data, samplerate, channels, bits);
        }

        /// <summary>
        /// Create sound from Wave Format
        /// </summary>
        /// <param name="wave">Wave data</param>
        /// <returns>Sound from wave data</returns>
        public static Sound FromWave(WaveFormat wave)
        {
            return new Sound(wave.Data, wave.SampleRate, wave.NumChannels, wave.BitsPerSample);
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
