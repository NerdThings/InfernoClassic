using System;
using System.IO;
using Inferno.Audio;
using Inferno.Formats.Audio;
using Inferno.Formats.Graphics.Text;
using Inferno.Graphics;
using Inferno.Graphics.Text;

namespace Inferno.Content
{
    /// <summary>
    /// Content loader that manages game content loading *duh*
    /// </summary>
    public static partial class ContentLoader
    {
        private static string CorrectPath(string path)
        {
            if (!Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                path = Directory.GetCurrentDirectory() + "\\" + path;
            }

            return path;
        }

        #region Font

        public static Font FontFromFile(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                return FontFromStream(stream);
            }
        }

        public static Font FontFromStream(Stream stream)
        {
            return Font.FromFontFormat(FontFormat.FromStream(stream));
        }

        #endregion

        #region Texture2D

        public static Texture2D Texture2DFromFile(string filename)
        {
            //Correct path
            filename = CorrectPath(filename);
            
            //Create texture
            Texture2D result;
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                result = Texture2DFromStream(stream);
            }

            return result;
        }

        #endregion

        #region Internal Audio Files

        //Public for testing

        public static Sound LoadWaveFromFile(string filename)
        {
            filename = CorrectPath(filename);

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                return LoadWaveFromStream(stream);
            }
        }

        public static Sound LoadWaveFromStream(Stream stream)
        {
            return Sound.FromWave(WaveFormat.FromStream(stream));
        }


        #endregion
    }
}