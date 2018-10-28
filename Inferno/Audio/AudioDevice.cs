using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Audio
{
    public partial class AudioDevice : IDisposable
    {
        private static List<Sound> _soundsPlaying;
        private static List<Sound> _toDispose;

        internal AudioDevice()
        {
            Initialise();
            _toDispose = new List<Sound>();
            _soundsPlaying = new List<Sound>();
        }

        public void PauseAll()
        {
            foreach (var sound in _soundsPlaying)
            {
                sound.PauseSound();
            }
        }

        public void ResumeAll()
        {
            foreach (var sound in _soundsPlaying)
            {
                sound.PlaySound();
            }
        }

        public void Update()
        {
            //Dispose sounds that need disposing
            foreach (var sound in _toDispose)
            {
                DisposeSoundNow(sound);
            }

            _toDispose.Clear();
        }

        internal static void MarkSound(Sound sound, bool playing)
        {
            if (playing)
            {
                if (!_soundsPlaying.Contains(sound))
                    _soundsPlaying.Add(sound);
            }
            else
            {
                if (_soundsPlaying.Contains(sound))
                    _soundsPlaying.Remove(sound);
            }
        }

        public static void DisposeSound(Sound sound)
        {
            _toDispose.Add(sound);
        }
    }
}
