namespace BenDi.FortuneVoronoi
{
    using System;

    internal class VDataEvent : VEvent
    {
        public Vector DataPoint;

        public VDataEvent( Vector DP )
        {
            this.DataPoint = DP;
        }

        public override double Y {
            get {
                return DataPoint[1];
            }
        }

        public override double X {
            get {
                return DataPoint[0];
            }
        }

    }
}
