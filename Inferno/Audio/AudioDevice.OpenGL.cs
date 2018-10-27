#if OPENGL

using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Inferno.Audio
{
    public partial class AudioDevice
    {
        internal AudioContext AudioContext;

        public void Initialise()
        {
            AudioContext = new AudioContext();
        }

        public void Play(Sound sound)
        {
            AL.SourcePlay(sound.Source);
        }

        public void Dispose()
        {
            AudioContext.Dispose();
        }
    }
}

#endif