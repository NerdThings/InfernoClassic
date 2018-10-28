#if OPENGL

using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Audio.OpenAL;

namespace Inferno.Audio
{
    public partial class Sound
    {
        internal int Buffer;
        internal int Source;

        #region Properties

        public float Volume
        {
            get
            {
                AL.GetSource(Source, ALSourcef.Gain, out var volume);
                return volume;
            }
            set => AL.Source(Source, ALSourcef.Gain, value);
        }

        public float Pitch
        {
            get
            {
                AL.GetSource(Source, ALSourcef.Pitch, out var pitch);
                return pitch;
            }
            set => AL.Source(Source, ALSourcef.Pitch, value);
        }

        public bool Looping
        {
            get
            {
                AL.GetSource(Source, ALSourceb.Looping, out var looping);
                return looping;
            }
            set => AL.Source(Source, ALSourceb.Looping, value);
        }

        #endregion

        private void Initialise(byte[] data, int samplerate, int channels, int bits)
        {
            Buffer = AL.GenBuffer();
            Source = AL.GenSource();

            AL.BufferData(Buffer, GetSoundFormat(channels, bits), data, data.Length, samplerate);
            AL.Source(Source, ALSourcei.Buffer, Buffer);
        }

        private ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        public void SetVolume(int volume)
        {
            AL.Source(Source, ALSourcef.Gain, volume);
        }

        internal void PlaySound()
        {
            AL.SourcePlay(Source);
        }

        internal void PauseSound()
        {
            AL.SourcePause(Source);
        }

        internal void StopSound()
        {
            AL.SourceStop(Source);
        }

        public void Dispose()
        {
            AudioDevice.DisposeSound(this);
        }
    }
}

#endif