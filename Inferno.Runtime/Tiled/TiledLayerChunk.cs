using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Tiled
{
    /// <summary>
    /// A chunk from a Tiled Layer
    /// </summary>
    public struct TiledLayerChunk
    {
        /// <summary>
        /// Starting X co-ord of the chunk
        /// </summary>
        public int X;

        /// <summary>
        /// Starting Y co-ord of the chunk
        /// </summary>
        public int Y;

        /// <summary>
        /// The width of the chunk
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the chunk
        /// </summary>
        public int Height;

        /// <summary>
        /// The tiles inside the chunk
        /// </summary>
        public List<int> Tiles;

        /// <summary>
        /// The parent map, used for getting the Tileset
        /// </summary>
        public TiledMap ParentMap;

        public void DrawChunk()
        {
            //Define starting points
            int x = 0;
            int y = 0;

            //Configure starting points
            if (ParentMap.RenderOrder == RenderOrder.RightDown)
            {
                x = X;
                y = Y;
            }
            else if (ParentMap.RenderOrder == RenderOrder.RightUp)
            {
                x = X;
                y = Y + Height - ParentMap.TileHeight;
            }
            else if (ParentMap.RenderOrder == RenderOrder.LeftDown)
            {
                x = X + Width - ParentMap.TileWidth;
                y = Y;
            }
            else if (ParentMap.RenderOrder == RenderOrder.LeftUp)
            {
                x = X + Width - ParentMap.TileWidth;
                y = Y + Height - ParentMap.TileHeight;
            }

            //Draw every tile
            for (int t = 0; t < Width / ParentMap.TileWidth * Height / ParentMap.TileHeight; t++)
            {
                if (Tiles.Count == 0 || Tiles.Count - 1 < t)
                    break;

                ParentMap.Tileset.DrawTile(new Vector2(x, y), Tiles[t]);

                //Increment logic
                if (ParentMap.RenderOrder == RenderOrder.RightDown)
                {
                    x += ParentMap.TileWidth;

                    if (x >= X + Width)
                    {
                        y += ParentMap.TileHeight;
                        x = X;
                    }
                }
                else if (ParentMap.RenderOrder == RenderOrder.RightUp)
                {
                    x += ParentMap.TileWidth;

                    if (x >= X + Width)
                    {
                        y -= ParentMap.TileHeight;
                        x = X;
                    }
                }
                else if (ParentMap.RenderOrder == RenderOrder.LeftDown)
                {
                    x -= ParentMap.TileWidth;

                    if (x >= X + Width)
                    {
                        y += ParentMap.TileHeight;
                        x = X + Width - ParentMap.TileWidth;
                    }
                }
                else if (ParentMap.RenderOrder == RenderOrder.LeftUp)
                {
                    x -= ParentMap.TileWidth;

                    if (x >= X + Width)
                    {
                        y -= ParentMap.TileHeight;
                        x = X + Width - ParentMap.TileWidth;
                    }
                }
            }
        }
    }
}
