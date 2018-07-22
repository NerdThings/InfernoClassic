using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Tiled
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
            int x = 0;
            int y = 0;

            //Configure starting points
            if (ParentMap.RenderOrder == RenderOrder.RightDown)
            {
                x = 0;
                y = 0;
            }
            else if (ParentMap.RenderOrder == RenderOrder.RightUp)
            {
                x = 0;
                y = Height - ParentMap.TileHeight;
            }
            else if (ParentMap.RenderOrder == RenderOrder.LeftDown)
            {
                x = Width - ParentMap.TileWidth;
                y = 0;
            }
            else if (ParentMap.RenderOrder == RenderOrder.LeftUp)
            {
                x = Width - ParentMap.TileWidth;
                y = Height - ParentMap.TileHeight;
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

                    if (x >= Width)
                    {
                        y += ParentMap.TileHeight;
                        x = 0;
                    }
                }
                else if (ParentMap.RenderOrder == RenderOrder.RightUp)
                {
                    x += ParentMap.TileWidth;

                    if (x >= Width)
                    {
                        y -= ParentMap.TileHeight;
                        x = 0;
                    }
                }
                else if (ParentMap.RenderOrder == RenderOrder.LeftDown)
                {
                    x -= ParentMap.TileWidth;

                    if (x >= Width)
                    {
                        y += ParentMap.TileHeight;
                        x = Width - ParentMap.TileWidth;
                    }
                }
                else if (ParentMap.RenderOrder == RenderOrder.LeftUp)
                {
                    x -= ParentMap.TileWidth;

                    if (x >= Width)
                    {
                        y -= ParentMap.TileHeight;
                        x = Width - ParentMap.TileWidth;
                    }
                }
            }

            //Draw all contained chunks
            foreach (TiledLayerChunk c in Chunks)
            {
                c.DrawChunk();
            }
        }
    }
}
