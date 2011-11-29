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

    using UnityEngine;

    public class VoronoiGraph
    {
        public HashSet<Vector2> Vertizes = new HashSet<Vector2>();
        public HashSet<VoronoiEdge> Edges = new HashSet<VoronoiEdge>();
    }
}
