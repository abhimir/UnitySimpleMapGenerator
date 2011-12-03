using System;
using System.Collections.Generic;
using System.Windows;

namespace BraveNewWorld.Models
{
    public class Corner : IEquatable<Corner> , IMapItem
    {
        public Corner(double ax, double ay)
        {
            Point = new Point(ax,ay);
            Key = Point.GetHashCode();
            Index = Point.GetHashCode();
            Ocean = Water = Coast = Border = false;
            Moisture = 0.0d;
            Elevation = 100.0d;

            Touches = new HashSet<Center>(new CenterComparer());
            Protrudes = new HashSet<Edge>(new EdgeComparer());
            Adjacents = new HashSet<Corner>(new CornerComparer());

            River = WatershedSize = 0;
        }

        public int Index { get; set; }

        public int Key { get; set; }
        public Point Point { get; set; }  // location
        public Boolean Ocean { get; set; }  // ocean
        public Boolean Water { get; set; }  // lake or ocean
        public Boolean Land { get { return !Water; } set { Water = !value; } }  // lake or ocean
        public Boolean Coast { get; set; }  // touches ocean and land polygons
        public Boolean Border { get; set; }  // at the edge of the map
        public double Elevation { get; set; }  // 0.0-1.0
        public double Moisture { get; set; }  // 0.0-1.0

        public HashSet<Center> Touches { get; set; }
        public HashSet<Edge> Protrudes { get; set; }
        public HashSet<Corner> Adjacents { get; set; }

        public int River { get; set; }  // 0 if no river, or volume of water in river
        public Corner Downslope { get; set; }  // pointer to adjacent corner most downhill
        public Corner Watershed { get; set; }  // pointer to coastal corner, or null
        public int WatershedSize { get; set; }
        
        public bool Equals(Corner other)
        {
            return this.Point.Equals(other.Point);
        }

        public void AddProtrudes(Edge edge)
        {
            Protrudes.Add(edge);
        }

        public void AddAdjacent(Corner corner)
        {
            Adjacents.Add(corner);
        }

        public void AddTouches(Center center)
        {
            Touches.Add(center);
        }
    }
}
