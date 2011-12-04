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
        /// <summary>
        /// Initializes a new instance of the <see cref="BenDi.FortuneVoronoi.VEdgeNode"/> class.
        /// </summary>
        /// <param name='E'>
        /// E.
        /// </param>
        /// <param name='Flipped'>
        /// Flipped.
        /// </param>
        public VEdgeNode( VoronoiEdge E, bool Flipped )
        {
            this.Edge = E;
            this.Flipped = Flipped;
        }

        /// <summary>
        /// The edge.
        /// </summary>
        public VoronoiEdge Edge;

        /// <summary>
        /// The flipped.
        /// </summary>
        public bool Flipped;

        /// <summary>
        /// Cut the specified ys and x.
        /// </summary>
        /// <param name='ys'>
        /// Ys.
        /// </param>
        /// <param name='x'>
        /// X.
        /// </param>
        public float Cut( float ys, float x )
        {
            if( !Flipped ) {
                return (float)Math.Round( x - Fortune.ParabolicCut( Edge.LeftData.x, Edge.LeftData.y, Edge.RightData.x, Edge.RightData.y, ys ) );
            }

            return (float)Math.Round( x - Fortune.ParabolicCut( Edge.RightData.x, Edge.RightData.y, Edge.LeftData.x, Edge.LeftData.y, ys ) );
        }
    }
}
