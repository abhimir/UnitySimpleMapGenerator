namespace BenDi.FortuneVoronoi
{
    using System;

    internal class VDataNode : VNode
    {
        public VDataNode( Vector DP )
        {
            this.DataPoint = DP;
        }

        public Vector DataPoint;
    }
}
