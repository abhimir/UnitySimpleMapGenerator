using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;
using BraveNewWorld.Models;

namespace BraveNewWorld.Services
{
    public interface IIslandService
    {
        void CreateIsland();
    }

    public class IslandService : IIslandService
    {
        private double MapMaxHeight = 0.0d;
        private double MapDeepest = 1.0d;
        private int MapX, MapY;
        

        public IslandService(int mapX, int mapY)
        {
            MapX = mapX;
            MapY = mapY;
        }

        public void CreateIsland()
        {
            foreach (var c in App.AppMap.Corners.Values)
            {
                c.Water = !InLand(c.Point); // calculate land&water corners
            }

            FixCentersFloodFillOceans();

            foreach (var c in App.AppMap.Corners.Values)
            {
                c.Coast = (c.Touches.Any(x => x.Water) && c.Touches.Any(x => x.Land)) ? true : false;

                c.Water = (c.Touches.Any(x => x.Land)) ? false : true;

                c.Ocean = (c.Touches.All(x => x.Ocean)) ? true : false;
            }

            CalculateElevation();

            RedistributeElevation();

            CalculateDownslopes();

            CalculateWatersheds();

            CreateRivers();

            CalculateCornerMoisture();

            //RedistributeMoisture();

            

            //Smooth1();


            foreach (Center c in App.AppMap.Centers.Values)
            {
                c.Moisture = c.Corners.Sum(x => x.Moisture) / c.Corners.Count();

                c.OrderCorners();

                c.Elevation = c.Corners.Sum(x => x.Elevation) / c.Corners.Count;

                c.SetBiome();
            }
        }

        private void RedistributeMoisture()
        {
            var locations = App.AppMap.Corners.Values.OrderBy(x => x.Moisture).ToArray();

            for (int i = 0; i < locations.Count(); i++) 
            {
                locations[i].Moisture = (float) i/(locations.Count() - 1);
            }
        }

        private void CalculateCornerMoisture()
        {
            var queue = new Queue<Corner>();

            foreach (var q in App.AppMap.Corners.Values) 
            {
                if ((q.Water || q.River > 0) && !q.Ocean) 
                {
                    q.Moisture = q.River > 0 ? Math.Min(3.0, (0.2 * q.River)) : 1.0;
                    queue.Enqueue(q);
                } 
                else 
                {
                    q.Moisture = 0.0;
                }
            }

            while (queue.Count > 0) 
            {
                var q = queue.Dequeue();

                foreach (var r in q.Adjacents) 
                {
                    var newMoisture = q.Moisture * 0.9;
                    if (newMoisture > r.Moisture) 
                    {
                        r.Moisture = newMoisture;
                        queue.Enqueue(r);
                    }
                }
            }

            foreach (var q in App.AppMap.Corners.Values) 
            {
                if (q.Ocean || q.Coast) 
                {
                    q.Moisture = 1.0;
                }
            }
        }

        private void CreateRivers()
        {
            for (int i = 0; i < MapX / 2; i++)
            {
                Corner q = App.AppMap.Corners.Values.ElementAt(App.Random.Next(0, App.AppMap.Corners.Values.Count - 1));
                
                if (q.Ocean || q.Elevation < 0.3 || q.Elevation > 0.9) continue;
                
                while (!q.Coast)
                {
                    if (q == q.Downslope)
                    {
                        break;
                    }

                    Edge edge = q.Protrudes.FirstOrDefault(ed => ed.VoronoiStart == q.Downslope || ed.VoronoiEnd == q.Downslope);
                    edge.River = edge.River + 1;
                    q.River = q.River + 1;
                    q.Downslope.River = q.Downslope.River + 1; 
                    q = q.Downslope;
                }
            }
        }

        private void CalculateWatersheds()
        {
            
            foreach (var q in App.AppMap.Corners.Values) 
            {
                q.Watershed = q;
                
                if (!q.Ocean && !q.Coast) 
                {
                    q.Watershed = q.Downslope;
                }
            }
            
            for (int i = 0; i < 100; i++) 
            {
                var changed = false;

                foreach (var q in App.AppMap.Corners.Values) 
                {
                    if (!q.Ocean && !q.Coast && !q.Watershed.Coast) 
                    {
                        var r = q.Downslope.Watershed;
                        
                        if (!r.Ocean) 
                            q.Watershed = r;
                        
                        changed = true;
                    }
                }

                if (!changed) 
                    break;
            }

            foreach (var q in App.AppMap.Corners.Values) 
            {
                var r = q.Watershed;
                r.WatershedSize = 1 + r.WatershedSize;
            }

        }

        private void CalculateDownslopes()
        {
            foreach (Corner corner in App.AppMap.Corners.Values) 
            {
                var buf = corner;
              
                foreach (var adj in corner.Adjacents) 
                {
                    if (adj.Elevation <= buf.Elevation) 
                    {
                        buf = adj;
                    }
                }

                corner.Downslope = buf;
            }
        }

        private void RedistributeElevation()
        {
            double scaleFactor = 1.1;
            var locations = App.AppMap.Corners.Values.Where(x => !x.Ocean).OrderBy(x => x.Elevation).ToArray();

            for (int i = 0; i < locations.Count(); i++)
            {
                double y = (double)i / (locations.Count() - 1);

                var x = 1.04880885 - Math.Sqrt(scaleFactor * (1 - y));
                if (x > 1.0) 
                    x = 1.0;  
                locations[i].Elevation = x;
            }
        }

