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

    internal class VEdgeNode : VNode
    {
        public VEdgeNode( VoronoiEdge E, bool Flipped )
        {
            this.Edge = E;
            this.Flipped = Flipped;
        }

        public VoronoiEdge Edge;
        public bool Flipped;

        public double Cut( double ys, double x )
        {
            if( !Flipped )
                return Math.Round( x - Fortune.ParabolicCut( Edge.LeftData[0], Edge.LeftData[1], Edge.RightData[0], Edge.RightData[1], ys ), 10 );
            return Math.Round( x - Fortune.ParabolicCut( Edge.RightData[0], Edge.RightData[1], Edge.LeftData[0], Edge.LeftData[1], ys ), 10 );
        }
    }
}
