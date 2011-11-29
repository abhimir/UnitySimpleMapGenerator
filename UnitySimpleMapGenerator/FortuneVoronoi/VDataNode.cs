namespace BenDi.FortuneVoronoi
{
    using System;

    using UnityEngine;

    internal class VDataNode : VNode
    {
        public VDataNode( Vector2 DP )
        {
            this.DataPoint = DP;
        }

        public Vector2 DataPoint;
    }
}
