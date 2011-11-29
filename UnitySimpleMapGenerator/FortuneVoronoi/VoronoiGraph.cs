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
