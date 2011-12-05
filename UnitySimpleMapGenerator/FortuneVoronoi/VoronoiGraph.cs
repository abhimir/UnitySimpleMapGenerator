// Original code by BenDi on CodeProject:
//
// http://www.codeproject.com/KB/recipes/fortunevoronoi.aspx
//
// Cleaned up and adapted by Taffer for UnitySimpleMapGenerator on Github:
//
// https://github.com/Taffer/UnitySimpleMapGenerator

namespace BenDi.FortuneVoronoi
{
    using System;
    using System.Collections.Generic;

    using NGenerics.DataStructures.Mathematical;

    public class VoronoiGraph
    {
        /// <summary>
        /// The vertizes.
        /// </summary>
        public HashSet<Vector2D> Vertizes = new HashSet<Vector2D>();
        
        /// <summary>
        /// The edges.
        /// </summary>
        public HashSet<VoronoiEdge> Edges = new HashSet<VoronoiEdge>();
    }
}
