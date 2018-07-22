using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Tiled
{
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