        private void CalculateElevation()
        {
            var queue = new Queue<Corner>();

            foreach (var q in App.AppMap.Corners.Values) 
            {
              if (q.Border) 
              {
                q.Elevation = 0.0;
                queue.Enqueue(q);
              } 
              else 
              {
                q.Elevation = double.MaxValue;
              }
            }

            while (queue.Count > 0) 
            {
                var corner = queue.Dequeue();

                foreach (var adj in corner.Adjacents) 
                {
                    double newElevation = 0.01 + corner.Elevation;
                    
                    if (!corner.Water && !adj.Water) 
                    {
                        newElevation += 1;
                    }
                    
                    if (newElevation < adj.Elevation) 
                    {
                        adj.Elevation = newElevation;
                        queue.Enqueue(adj);
                    }
                }
             }


        }


        private void Smooth1()
        {
            var ordered = new List<Corner>();
            var first = App.AppMap.Corners.Values.First(x => x.Coast && x.Touches.Any(z=>z.Ocean));
            var start = first;
            ordered.Add(first);
            var next = first.Adjacents.First(x => x.Coast);

            while (next != start)
            {
                var nexte = next.Protrudes.FirstOrDefault(x => x.Coast && (x.VoronoiStart != ordered.Last() && x.VoronoiEnd != ordered.Last()));
                ordered.Add(next);
                next = nexte.VoronoiStart == next ? nexte.VoronoiEnd : nexte.VoronoiStart;
            }

            for (int a = 0; a < 2; a++)
            {
                for (int i = 2; i < ordered.Count - 2; i++)
                {
                    ordered[i].Point = PointOnCurve(ordered[i - 2].Point, ordered[i - 1].Point, ordered[i + 1].Point,
                                                 ordered[i + 2].Point, 0.5f);
                }
            }
        }

        public Point PointOnCurve(Point p0, Point p1, Point p2, Point p3, float t)
        {
            Point ret = new Point();
            
            float t2 = t * t;
            float t3 = t2 * t;

            ret.X = 0.5f * ((2.0f * p1.X) +
            (-p0.X + p2.X) * t +
            (2.0f * p0.X - 5.0f * p1.X + 4 * p2.X - p3.X) * t2 +
            (-p0.X + 3.0f * p1.X - 3.0f * p2.X + p3.X) * t3);

            ret.Y = 0.5f * ((2.0f * p1.Y) +
            (-p0.Y + p2.Y) * t +
            (2.0f * p0.Y - 5.0f * p1.Y + 4 * p2.Y - p3.Y) * t2 +
            (-p0.Y + 3.0f * p1.Y - 3.0f * p2.Y + p3.Y) * t3);

            return ret;
        }

        private void FixCentersFloodFillOceans()
        {
            foreach (var ct in App.AppMap.Centers.Values)
            {
                ct.FixBorders(); //Fix edges at map border , set "border" and "ocean" values
                ct.OrderCorners(); //Order corners clockwise as we'Ll need it for polygons and 3d stuff

                //if it touches any water corner , it's water ; there will be leftovers tho
                ct.Water = (ct.Corners.Any(x => x.Water)) ? true : false;
            }

            var Oceans = new Queue<Center>();
            //start with oceans at the borders
            foreach (Center c in App.AppMap.Centers.Values.Where(c => c.Ocean))
            {
                Oceans.Enqueue(c);
            }

            //floodfill oceans
            while (Oceans.Count > 0)
            {
                Center c = Oceans.Dequeue();

                foreach (Center n in c.Neighbours.Where(x => !x.Ocean))
                {
                    if (n.Corners.Any(x => x.Water))
                    {
                        n.Ocean = true;
                        if (!Oceans.Contains(n))
                            Oceans.Enqueue(n);
                    }
                    else
                    {
                        n.Coast = true;
                    }
                }
            }
        }

        private bool InLand(Point p)
        {
            return IsLandShape(new Point(2 * (p.X / MapX - 0.5), 2 * (p.Y / MapY - 0.5)));
        }

        private bool IsLandShape(Point point)
        {
            double ISLAND_FACTOR = 1.07;
            Random islandRandom = new Random();
            int bumps = islandRandom.Next(1, 6);
            double startAngle = islandRandom.NextDouble() * 2 * Math.PI;
            double dipAngle = islandRandom.NextDouble() * 2 * Math.PI;
            double dipWidth = islandRandom.Next(2, 7) / 10;

            double angle = Math.Atan2(point.Y, point.X);
            double length = 0.5 * (Math.Max(Math.Abs(point.X), Math.Abs(point.Y)) + GetPointLength(point));

            double r1 = 0.5 + 0.40 * Math.Sin(startAngle + bumps * angle + Math.Cos((bumps + 3) * angle));
            double r2 = 0.7 - 0.20 * Math.Sin(startAngle + bumps * angle - Math.Sin((bumps + 2) * angle));
            if (Math.Abs(angle - dipAngle) < dipWidth
                || Math.Abs(angle - dipAngle + 2 * Math.PI) < dipWidth
                || Math.Abs(angle - dipAngle - 2 * Math.PI) < dipWidth)
            {
                r1 = r2 = 0.2;
            }
            return (length < r1 || (length > r1 * ISLAND_FACTOR && length < r2));
        }

        private double GetPointLength(Point point)
        {
            return Math.Sqrt((Math.Pow(point.X, 2)) + (Math.Pow(point.Y, 2)));
        }

    }
}
