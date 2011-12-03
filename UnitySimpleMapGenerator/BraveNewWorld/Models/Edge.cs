using System;
using System.Windows;

namespace BraveNewWorld.Models
{
    public class Edge : IEquatable<Edge> , IMapItem
    {
        public Edge(Corner begin, Corner end, Center left, Center right)
        {
            River = 0;
            VoronoiStart = begin;
            VoronoiEnd = end;

            DelaunayStart = left;
            DelaunayEnd = right;

            Midpoint = new Point((VoronoiStart.Point.X + VoronoiEnd.Point.X) / 2, (VoronoiStart.Point.Y + VoronoiEnd.Point.Y) / 2);
            Key = Midpoint.GetHashCode();
            Index = Midpoint.GetHashCode();
        }

        public Edge(int index, Corner begin, Corner end)
        {
            Index = index;
            River = 0;
            VoronoiStart = begin;
            VoronoiEnd = end;
            Midpoint = new Point((VoronoiStart.Point.X + VoronoiEnd.Point.X) / 2, (VoronoiStart.Point.Y + VoronoiEnd.Point.Y) / 2);
            Key = Midpoint.GetHashCode();
        }

        public int Index { get; set; }

        public int Key { get; set; }
        public Center DelaunayStart { get; set; }
        public Center DelaunayEnd { get; set; }// Delaunay edge
        public Corner VoronoiStart { get; set; }
        public Corner VoronoiEnd { get; set; }// Voronoi edge
        public Point Midpoint { get; set; }  // halfway between v0,v1
        public int River { get; set; }  // volume of water, or 0
        public bool MapEdge = false;
        public Point Point { get { return VoronoiStart.Point; } }

        public bool Coast
        {
            get
            {
                if (DelaunayStart != null && DelaunayEnd != null)
                    return ((VoronoiStart.Coast) && (VoronoiEnd.Coast)
                        && !(DelaunayStart.Water && DelaunayEnd.Water)
                        && !(DelaunayStart.Land && DelaunayEnd.Land));
                return false;
            }

            set {  }
        }

        public Corner[] Corners { get { return new Corner[] {VoronoiStart, VoronoiEnd}; } }

        public double DiffX { get { return VoronoiEnd.Point.X - VoronoiStart.Point.X; } }
        public double DiffY { get { return VoronoiEnd.Point.Y - VoronoiStart.Point.Y; } }
        
        public bool Equals(Edge other)
        {
            return this.VoronoiStart.Equals(other.VoronoiStart) && 
                this.VoronoiEnd.Equals(other.VoronoiEnd);
        }
    }
}
