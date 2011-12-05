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
        public Vector2D Center;

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
        public override double Y {
            get {
                return Math.Round( Center.Y + MathTools.Dist( NodeN.DataPoint.X, NodeN.DataPoint.Y, Center.X, Center.Y ), 10 );
            }
        }

        /// <summary>
        /// Gets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public override double X {
            get {
                return Center.X;
            }
        }
    }
}
