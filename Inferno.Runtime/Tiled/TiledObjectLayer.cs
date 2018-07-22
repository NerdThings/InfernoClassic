﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Tiled
{
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
}
