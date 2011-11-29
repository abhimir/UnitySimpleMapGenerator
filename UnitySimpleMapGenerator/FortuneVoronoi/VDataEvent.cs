namespace BenDi.FortuneVoronoi
{
    using System;

    using UnityEngine;

    internal class VDataEvent : VEvent
    {
        public Vector2 DataPoint;

        public VDataEvent( Vector2 DP )
        {
            this.DataPoint = DP;
        }

        public override double Y {
            get {
                return DataPoint.y;
            }
        }

        public override double X {
            get {
                return DataPoint.x;
            }
        }

    }
}
