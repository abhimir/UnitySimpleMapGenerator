namespace BenDi.FortuneVoronoi
{
    using System;

    public class VoronoiEdge
    {
        public Vector RightData, LeftData;
        public Vector VVertexA = Fortune.VVUnkown, VVertexB = Fortune.VVUnkown;

        public void AddVertex( Vector V )
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

