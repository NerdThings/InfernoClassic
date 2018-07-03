using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Graphics
{
    /// <summary>
    /// A tileset is an image which houses numerous tiles with unique IDs.
    /// This is based off Tiled's format and is interchangable
    /// </summary>
    public struct Tileset
    {
        /// <summary>
        /// The name of the Tileset
        /// </summary>
        public string Name;

        /// <summary>
        /// The width of each tile
        /// </summary>
        public int TileWidth;

        /// <summary>
        /// The height of each tile
        /// </summary>
        public int TileHeight;

        /// <summary>
        /// The number of tiles in the set
        /// </summary>
        public int TileCount;

        /// <summary>
        /// The source image for the tiles
        /// </summary>
        public Sprite Source;

        public void DrawTile(Vector2 Position, int id)
        {
            if (id == 0)
                return;

            //Calculate x and y of tile
            int x = 0;
            int y = 0;

            for (int i = 0; i < id - 1; i++)
            {
                if (x < Source.Width && x + TileWidth < Source.Width)
                {
                    x += TileWidth;
                }
                else if (x + TileWidth > Source.Width)
                {
                    x = TileWidth;
                    y += TileHeight;
                }
                else
                {
                    x = 0;
                    y += TileHeight;
                }
            }

            //Build rectangle
            Rectangle sourceRectangle = new Rectangle(x, y, TileWidth, TileHeight);

            //Draw
            Drawing.Draw_Sprite(Position, Source, sourceRectangle);
        }
    }
}
