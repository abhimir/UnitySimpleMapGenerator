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
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Drawing;
    
	public abstract class MathTools
	{
        /// <summary>
        /// Dist the specified x1, y1, x2 and y2.
        /// </summary>
        /// <param name='x1'>
        /// X1.
        /// </param>
        /// <param name='y1'>
        /// Y1.
        /// </param>
        /// <param name='x2'>
        /// X2.
        /// </param>
        /// <param name='y2'>
        /// Y2.
        /// </param>
		public static double Dist(double x1, double y1, double x2, double y2)
		{
			return (double)Math.Sqrt((x2-x1)*(x2-x1)+(y2-y1)*(y2-y1));
		}

        /// <summary>
        /// Ccw the specified P0x, P0y, P1x, P1y, P2x, P2y and PlusOneOnZeroDegrees.
        /// </summary>
        /// <param name='P0x'>
        /// P0x.
        /// </param>
        /// <param name='P0y'>
        /// P0y.
        /// </param>
        /// <param name='P1x'>
        /// P1x.
        /// </param>
        /// <param name='P1y'>
        /// P1y.
        /// </param>
        /// <param name='P2x'>
        /// P2x.
        /// </param>
        /// <param name='P2y'>
        /// P2y.
        /// </param>
        /// <param name='PlusOneOnZeroDegrees'>
        /// Plus one on zero degrees.
        /// </param>
        public static int ccw(double P0x, double P0y, double P1x, double P1y, double P2x, double P2y, bool PlusOneOnZeroDegrees)
		{
			double dx1, dx2, dy1, dy2;

			dx1 = P1x - P0x;
            dy1 = P1y - P0y;
			dx2 = P2x - P0x;
            dy2 = P2y - P0y;

			if (dx1*dy2 > dy1*dx2) {
                return +1;
            }

			if (dx1*dy2 < dy1*dx2) {
                return -1;
            }

			if ((dx1*dx2 < 0) || (dy1*dy2 < 0)) {
                return -1;
            }

			if ((dx1*dx1+dy1*dy1) < (dx2*dx2+dy2*dy2) && PlusOneOnZeroDegrees) {
				return +1;
            }

			return 0;
		}
    }
}
