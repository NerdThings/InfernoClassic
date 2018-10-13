using Inferno.Graphics;
using System.Collections.Generic;

namespace Inferno.Tiled
{
    /// <summary>
    /// The orientation of a Tiled map
    /// </summary>
    public enum TiledMapOrientation
    {
        Orthogonal
        //Other formats not supported yet
    }

    /// <summary>
    /// The redner order of a Tiled map
    /// </summary>
    public enum RenderOrder
    {
        RightDown,
        RightUp,
        LeftDown,
        LeftUp
    }

    /// <summary>
    /// A Tiled map.
    /// Must be CSV formatted.
    /// Objects are loaded but must be manually implemented.
    /// </summary>
    public struct TiledMap
    {
        /// <summary>
        /// The orientation of the map
        /// </summary>
        public TiledMapOrientation Orientation;

        /// <summary>
        /// The render order of the map
        /// </summary>
        public RenderOrder RenderOrder;

        /// <summary>
        /// The width of the map
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the map
        /// </summary>
        public int Height;

        /// <summary>
        /// The width of the tiles in the map
        /// </summary>
        public int TileWidth;
        
        /// <summary>
        /// The height of the tiles in the map
        /// </summary>
        public int TileHeight;

        /// <summary>
        /// Whether or not the map is infinite
        /// </summary>
        public bool Infinite;

        /// <summary>
        /// The tileset used in the map
        /// </summary>
        public Tileset Tileset;

        /// <summary>
        /// The layers inside the map
        /// </summary>
        public List<TiledLayer> Layers;

        /// <summary>
        /// The object layers inside the map
        /// </summary>
        public List<TiledObjectLayer> ObjectLayers;

        /// <summary>
        /// Draw the entire map
        /// </summary>
        public void DrawMap()
        {
            //Draw every layer
            foreach (var l in Layers)
            {
                l.DrawLayer();
            }
        }
    }
}
