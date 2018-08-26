using Inferno.Runtime.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Inferno.Runtime.Tiled
{
    /// <summary>
    /// Simple version struct to aid version checking
    /// </summary>
    public struct TiledVersion
    {
        public int Major;
        public int Minor;
        public int Build;

        public TiledVersion(string str)
        {
            var bits = str.Split('.');
            Major = int.Parse(bits[0]);
            Minor = int.Parse(bits[1]);
            Build = int.Parse(bits[2]);
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
        /// <returns></returns>
        public static TiledMap LoadMap(string filename)
        {
            //Load xml
            var doc = XDocument.Load(filename);

            //Get map element
            var map = doc.Element("map");

            //Version check
            if (map == null)
                throw new NullReferenceException();

            var version = new TiledVersion(map.Attribute("tiledversion")?.Value);
            if (!Compatible(version))
                throw new Exception("Incompatible Tiled version.");

            //Construct map
            var retMap = new TiledMap
            {
                Height = int.Parse(map.Attribute("height")?.Value ?? throw new InvalidOperationException()),
                Infinite = map.Attribute("infinite")?.Value == "1",
                Layers = new List<TiledLayer>(),
                ObjectLayers = new List<TiledObjectLayer>(),
                Tileset = LoadTileset(map.Element("tileset")?.Attribute("source")?.Value)
            };

            //Load the tileset

            //Set the orientation and render order
            var orientation = map.Attribute("orientation")?.Value;
            var renderOrder = map.Attribute("renderorder")?.Value;

            if (orientation == "orthogonal")
                retMap.Orientation = TiledMapOrientation.Orthogonal;
            else
                throw new Exception("Unsupported orientation detected.");

            switch (renderOrder)
            {
                case "right-down":
                    retMap.RenderOrder = RenderOrder.RightDown;
                    break;
                case "right-up":
                    retMap.RenderOrder = RenderOrder.RightUp;
                    break;
                case "left-down":
                    retMap.RenderOrder = RenderOrder.LeftDown;
                    break;
                case "left-up":
                    retMap.RenderOrder = RenderOrder.LeftUp;
                    break;
                default:
                    throw new Exception("Unsupported RenderOrder.");
            }

            //Finish getting attributes
            retMap.TileHeight = int.Parse(map.Attribute("tileheight")?.Value ?? throw new InvalidOperationException());
            retMap.TileWidth = int.Parse(map.Attribute("tilewidth")?.Value ?? throw new InvalidOperationException());
            retMap.Width = int.Parse(map.Attribute("width")?.Value ?? throw new InvalidOperationException());

            //Load the map
            foreach (var e in map.Elements())
            {
                if (e.Name == "layer")
                {
                    //Load a layer and it's attributes
                    var l = new TiledLayer
                    {
                        Chunks = new List<TiledLayerChunk>(),
                        Name = e.Attribute("name")?.Value,
                        Height = int.Parse(e.Attribute("height")?.Value ?? throw new InvalidOperationException()),
                        Width = int.Parse(e.Attribute("width")?.Value ?? throw new InvalidOperationException()),
                        Tiles = new List<int>(),
                        ParentMap = retMap
                    };

                    //Grab layer data
                    var data = e.Element("data");

                    //Ensure CSV format
                    //TODO: Support all formats
                    if (data?.Attribute("encoding")?.Value != "csv")
                        throw new Exception("Incompatible map encoding.");

                    if (retMap.Infinite)
                    {
                        //Load chunks
                        foreach (var chunk in data.Elements("chunk"))
                        {
                            //Create chunk and load attributes
                            var c = new TiledLayerChunk
                            {
                                Height = int.Parse(chunk.Attribute("height")?.Value ?? throw new InvalidOperationException()) * retMap.TileHeight,
                                Tiles = new List<int>(),
                                Width = int.Parse(chunk.Attribute("width")?.Value ?? throw new InvalidOperationException()) * retMap.TileWidth,
                                X = int.Parse(chunk.Attribute("x")?.Value ?? throw new InvalidOperationException()) * retMap.TileWidth,
                                Y = int.Parse(chunk.Attribute("y")?.Value ?? throw new InvalidOperationException()) * retMap.TileHeight,
                                ParentMap = retMap
                            };

                            //Load tiles
                            var tiles = chunk.Value.Split(',');
                            foreach (var tile in tiles)
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
                        var tiles = data.Value.Split(',');
                        foreach (var tile in tiles)
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
                    var l = new TiledObjectLayer
                    {
                        Name = e.Attribute("name")?.Value,
                        Objects = new List<TiledObject>()
                    };

                    //Add objects
                    foreach (var obj in e.Elements("object"))
                    {
                        var obje = new TiledObject
                        {
                            Height = float.Parse(obj.Attribute("height")?.Value ?? throw new InvalidOperationException()),
                            Id = int.Parse(obj.Attribute("id")?.Value ?? throw new InvalidOperationException()),
                            Name = obj.Attribute("name")?.Value,
                            Type = obj.Attribute("type")?.Value,
                            Width = float.Parse(obj.Attribute("width")?.Value ?? throw new InvalidOperationException()),
                            X = float.Parse(obj.Attribute("x")?.Value ?? throw new InvalidOperationException()),
                            Y = float.Parse(obj.Attribute("y")?.Value ?? throw new InvalidOperationException())
                        };

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
            var doc = XDocument.Load(filename);
            var tileset = doc.Element("tileset");

            if (tileset == null)
                throw new NullReferenceException();

            var retTileset = new Tileset {Name = tileset.Attribute("name")?.Value};

            var src = tileset.Element("image")?.Attribute("source")?.Value;

            var s = new FileStream(src ?? throw new InvalidOperationException(), FileMode.Open);

            var tex = Texture2D.FromStream(Game.GraphicsDeviceInstance, s);

            s.Dispose();

            retTileset.Source = new Sprite(tex, new Vector2(0, 0));

            retTileset.TileCount = int.Parse(tileset.Attribute("tilecount")?.Value ?? throw new InvalidOperationException());
            retTileset.TileHeight = int.Parse(tileset.Attribute("tileheight")?.Value ?? throw new InvalidOperationException());
            retTileset.TileWidth = int.Parse(tileset.Attribute("tilewidth")?.Value ?? throw new InvalidOperationException());

            return retTileset;
        }

        /// <summary>
        /// Determine if a map is compatible
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static bool Compatible(TiledVersion v)
        {
            if (v.Major < 1) //Not 1.x.x
                return false;
            if (v.Minor < 1) //Not 1.0.x
                return false;
            return v.Minor != 1 || v.Build >= 5;
        }
    }
}
