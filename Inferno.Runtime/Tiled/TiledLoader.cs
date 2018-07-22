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
            retMap.Infinite = map.Attribute("infinite").Value == "1" ? true : false;
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
}
