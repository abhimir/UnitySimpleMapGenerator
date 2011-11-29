namespace BenDi.FortuneVoronoi
{
    using System;

    internal abstract class VEvent : IComparable
    {
        public abstract double Y { get; }

        public abstract double X { get; }

        #region IComparable Members

        public int CompareTo( object obj )
        {
            if( !( obj is VEvent ) )
                throw new ArgumentException( "obj not VEvent!" );
            int i = Y.CompareTo( ( (VEvent)obj ).Y );
            if( i != 0 )
                return i;
            return X.CompareTo( ( (VEvent)obj ).X );
        }

     #endregion
    }
}
