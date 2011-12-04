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

    using NGenerics.DataStructures.Queues;
    using UnityEngine;

	// VoronoiVertex or VoronoiDataPoint are represented as Vector

	public abstract class Fortune
	{
        /// <summary>
     /// The VV infinite.
     /// </summary>
		public static readonly Vector2 VVInfinite = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        /// <summary>
     /// The VV unkown.
     /// </summary>
		public static readonly Vector2 VVUnkown = new Vector2(float.NaN, float.NaN);

        internal static float ParabolicCut(float x1, float y1, float x2, float y2, float ys)
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

			float a1 = 1/(2*(y1-ys));
			float a2 = 1/(2*(y2-ys));
			if(Math.Abs(a1-a2)<1e-10) {
				return (x1+x2)/2;
            }

            float xs1 = (float)(0.5/(2*a1-2*a2)*(4*a1*x1-4*a2*x2+2*Math.Sqrt(-8*a1*x1*a2*x2-2*a1*y1+2*a1*y2+4*a1*a2*x2*x2+2*a2*y1+4*a2*a1*x1*x1-2*a2*y2)));
			float xs2 = (float)(0.5/(2*a1-2*a2)*(4*a1*x1-4*a2*x2-2*Math.Sqrt(-8*a1*x1*a2*x2-2*a1*y1+2*a1*y2+4*a1*a2*x2*x2+2*a2*y1+4*a2*a1*x1*x1-2*a2*y2)));
			xs1=(float)Math.Round(xs1,10);
			xs2=(float)Math.Round(xs2,10);
			if(xs1>xs2)
			{
				float h = xs1;
				xs1=xs2;
				xs2=h;
			}

			if(y1>=y2) {
				return xs2;
            }

            return xs1;
		}

        internal static Vector2 CircumCircleCenter(Vector2 A, Vector2 B, Vector2 C)
		{
			if(A==B || B==C || A==C) {
				throw new Exception("Need three different points!");
            }

            float tx = (A.x + C.x)/2;
			float ty = (A.y + C.y)/2;

			float vx = (B.x + C.x)/2;
			float vy = (B.y + C.y)/2;

			float ux,uy,wx,wy;
			
			if(A.x == C.x)
			{
				ux = 1;
				uy = 0;
			}
			else
			{
				ux = (C.y - A.y)/(A.x - C.x);
				uy = 1;
			}

			if(B.x == C.x)
			{
				wx = -1;
				wy = 0;
			}
			else
			{
				wx = (B.y - C.y)/(B.x - C.x);
				wy = -1;
			}

			float alpha = (wy*(vx-tx)-wx*(vy - ty))/(ux*wy-wx*uy);

			return new Vector2(tx+alpha*ux,ty+alpha*uy);
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

			foreach(Vector2 V in Datapoints)
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
					Vector2 DP = ((VDataEvent)VE).DataPoint;
					foreach(VCircleEvent VCE in CurrentCircles.Values)
					{
						if(MathTools.Dist(DP.x,DP.y,VCE.Center.x,VCE.Center.y)<VCE.Y-VCE.Center.y && Math.Abs(MathTools.Dist(DP.x,DP.y,VCE.Center.x,VCE.Center.y)-(VCE.Y-VCE.Center.y))>1e-10) {
							VCE.Valid = false;
                        }
					}
				}
			}

            return VG;
		}
	}
}
