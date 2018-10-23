using System;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Messaging;
using Inferno.Graphics;

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
    }
}