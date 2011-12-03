using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BenTools.Mathematics;
using BraveNewWorld.Models;
using Vector = BenTools.Mathematics.Vector;

namespace BraveNewWorld.Services
{
    //do I need this? meh
    public class LoadMapParams
    {
        public IEnumerable<Vector> Points;
        public bool Fix;
        public int MapX;
        public int MapY;

        public LoadMapParams(IEnumerable<Vector> points, bool fix = false)
        {
            Points = points;
            Fix = fix;
        }

        public LoadMapParams(IEnumerable<Vector> points, bool fix, int mapX, int mapY)
        {
            Points = points;
            Fix = fix;
            MapX = mapX;
            MapY = mapY;
        }
    }

    public interface IMapService
    {
        void LoadMap(LoadMapParams loadMapParams);
    }

    public class MapService : IMapService
    {
        private int MapX, MapY;
        private IIslandService IslandHandler;

        public void LoadMap(LoadMapParams loadMapParams)
        {
            MapX = loadMapParams.MapX;
            MapY = loadMapParams.MapY;
            IslandHandler = new IslandService(MapX, MapY);

            VoronoiGraph voronoiMap = null;
            for (int i = 0; i < 3; i++)
            {
                voronoiMap = Fortune.ComputeVoronoiGraph(loadMapParams.Points);
                foreach (Vector vector in loadMapParams.Points)
                {
                    double v0 = 0.0d;
                    double v1 = 0.0d;
                    int say = 0;
                    foreach (VoronoiEdge edge in voronoiMap.Edges)
                    {
                        if (edge.LeftData == vector || edge.RightData == vector)
                        {
                            double p0 = (edge.VVertexA[0] + edge.VVertexB[0]) / 2;
                            double p1 = (edge.VVertexA[1] + edge.VVertexB[1]) / 2;
                            v0 += double.IsNaN(p0) ? 0 : p0;
                            v1 += double.IsNaN(p1) ? 0 : p1;
                            say++;
                        }
                    }

                    if (((v0 / say) < MapX) && ((v0 / say) > 0))
                    {
                        vector[0] = v0 / say;
                    }

                    if (((v1 / say) < MapY) && ((v1 / say) > 0))
                    {
                        vector[1] = v1 / say;
                    }
                }
            }

            

            voronoiMap = Fortune.ComputeVoronoiGraph(loadMapParams.Points);
            ImproveMapData(voronoiMap, loadMapParams.Fix);
        }

        private void ImproveMapData(VoronoiGraph voronoiMap, bool fix = false)
        {
            IFactory fact = new MapItemFactory();

            foreach (VoronoiEdge edge in voronoiMap.Edges)
            {
                if (fix)
                {
                    if (!FixPoints(edge))
                        continue;
                }

                Corner c1 = fact.CornerFactory(edge.VVertexA[0], edge.VVertexA[1]);
                Corner c2 = fact.CornerFactory(edge.VVertexB[0], edge.VVertexB[1]);
                Center cntrLeft = fact.CenterFactory(edge.LeftData[0], edge.LeftData[1]);
                Center cntrRight = fact.CenterFactory(edge.RightData[0], edge.RightData[1]);

                if(c1 == null || c2 == null)
                {
                    
                }

                c1.AddAdjacent(c2);
                c2.AddAdjacent(c1);

                cntrRight.Corners.Add(c1);
                cntrRight.Corners.Add(c2);

                cntrLeft.Corners.Add(c1);
                cntrLeft.Corners.Add(c2);

                Edge e = fact.EdgeFactory(c1, c2, cntrLeft, cntrRight);


                cntrLeft.Borders.Add(e);
                cntrRight.Borders.Add(e);

                cntrLeft.Neighbours.Add(cntrRight);
                cntrRight.Neighbours.Add(cntrLeft);

                c1.AddProtrudes(e);
                c2.AddProtrudes(e);
                c1.AddTouches(cntrLeft);
                c1.AddTouches(cntrRight);
                c2.AddTouches(cntrLeft);
                c2.AddTouches(cntrRight);
            }

            foreach (Corner q in App.AppMap.Corners.Values)
            {
                if (!q.Border)
                {
                    var point = new Point(0, 0);
                    foreach (Center c in q.Touches)
                    {
                        point.X += c.Point.X;
                        point.Y += c.Point.Y;
                    }
                    point.X = point.X / q.Touches.Count;
                    point.Y = point.Y / q.Touches.Count;
                    q.Point = point;
                }
            }

            //foreach (var c in App.AppMap.Centers)
            //{
            //    c.Value.FixBorders();
            //    c.SetEdgeAreas();
            //    c.Value.OrderCorners();
            //}

            IslandHandler.CreateIsland();
        }

