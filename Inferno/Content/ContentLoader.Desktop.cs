using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Inferno.Graphics;
using Color = Inferno.Graphics.Color;

#if DESKTOP

namespace Inferno.Content
{
    public static partial class ContentLoader
    {
        #region Texture2D
        
        public static Texture2D Texture2DFromStream(Stream stream)
        {
            using (var bitmap = new Bitmap(stream))
            {
                return Texture2DFromBitmap(bitmap);
            }
        }
        
        public static Texture2D Texture2DFromBitmap(Bitmap bitmap)
        {
            //Get data from bitmap
            var data = GetColors(bitmap);
            
            //Create texture with color array
            return new Texture2D(bitmap.Width, bitmap.Height, data);
        }

        private static unsafe Color[] GetColors(Bitmap bitmap)
        {
            var data = new Color[bitmap.Width * bitmap.Height];

            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                {
                    var ptr = (byte*)bitmapData.Scan0;
                    var i = 0;
                    for (var y = 0; y < bitmap.Height; ++y)
                    {
                        for (var x = 0; x < bitmap.Width; ++x)
                        {
                            var c = new Color(*(ptr + 2), *(ptr + 1), *ptr, *(ptr + 3));
                            data[i] = c;

                            i++;
                            ptr += 4;
                        }
                    }

                    break;
                }

                case PixelFormat.Format24bppRgb:
                {
                    var ptr = (byte*)bitmapData.Scan0;
                    var i = 0;
                    for (var y = 0; y < bitmap.Height; ++y)
                    {
                        for (var x = 0; x < bitmap.Width; ++x)
                        {
                            var c = new Color(*(ptr + 2), *(ptr + 1), *ptr, (byte)255);
                            data[i] = c;

                            i++;
                            ptr += 3;
                        }
                    }

                    break;
                }
                default:
                    throw new Exception("Other pixel data types are not supported.");
            }

            bitmap.UnlockBits(bitmapData);

            return data;
        }

        #endregion
    }
}

#endif