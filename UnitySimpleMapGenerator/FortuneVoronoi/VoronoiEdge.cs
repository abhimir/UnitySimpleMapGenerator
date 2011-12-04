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

    public class VoronoiEdge
    {
        /// <summary>
        /// The right data.
        /// </summary>
        public Vector2 RightData;

        /// <summary>
        /// The left data.
        /// </summary>
        public Vector2 LeftData;

        /// <summary>
        /// The V vertex a.
        /// </summary>
        public Vector2 VVertexA = Fortune.VVUnkown;

        /// <summary>
        /// The V vertex b.
        /// </summary>
        public Vector2 VVertexB = Fortune.VVUnkown;

        /// <summary>
        /// Adds the vertex.
        /// </summary>
        /// <param name='V'>
        /// V.
        /// </param>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public void AddVertex( Vector2 V )
        {
            if( VVertexA == Fortune.VVUnkown ) {
                VVertexA = V;
            }
            else if( VVertexB == Fortune.VVUnkown ) {
                VVertexB = V;
            }
            else {
                throw new Exception( "Tried to add third vertex!" );
            }
        }
    }
}

