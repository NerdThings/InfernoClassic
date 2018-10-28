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

        private void DisposeSoundNow(Sound sound)
        {
            AL.SourceStop(sound.Source);
            AL.DeleteSource(sound.Source);
            AL.DeleteBuffer(sound.Buffer);
        }

        public void Dispose()
        {
            AudioContext.Dispose();
        }
    }
}

#endif