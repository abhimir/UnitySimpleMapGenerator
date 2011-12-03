using System;
using System.Collections.Generic;
using System.Windows;

namespace BraveNewWorld.Models
{
    class CenterComparer : IEqualityComparer<Center>
    {
        public CenterComparer() { }
        public bool Equals(Center x, Center y)
        {
            //return (x.Point.X == y.Point.X) && (x.Point.Y == y.Point.Y);
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(Center obj)
        {
            return obj.Point.GetHashCode();
        }
    }

    class CornerComparer : IEqualityComparer<Corner>
    {
        public CornerComparer() { }
        public bool Equals(Corner x, Corner y)
        {
            return x.Point.Equals(y.Point);
        }

        public int GetHashCode(Corner obj)
        {
            return obj.Point.GetHashCode();
        }
    }

    class EdgeComparer : IEqualityComparer<Edge>
    {
        public EdgeComparer() { }
        public bool Equals(Edge x, Edge y)
        {
            return x.Midpoint == y.Midpoint;
        }

        public int GetHashCode(Edge obj)
        {
            return obj.Midpoint.GetHashCode();
        }
    }

    public interface IFactory
    {
        Center CenterFactory(double ax, double ay);
        Edge EdgeFactory(Corner begin, Corner end, Center Left, Center Right);
        Corner CornerFactory(double ax, double ay);
    }

    public class MapItemFactory : IFactory
    {
        #region Implementation of IFactory

        public Center CenterFactory(double ax, double ay)
        {
            Point p = new Point(ax, ay);
            int hash = p.GetHashCode();
            if(App.AppMap.Centers.ContainsKey(hash))
            {
                return App.AppMap.Centers[hash];
            }
            else
            {
                var nc = new Center(ax, ay);
                App.AppMap.Centers.Add(nc.Key, nc);
                return nc;
            }
        }

        public Edge EdgeFactory(Corner begin, Corner end, Center Left, Center Right)
        {
            Point p = new Point((begin.Point.X + end.Point.X) / 2, (begin.Point.Y + end.Point.Y) / 2);
            int hash = p.GetHashCode();
            if (App.AppMap.Edges.ContainsKey(hash))
            {
                return App.AppMap.Edges[hash];
            }
            else
            {
                var nc = new Edge(begin,end,Left,Right);
                App.AppMap.Edges.Add(nc.Key, nc);
                return nc;
            }
        }

        public Corner CornerFactory(double ax, double ay)
        {
            Point p = new Point(ax, ay);
            int hash = p.GetHashCode();
            if (App.AppMap.Corners.ContainsKey(hash))
            {
                return App.AppMap.Corners[hash];
            }
            else
            {
                var nc = new Corner(ax, ay);
                App.AppMap.Corners.Add(nc.Key, nc);
                return nc;
            }
        }

        #endregion

        public void RemoveEdge(Edge edge)
        {
            App.AppMap.Edges.Remove(edge.Midpoint.GetHashCode());
        }

        public void RemoveCorner(Corner corner)
        {
            App.AppMap.Corners.Remove(corner.Point.GetHashCode());
        }
    }
}
