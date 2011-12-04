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

    internal abstract class VEvent : IComparable
    {
        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public abstract float Y { get; }

        /// <summary>
        /// Gets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public abstract float X { get; }

        #region IComparable Members

        /// <summary>
        /// Compares to a VEvent.
        /// </summary>
        /// <returns>
        /// The to.
        /// </returns>
        /// <param name='obj'>
        /// Object.
        /// </param>
        /// <exception cref='ArgumentException'>
        /// Is thrown when an argument passed to a method is invalid.
        /// </exception>
        public int CompareTo( object obj )
        {
            VEvent vObj = obj as VEvent;

            if( vObj == null ) {
                throw new ArgumentException( "obj not VEvent!" );
            }

            int i = Y.CompareTo( vObj.Y );
            if( i != 0 ) {
                return i;
            }

            return X.CompareTo( vObj.X );
        }

     #endregion
    }
}
