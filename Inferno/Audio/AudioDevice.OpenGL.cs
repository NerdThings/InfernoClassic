#if OPENGL

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Inferno.Audio
{
    public partial class AudioDevice
    {
        /// <summary>
        /// The GL Audio Context
        /// </summary>
        internal AudioContext AudioContext;

        /// <summary>
        /// Initialise the device
        /// </summary>
        public void Initialise()
        {
            AudioContext = new AudioContext();
        }

        /// <summary>
        /// Dispose a sound
        /// </summary>
        /// <param name="sound">Sound to dispose</param>
        private void DisposeSoundNow(Sound sound)
        {
            AL.SourceStop(sound.Source);
            AL.DeleteSource(sound.Source);
            AL.DeleteBuffer(sound.Buffer);

            sound.Source = -1;
            sound.Buffer = -1;
        }

        /// <summary>
        /// Dispose the audio device
        /// </summary>
        public void Dispose()
        {
            AudioContext.Dispose();
        }
    }
}

#endif