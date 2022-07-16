using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace shared._2d
{
    public interface IClusterFeature
    {
        int Count { get; }
        Vector2 LS { get; }
        Vector2 SS { get; }
        Node Parent { get; }
        float Radius { get; }
        void UpdateParent(Node parent);
    }
    public class Cluster
    {
        public Vector2 Centroid { get; set; } = Vector2.Zero;
        public List<Vector2> Children { get; set; } = new List<Vector2>();
    }
    public class Node : IClusterFeature
    {
        private List<IClusterFeature> children;
        private Node parent;
        public Node()
        {
            children = new List<IClusterFeature>();
            parent = null;
        }
        public Node(Node parent)
        {
            children = new List<IClusterFeature>();
            this.parent = parent;
        }

        //-------------- Properties
        public int Count => children.Sum(c => c.Count);
        public Vector2 LS => children.Sum(c => c.LS);
        public Vector2 SS => children.Sum(c => c.SS);
        public List<IClusterFeature> Children => children;
        public float Radius => this.GetRadius();
        public Node Parent => parent;

        //----------------------- Public Methods
        public void Add(IClusterFeature clusterFeature) => children.Add(clusterFeature);
        public void UpdateParent(Node parent) => this.parent = parent;
        public void AddRange(IEnumerable<IClusterFeature> clusterFeatures) => children.AddRange(clusterFeatures);

        //------------------- Private Methods
    }

    public class Leaf : IClusterFeature
    {
        private List<Vector2> entries;
        private Node parent;
        public Leaf(Node parent, Vector2 data)
        {
            entries = new List<Vector2>();
            entries.Add(data);
            this.parent = parent;
        }

        //-------------- Properties
        public int Count => entries.Count;
        public Vector2 LS => entries.Sum(c => c);
        public Vector2 SS => entries.Sum(c => c * c);
        public List<Vector2> Entries => entries;
        public Node Parent => parent;
        public float Radius => this.GetRadius();

        //---------------- Public Methods
        public void Add(Vector2 data) => entries.Add(data);
        public void UpdateParent(Node parent) => this.parent = parent;

        public float RadiusWithData(Vector2 data)
        {
            var ss = SS + data * data;
            var ls = LS + data;
            var n = Count + 1;

            var ts = ss - (ls * ls / n);
            var ms = n;

            return Vector2.SquareRoot(ts / ms).Length();
        }
    }
}
