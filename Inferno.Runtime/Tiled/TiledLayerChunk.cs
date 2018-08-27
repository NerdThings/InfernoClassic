using System;
using System.Collections.Generic;

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
            int x;
            int y;

            switch (ParentMap.RenderOrder)
            {
                //Configure starting points
                case RenderOrder.RightDown:
                    x = X;
                    y = Y;
                    break;
                case RenderOrder.RightUp:
                    x = X;
                    y = Y + Height - ParentMap.TileHeight;
                    break;
                case RenderOrder.LeftDown:
                    x = X + Width - ParentMap.TileWidth;
                    y = Y;
                    break;
                case RenderOrder.LeftUp:
                    x = X + Width - ParentMap.TileWidth;
                    y = Y + Height - ParentMap.TileHeight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Draw every tile
            for (var t = 0; t < Width / ParentMap.TileWidth * Height / ParentMap.TileHeight; t++)
            {
                if (Tiles.Count == 0 || Tiles.Count - 1 < t)
                    break;

                ParentMap.Tileset.DrawTile(new Vector2(x, y), Tiles[t]);

                switch (ParentMap.RenderOrder)
                {
                    //Increment logic
                    case RenderOrder.RightDown:
                        x += ParentMap.TileWidth;

                        if (x >= X + Width)
                        {
                            y += ParentMap.TileHeight;
                            x = X;
                        }

                        break;
                    case RenderOrder.RightUp:
                        x += ParentMap.TileWidth;

                        if (x >= X + Width)
                        {
                            y -= ParentMap.TileHeight;
                            x = X;
                        }

                        break;
                    case RenderOrder.LeftDown:
                        x -= ParentMap.TileWidth;

                        if (x >= X + Width)
                        {
                            y += ParentMap.TileHeight;
                            x = X + Width - ParentMap.TileWidth;
                        }

                        break;
                    case RenderOrder.LeftUp:
                        x -= ParentMap.TileWidth;

                        if (x >= X + Width)
                        {
                            y -= ParentMap.TileHeight;
                            x = X + Width - ParentMap.TileWidth;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
