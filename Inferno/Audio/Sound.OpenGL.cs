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

        public void Dispose()
        {
            //TODO
        }
    }
}

#endif