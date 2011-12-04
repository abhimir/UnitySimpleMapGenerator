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

    using UnityEngine;

    internal class VDataEvent : VEvent
    {
        /// <summary>
        /// The data point.
        /// </summary>
        public Vector2 DataPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="BenDi.FortuneVoronoi.VDataEvent"/> class.
        /// </summary>
        /// <param name='DP'>
        /// D.
        /// </param>
        public VDataEvent( Vector2 DP )
        {
            this.DataPoint = DP;
        }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public override float Y {
            get {
                return DataPoint.y;
            }
        }

        /// <summary>
        /// Gets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public override float X {
            get {
                return DataPoint.x;
            }
        }

    }
}
