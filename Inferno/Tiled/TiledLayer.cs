using System;
using System.Collections.Generic;

namespace Inferno.Tiled
{
    /// <summary>
    /// A layer from a Tiled map
    /// </summary>
    public struct TiledLayer
    {
        /// <summary>
        /// Name of the layer
        /// </summary>
        public string Name;

        /// <summary>
        /// Width of the layer
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the layer
        /// </summary>
        public int Height;

        /// <summary>
        /// Layer tiles (for finite maps)
        /// </summary>
        public List<int> Tiles;

        /// <summary>
        /// Layer chunks (for infinite maps)
        /// </summary>
        public List<TiledLayerChunk> Chunks;

        /// <summary>
        /// The parent map, used for getting the Tileset
        /// </summary>
        public TiledMap ParentMap;

        public void DrawLayer()
        {
            //Define starting points
            int x;
            int y;

            switch (ParentMap.RenderOrder)
            {
                //Configure starting points
                case RenderOrder.RightDown:
                    x = 0;
                    y = 0;
                    break;
                case RenderOrder.RightUp:
                    x = 0;
                    y = Height - ParentMap.TileHeight;
                    break;
                case RenderOrder.LeftDown:
                    x = Width - ParentMap.TileWidth;
                    y = 0;
                    break;
                case RenderOrder.LeftUp:
                    x = Width - ParentMap.TileWidth;
                    y = Height - ParentMap.TileHeight;
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

                        if (x >= Width)
                        {
                            y += ParentMap.TileHeight;
                            x = 0;
                        }

                        break;
                    case RenderOrder.RightUp:
                        x += ParentMap.TileWidth;

                        if (x >= Width)
                        {
                            y -= ParentMap.TileHeight;
                            x = 0;
                        }

                        break;
                    case RenderOrder.LeftDown:
                        x -= ParentMap.TileWidth;

                        if (x >= Width)
                        {
                            y += ParentMap.TileHeight;
                            x = Width - ParentMap.TileWidth;
                        }

                        break;
                    case RenderOrder.LeftUp:
                        x -= ParentMap.TileWidth;

                        if (x >= Width)
                        {
                            y -= ParentMap.TileHeight;
                            x = Width - ParentMap.TileWidth;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //Draw all contained chunks
            foreach (var c in Chunks)
            {
                c.DrawChunk();
            }
        }
    }
}
