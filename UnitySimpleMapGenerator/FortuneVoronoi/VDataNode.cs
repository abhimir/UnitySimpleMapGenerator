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

    using NGenerics.DataStructures.Mathematical;

    internal class VDataNode : VNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BenDi.FortuneVoronoi.VDataNode"/> class.
        /// </summary>
        /// <param name='DP'>
        /// D.
        /// </param>
        public VDataNode( Vector2D DP )
        {
            this.DataPoint = DP;
        }

        /// <summary>
        /// The data point.
        /// </summary>
        public Vector2D DataPoint;
    }
}
