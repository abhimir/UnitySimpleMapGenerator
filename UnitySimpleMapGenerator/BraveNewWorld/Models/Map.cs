using System.Collections.Generic;

namespace BraveNewWorld.Models
{
    public class Map
    {
        public Dictionary<int,Center> Centers { get; set; }
        public Dictionary<int, Corner> Corners { get; set; }
        public Dictionary<int, Edge> Edges { get; set; }

        public Map()
        {
            Centers = new Dictionary<int, Center>();
            Corners = new Dictionary<int, Corner>();
            Edges = new Dictionary<int, Edge>();
        }
    }
}
