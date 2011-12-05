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

    using NGenerics.DataStructures.Mathematical;
    using NGenerics.DataStructures.Queues;

	// VoronoiVertex or VoronoiDataPoint are represented as Vector

	public abstract class Fortune
	{
        /// <summary>
     /// The VV infinite.
     /// </summary>
		public static readonly Vector2D VVInfinite = new Vector2D(double.PositiveInfinity, double.PositiveInfinity);
        /// <summary>
     /// The VV unkown.
     /// </summary>
		public static readonly Vector2D VVUnkown = new Vector2D(double.NaN, double.NaN);

        internal static double ParabolicCut(double x1, double y1, double x2, double y2, double ys)
		{
			if(Math.Abs(x1-x2)<1e-10 && Math.Abs(y1-y2)<1e-10)
			{
				throw new Exception("Identical datapoints are not allowed!");
			}

			if(Math.Abs(y1-ys)<1e-10 && Math.Abs(y2-ys)<1e-10) {
				return (x1+x2)/2;
            }

            if(Math.Abs(y1-ys)<1e-10) {
				return x1;
            }

			if(Math.Abs(y2-ys)<1e-10) {
				return x2;
            }

			double a1 = 1/(2*(y1-ys));
			double a2 = 1/(2*(y2-ys));
			if(Math.Abs(a1-a2)<1e-10) {
				return (x1+x2)/2;
            }

            double xs1 = (double)(0.5/(2*a1-2*a2)*(4*a1*x1-4*a2*x2+2*Math.Sqrt(-8*a1*x1*a2*x2-2*a1*y1+2*a1*y2+4*a1*a2*x2*x2+2*a2*y1+4*a2*a1*x1*x1-2*a2*y2)));
			double xs2 = (double)(0.5/(2*a1-2*a2)*(4*a1*x1-4*a2*x2-2*Math.Sqrt(-8*a1*x1*a2*x2-2*a1*y1+2*a1*y2+4*a1*a2*x2*x2+2*a2*y1+4*a2*a1*x1*x1-2*a2*y2)));
			xs1=(double)Math.Round(xs1,10);
			xs2=(double)Math.Round(xs2,10);
			if(xs1>xs2)
			{
				double h = xs1;
				xs1=xs2;
				xs2=h;
			}

			if(y1>=y2) {
				return xs2;
            }

            return xs1;
		}

        internal static Vector2D CircumCircleCenter(Vector2D A, Vector2D B, Vector2D C)
		{
			if(A==B || B==C || A==C) {
				throw new Exception("Need three different points!");
            }

            double tx = (A.X + C.X)/2;
			double ty = (A.Y + C.Y)/2;

			double vx = (B.X + C.X)/2;
			double vy = (B.Y + C.Y)/2;

			double ux,uy,wx,wy;
			
			if(A.X == C.X)
			{
				ux = 1;
				uy = 0;
			}
			else
			{
				ux = (C.Y - A.Y)/(A.X - C.X);
				uy = 1;
			}

			if(B.X == C.X)
			{
				wx = -1;
				wy = 0;
			}
			else
			{
				wx = (B.Y - C.Y)/(B.X - C.X);
				wy = -1;
			}

			double alpha = (wy*(vx-tx)-wx*(vy - ty))/(ux*wy-wx*uy);

			return new Vector2D(tx+alpha*ux,ty+alpha*uy);
		}

        /// <summary>
     /// Computes the voronoi graph.
     /// </summary>
     /// <returns>
     /// The voronoi graph.
     /// </returns>
     /// <param name='Datapoints'>
     /// Datapoints.
     /// </param>
     /// <exception cref='Exception'>
     /// Represents errors that occur during application execution.
     /// </exception>
		public static VoronoiGraph ComputeVoronoiGraph(IEnumerable Datapoints)
		{
            PriorityQueue<VEvent> PQ = new PriorityQueue<VEvent>(PriorityQueueType.Minimum);
			Hashtable CurrentCircles = new Hashtable();
			VoronoiGraph VG = new VoronoiGraph();
			VNode RootNode = null;

			foreach(Vector2D V in Datapoints)
			{
				PQ.Enqueue(new VDataEvent(V));
			}

			while(PQ.Count>0)
			{
				VEvent VE = PQ.Dequeue() as VEvent;
				VDataNode[] CircleCheckList;

				if(VE is VDataEvent)
				{
					RootNode = VNode.ProcessDataEvent(VE as VDataEvent,RootNode,VG,VE.Y,out CircleCheckList);
				}
				else if(VE is VCircleEvent)
				{
					CurrentCircles.Remove(((VCircleEvent)VE).NodeN);
					if(!((VCircleEvent)VE).Valid)
						continue;
					RootNode = VNode.ProcessCircleEvent(VE as VCircleEvent,RootNode,VG,VE.Y,out CircleCheckList);
				}
				else {
                    throw new Exception("Got event of type "+VE.GetType().ToString()+"!");
                }

				foreach(VDataNode VD in CircleCheckList)
				{
					if(CurrentCircles.ContainsKey(VD))
					{
						((VCircleEvent)CurrentCircles[VD]).Valid=false;
						CurrentCircles.Remove(VD);
					}

					VCircleEvent VCE = VNode.CircleCheckDataNode(VD,VE.Y);
					if(VCE!=null)
					{
						PQ.Enqueue(VCE);
						CurrentCircles[VD]=VCE;
					}
				}

				if(VE is VDataEvent)
				{
					Vector2D DP = ((VDataEvent)VE).DataPoint;
					foreach(VCircleEvent VCE in CurrentCircles.Values)
					{
						if(MathTools.Dist(DP.X,DP.Y,VCE.Center.X,VCE.Center.Y)<VCE.Y-VCE.Center.Y && Math.Abs(MathTools.Dist(DP.X,DP.Y,VCE.Center.X,VCE.Center.Y)-(VCE.Y-VCE.Center.Y))>1e-10) {
							VCE.Valid = false;
                        }
					}
				}
			}

            return VG;
		}
	}
}
