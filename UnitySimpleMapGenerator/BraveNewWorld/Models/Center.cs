using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BraveNewWorld.Models
{
    public class Center : DependencyObject, IEquatable<Center>, IMapItem
    {
        public int Index { get; set; }

        public int Key { get; set; }
        public Point Point { get; set; }  // location
        public Boolean Water { get; set; }  // lake or ocean
        public Boolean Land { get { return !Water; } set { Water = !value; } }  // lake or ocean
        public Boolean Ocean { get; set; }  // ocean
        public Boolean Coast { get; set; }  // land polygon touching an ocean
        public Boolean Border { get; set; }  // at the edge of the map
        public Vector3D polyNorm { get; set; }

        private string _biome;
        public string Biome
        {
            get
            {
                return _biome;
            }
            set
            {
                if (value == "Ocean")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 54, G = 54, B = 97, A = 255 });
                if (value == "ShallowWater")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 74, G = 74, B = 117, A = 255 });
                else if (value == "Marsh")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 196, G = 204, B = 187 , A=255 });
                else if (value == "Lake")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 54, G = 54, B = 97, A = 255 });
                else if (value == "Beach")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 173, G = 161, B = 139, A = 255 });
                else if (value == "Snow")
                    PolygonBrush = new SolidColorBrush(Colors.White);
                else if (value == "Tundra")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 196, G = 204, B = 187, A = 255 });
                else if (value == "Bare")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 187, G = 187, B = 187, A = 255 });
                else if (value == "Scorched")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 153, G = 153, B = 153, A = 255 });
                else if (value == "Taiga")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 204, G = 212, B = 187, A = 255 });
                else if (value == "Shrubland")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 153, G = 166, B = 139, A = 255 });
                else if (value == "TemperateDesert")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 228, G = 232, B = 202, A = 255 });
                else if (value == "TemperateRainForest")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 84, G = 116, B = 88, A = 255 });
                else if (value == "TemperateDeciduousForest")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 119, G = 139, B = 85, A = 255 });
                else if (value == "Grassland")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 153, G = 180, B = 112, A = 255 });
                else if (value == "TropicalRainForest")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 112, G = 139, B = 85, A = 255 });
                else if (value == "TropicalSeasonalForest")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 85, G = 139, B = 85, A = 255 });
                else if (value == "SubtropicalDesert")
                    PolygonBrush = new SolidColorBrush(new Color() { R = 172, G = 159, B = 139, A = 255 });

                _biome = value;
            }
        }

        public SolidColorBrush PolygonBrush { get; set; } 
        
        //public string Biome { get; set; }  // biome type (see article)
        public double Elevation { get; set; }  // 0.0-1.0
        public double Moisture { get; set; }  // 0.0-1.0

        public HashSet<Center> Neighbours { get; set; }
        public HashSet<Edge> Borders { get; set; }
        public HashSet<Corner> Corners { get; set; }

        public Center(double x , double y)
        {
            polyNorm = new Vector3D(0, 0, 0);
            Point = new Point(x,y);
            Key = Point.GetHashCode();
            Index = Point.GetHashCode();
            
            Water = Coast = Ocean = Border = false;
            Elevation = Moisture = 0.0d;

            Neighbours = new HashSet<Center>(new CenterComparer());
            Borders = new HashSet<Edge>(new EdgeComparer());
            Corners = new HashSet<Corner>(new CornerComparer());
        }

        #region Methods
        public bool Equals(Center other)
        {
            return this.Point == other.Point;
        }

        public void OrderCorners()
        {
            Corner CurrentCorner = Corners.First();
            List<Corner> Ordered = new List<Corner>();
            Corner newdot;
            Edge ed;

            Ordered.Add(CurrentCorner);
            do
            {
                ed = CurrentCorner.Protrudes.FirstOrDefault(x => Borders.Contains(x) && !(Ordered.Contains(x.VoronoiStart) && (Ordered.Contains(x.VoronoiEnd))));

                if (ed != null)
                {
                    newdot = ed.Corners.FirstOrDefault(x => !Ordered.Contains(x));
                    Ordered.Add(newdot);
                    CurrentCorner = newdot;
                }
            } while (ed != null);

            Corners.Clear();

            foreach (var corner in Ordered)
            {
                Corners.Add(corner);
            }
        }

        public void FixBorders()
        {
            var ms = from p in Borders.SelectMany(x => x.Corners)
                     group p by p.Point
                     into grouped
                     select new {point = grouped.Key, count = grouped.Count()};

            var fpoint = ms.FirstOrDefault(x => x.count == 1);
            var spoint = ms.LastOrDefault(x => x.count == 1);

            if(fpoint != null & spoint != null)
            {
                Corner p1 = Corners.FirstOrDefault(x => x.Point == fpoint.point);
                Corner p2 = Corners.FirstOrDefault(x => x.Point == spoint.point);

                if (p1 == null || p2 == null)
                    return;

                IFactory fact = new MapItemFactory();
                Edge e = fact.EdgeFactory(
                    p1,
                    p2,
                    this, null);

                e.MapEdge = true;

                p1.Protrudes.Add(e);
                p2.Protrudes.Add(e);

                this.Border = this.Ocean = this.Water = true;
                e.VoronoiStart.Border = e.VoronoiEnd.Border = true;
                e.VoronoiStart.Elevation = e.VoronoiEnd.Elevation = 0.0d;

                this.Borders.Add(e);
            }
        }
        
        
        #endregion

        public void SetBiome()
        {
            if (Ocean && Elevation < -0.1d)
            {
                Biome = "Ocean";
            }
            else
                if (Ocean && Elevation > -0.1d)
                {
                    Biome = "ShallowWater";
                }
                else if (Water)
                {
                    if (Elevation < 0.1) Biome = "Marsh";
                    if (Elevation > 0.8) Biome = "Ice";
                    Biome = "Lake";
                }
                else if (Coast)
                {
                    Biome = "Beach";
                }
                else if (Elevation > 0.8)
                {
                    if (Moisture > 0.50) Biome = "Snow";
                    else if (Moisture > 0.33) Biome = "Tundra";
                    else if (Moisture > 0.16) Biome = "Bare";
                    else Biome = "Scorched";
                }
                else if (Elevation > 0.6)
                {
                    if (Moisture > 0.66) Biome = "Taiga";
                    else if (Moisture > 0.33) Biome = "Shrubland";
                    else Biome = "TemperateDesert";
                }
                else if (Elevation > 0.3)
                {
                    if (Moisture > 0.83) Biome = "TemperateRainForest";
                    else if (Moisture > 0.50) Biome = "TemperateDeciduousForest";
                    else if (Moisture > 0.16) Biome = "Grassland";
                    else Biome = "TemperateDesert";
                }
                else
                {
                    if (Moisture > 0.66) Biome = "TropicalRainForest";
                    else if (Moisture > 0.33) Biome = "TropicalSeasonalForest";
                    else if (Moisture > 0.16) Biome = "Grassland";
                    else Biome = "SubtropicalDesert";
                }
        }


        
    }
}
