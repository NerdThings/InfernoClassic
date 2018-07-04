using Inferno.Runtime.Core;
using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Inferno.Runtime.Tiled
{
    /// <summary>
    /// Simple version struct to aid version checking
    /// </summary>
    public struct TiledVersion
    {
        public int major;
        public int minor;
        public int build;

        public TiledVersion(string str)
        {
            string[] bits = str.Split('.');
            major = int.Parse(bits[0]);
            minor = int.Parse(bits[1]);
            build = int.Parse(bits[2]);
        }
    }

    /// <summary>
    /// This class loads things from Tiled files.
    /// </summary>
    public static class TiledLoader
    {
        /// <summary>
        /// Load a Tiled map.
        /// Compatible with Tiled 1.1.5 and up.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="ObjectMap"></param>
        /// <returns></returns>
        public static TiledMap LoadMap(string filename)
        {
            //Load xml
            XDocument doc = XDocument.Load(filename);

            //Get map element
            XElement map = doc.Element("map");

            //Version check
            TiledVersion version = new TiledVersion(map.Attribute("tiledversion").Value);
            if (!Compatible(version))
                throw new Exception("Incompatible Tiled version.");

            //Construct map
            TiledMap retMap = new TiledMap();
            retMap.Height = int.Parse(map.Attribute("height").Value);
            retMap.Infinite = map.Attribute("infinite").Value == "1"?true:false;
            retMap.Layers = new List<TiledLayer>();
            retMap.ObjectLayers = new List<TiledObjectLayer>();

            //Load the tileset
            retMap.Tileset = LoadTileset(map.Element("tileset").Attribute("source").Value);

            //Set the orientation and render order
            string Orientation = map.Attribute("orientation").Value;
            string RenderOrder = map.Attribute("renderorder").Value;

            if (Orientation == "orthogonal")
                retMap.Orientation = TiledMapOrientation.Orthogonal;
            else
                throw new Exception("Unsupported orientation detected.");

            if (RenderOrder == "right-down")
                retMap.RenderOrder = Tiled.RenderOrder.RightDown;
            else if (RenderOrder == "right-up")
                retMap.RenderOrder = Tiled.RenderOrder.RightUp;
            else if (RenderOrder == "left-down")
                retMap.RenderOrder = Tiled.RenderOrder.LeftDown;
            else if (RenderOrder == "left-up")
                retMap.RenderOrder = Tiled.RenderOrder.LeftUp;
            else
                throw new Exception("Unsupported RenderOrder.");

            //Finish getting attributes
            retMap.TileHeight = int.Parse(map.Attribute("tileheight").Value);
            retMap.TileWidth = int.Parse(map.Attribute("tilewidth").Value);
            retMap.Width = int.Parse(map.Attribute("width").Value);
            
            //Load the map
            foreach (XElement e in map.Elements())
            {
                if (e.Name == "layer")
                {
                    //Load a layer and it's attributes
                    TiledLayer l = new TiledLayer();
                    l.Chunks = new List<TiledLayerChunk>();
                    l.Name = e.Attribute("name").Value;
                    l.Height = int.Parse(e.Attribute("height").Value);
                    l.Width = int.Parse(e.Attribute("width").Value);
                    l.Tiles = new List<int>();
                    l.ParentMap = retMap;

                    //Grab layer data
                    XElement data = e.Element("data");

                    //Ensure CSV format
                    if (data.Attribute("encoding").Value != "csv")
                        throw new Exception("Incompatible map encoding.");
                    
                    if (retMap.Infinite)
                    {
                        //Load chunks
                        foreach (XElement chunk in data.Elements("chunk"))
                        {
                            //Create chunk and load attributes
                            TiledLayerChunk c = new TiledLayerChunk();
                            c.Height = int.Parse(chunk.Attribute("height").Value) * retMap.TileHeight;
                            c.Tiles = new List<int>();
                            c.Width = int.Parse(chunk.Attribute("width").Value) * retMap.TileWidth;
                            c.X = int.Parse(chunk.Attribute("x").Value) * retMap.TileWidth;
                            c.Y = int.Parse(chunk.Attribute("y").Value) * retMap.TileHeight;
                            c.ParentMap = retMap;

                            //Load tiles
                            string[] tiles = chunk.Value.Split(',');
                            foreach (string tile in tiles)
                            {
                                c.Tiles.Add(int.Parse(tile));
                            }

                            //Add to the layer
                            l.Chunks.Add(c);
                        }
                    }
                    else
                    {
                        //Load layer
                        string[] tiles = data.Value.Split(',');
                        foreach (string tile in tiles)
                        {
                            l.Tiles.Add(int.Parse(tile));
                        }
                    }

                    //Add layer to map
                    retMap.Layers.Add(l);
                }
                else if (e.Name == "objectgroup")
                {
                    //Create new object layer
                    TiledObjectLayer l = new TiledObjectLayer();
                    l.Name = e.Attribute("name").Value;
                    l.Objects = new List<TiledObject>();

                    //Add objects
                    foreach (XElement obj in e.Elements("object"))
                    {
                        TiledObject obje = new TiledObject();
                        obje.Height = float.Parse(obj.Attribute("height").Value);
                        obje.ID = int.Parse(obj.Attribute("id").Value);
                        obje.Name = obj.Attribute("name").Value;
                        obje.Type = obj.Attribute("type").Value;
                        obje.Width = float.Parse(obj.Attribute("width").Value);
                        obje.X = float.Parse(obj.Attribute("x").Value);
                        obje.Y = float.Parse(obj.Attribute("y").Value);

                        l.Objects.Add(obje);
                    }

                    //Add layer to map
                    retMap.ObjectLayers.Add(l);
                }
            }

            return retMap;
        }

        /// <summary>
        /// Load a tileset from a Tiled file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Tileset LoadTileset(string filename)
        {
            XDocument doc = XDocument.Load(filename);
            XElement tileset = doc.Element("tileset");

            Tileset retTileset = new Tileset();
            retTileset.Name = tileset.Attribute("name").Value;

            string src = tileset.Element("image").Attribute("source").Value;

            FileStream s = new FileStream(src, FileMode.Open);

            Texture2D Tex = Texture2D.FromStream(Game.Graphics, s);

            s.Dispose();

            retTileset.Source = new Sprite(Tex, new Vector2(0, 0));

            retTileset.TileCount = int.Parse(tileset.Attribute("tilecount").Value);
            retTileset.TileHeight = int.Parse(tileset.Attribute("tileheight").Value);
            retTileset.TileWidth = int.Parse(tileset.Attribute("tilewidth").Value);

            return retTileset;
        }

        /// <summary>
        /// Determine if a map is compatible
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static bool Compatible(TiledVersion v)
        {
            if (v.major < 1) //Not 1.x.x
                return false;
            if (v.minor < 1) //Not 1.0.x
                return false;
            if (v.minor == 1 && v.build < 5) //Not below 1.1.5 (The build I used to code this)
                return false;
            return true;
        }
    }

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
    /// Objects are not supported yet.
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
            foreach (TiledLayer l in Layers)
            {
                l.DrawLayer();
            }
        }
    }

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

    /// <summary>
    /// A layer in a Tiled map containing Objects
    /// </summary>
    public struct TiledObjectLayer
    {
        /// <summary>
        /// The name of the layer
        /// </summary>
        public string Name;

        /// <summary>
        /// The objects in the layer
        /// </summary>
        public List<TiledObject> Objects;
    }

    /// <summary>
    /// This is an object from a Tiled object layer
    /// </summary>
    public struct TiledObject
    {
        /// <summary>
        /// The Object ID
        /// </summary>
        public int ID;

        /// <summary>
        /// The name given to the Object.
        /// </summary>
        public string Name;

        /// <summary>
        /// The type of object.
        /// </summary>
        public string Type;

        /// <summary>
        /// The X coord of the Object
        /// </summary>
        public float X;

        /// <summary>
        /// The Y coord of the Object
        /// </summary>
        public float Y;

        /// <summary>
        /// The Width of the object
        /// </summary>
        public float Width;

        /// <summary>
        /// The Height of the object
        /// </summary>
        public float Height;
    }
}
