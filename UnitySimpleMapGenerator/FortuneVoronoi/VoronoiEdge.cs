namespace BenDi.FortuneVoronoi
{
    using System;

    using UnityEngine;

    public class VoronoiEdge
    {
        public Vector2 RightData, LeftData;
        public Vector2 VVertexA = Fortune.VVUnkown, VVertexB = Fortune.VVUnkown;

        public void AddVertex( Vector2 V )
        {
            if( VVertexA == Fortune.VVUnkown )
                VVertexA = V;
            else if( VVertexB == Fortune.VVUnkown )
                VVertexB = V;
            else
                throw new Exception( "Tried to add third vertex!" );
        }
    }
}

