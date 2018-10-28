using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Audio
{
    /// <summary>
    /// The system audio device
    /// </summary>
    public partial class AudioDevice : IDisposable
    {
        /// <summary>
        /// List of sounds currently playing
        /// </summary>
        private static List<Sound> _soundsPlaying;

        /// <summary>
        /// List of sounds to be disposed
        /// </summary>
        private static List<Sound> _toDispose;

        internal AudioDevice()
        {
            Initialise();
            _toDispose = new List<Sound>();
            _soundsPlaying = new List<Sound>();
        }

        ~AudioDevice()
        {
            Dispose();
        }

        /// <summary>
        /// Pause all currently playing sounds
        /// </summary>
        public void PauseAll()
        {
            foreach (var sound in _soundsPlaying)
            {
                sound.PauseSound();
            }
        }

        /// <summary>
        /// Resume all currently playing sounds
        /// </summary>
        public void ResumeAll()
        {
            foreach (var sound in _soundsPlaying)
            {
                sound.PlaySound();
            }
        }

        /// <summary>
        /// Update call to dispose sounds
        /// </summary>
        internal void Update()
        {
            //Dispose sounds that need disposing
            foreach (var sound in _toDispose)
            {
                DisposeSoundNow(sound);
            }

            _toDispose.Clear();
        }

        /// <summary>
        /// Mark a sounds status
        /// </summary>
        /// <param name="sound">The sound to mark</param>
        /// <param name="playing">Playing or not</param>
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

        /// <summary>
        /// Dispose a sound
        /// </summary>
        /// <param name="sound"></param>
        internal static void DisposeSound(Sound sound)
        {
            _toDispose.Add(sound);
        }
    }
}