        private bool FixPoints(VoronoiEdge edge)
        {
            double x1 = edge.VVertexA[0];
            double y1 = edge.VVertexA[1];

            double x2 = edge.VVertexB[0];
            double y2 = edge.VVertexB[1];



            //if both ends are in map, not much to do
            if ((DotInMap(x1, y1) && DotInMap(x2, y2)))
                return true;
            
            //if one end is out of map
            if ((DotInMap(x1, y1) && !DotInMap(x2, y2)) || (!DotInMap(x1, y1) && DotInMap(x2, y2)))
            {
                double b = 0.0d, slope = 0.0d;

                //and that point is actually a number ( not going to infinite ) 
                if (!(double.IsNaN(x2) || double.IsNaN(y2)))
                {
                    slope = ((y2 - y1) / (x2 - x1));

                    b = edge.VVertexA[1] - (slope * edge.VVertexA[0]);

                    // y = ( slope * x ) + b


                    if (edge.VVertexA[0] < 0)
                        edge.VVertexA = new Vector(0, b);

                    if (edge.VVertexA[0] > App.MapSize)
                        edge.VVertexA = new Vector(App.MapSize, (App.MapSize * slope) + b);

                    if (edge.VVertexA[1] < 0)
                        edge.VVertexA = new Vector((-b / slope), 0);

                    if (edge.VVertexA[1] > App.MapSize)
                        edge.VVertexA = new Vector((App.MapSize - b) / slope, App.MapSize);



                    if (edge.VVertexB[0] < 0)
                        edge.VVertexB = new Vector(0, b);

                    if (edge.VVertexB[0] > App.MapSize)
                        edge.VVertexB = new Vector(App.MapSize, (App.MapSize * slope) + b);

                    if (edge.VVertexB[1] < 0)
                        edge.VVertexB = new Vector((-b / slope), 0);

                    if (edge.VVertexB[1] > App.MapSize)
                        edge.VVertexB = new Vector((App.MapSize - b) / slope, App.MapSize);

                }
                else
                {
                    //and if that end is actually not a number ( going to infinite )
                    if (double.IsNaN(x2) || double.IsNaN(y2))
                    {
                        var x3 = (edge.LeftData[0] + edge.RightData[0]) / 2;
                        var y3 = (edge.LeftData[1] + edge.RightData[1]) / 2;

                        slope = ((y3 - y1) / (x3 - x1));

                        slope = Math.Abs(slope);

                        b = edge.VVertexA[1] - (slope * edge.VVertexA[0]);

                        // y = ( slope * x ) + b
                        var i = 0.0d;

                        if(x3 < y3)
                        {
                            if(App.MapSize - x3 > y3)
                            {
                                i = b;
                                if (i > 0 && i < MapY)
                                    edge.VVertexB = new Vector(0, i);

                            }
                            else
                            {
                                i = (MapX - b) / slope;
                                if (i > 0 && i < MapY)
                                    edge.VVertexB = new Vector(i, MapY);

                            }
                        }
                        else
                        {
                            if (MapX - x3 > y3)
                            {
                                i = (-b / slope);
                                if (i > 0 && i < MapX)
                                    edge.VVertexB = new BenTools.Mathematics.Vector(i, 0);
                            }
                            else
                            {
                                i = (MapY * slope) + b;
                                if (i > 0 && i < MapX)
                                    edge.VVertexB = new BenTools.Mathematics.Vector(MapX, i);

                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private bool DotInMap(double x, double y)
        {
            if (x == double.NaN || y == double.NaN)
                return false;

            return (x > 0 && x < MapX) && (y > 0 && y < MapY);
        }

    }
}
