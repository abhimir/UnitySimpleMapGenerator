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
        public VDataNode NodeN, NodeL, NodeR;
        public Vector2 Center;

        public override double Y {
            get {
                return Math.Round( Center.y + MathTools.Dist( NodeN.DataPoint.x, NodeN.DataPoint.y, Center.x, Center.y ), 10 );
            }
        }

        public override double X {
            get {
                return Center.x;
            }
        }

        public bool Valid = true;
    }
}
