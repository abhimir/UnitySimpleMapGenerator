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

    internal class VCircleEvent : VEvent
    {
        /// <summary>
        /// The node n.
        /// </summary>
        public VDataNode NodeN;

        /// <summary>
        /// The node l.
        /// </summary>
        public VDataNode NodeL;

        /// <summary>
        /// The node r.
        /// </summary>
        public VDataNode NodeR;

        /// <summary>
        /// The center.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// The valid.
        /// </summary>
        public bool Valid = true;

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public override float Y {
            get {
                return Mathf.Round( Center.y + MathTools.Dist( NodeN.DataPoint.x, NodeN.DataPoint.y, Center.x, Center.y ) );
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
                return Center.x;
            }
        }
    }
}
