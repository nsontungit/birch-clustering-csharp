using Extreme.Statistics.Distributions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace shared._2d
{
    public class CFTree
    {
        private int branchingFactor;
        private int numberOfEntries;
        private float threshold;
        private Node root;
        public CFTree()
        {
            branchingFactor = 2;
            numberOfEntries = 2;
            threshold = 0.15f;
            root = new Node();
        }
        public CFTree(int branchingFactor, float threshold, int numberOfEntries)
        {
            this.branchingFactor = branchingFactor;
            this.numberOfEntries = numberOfEntries;
            this.threshold = threshold;
            root = new Node();
        }
        public Node Root => root;
        public int BranchingFactor { get => branchingFactor; set { branchingFactor = value; } }
        public int NumberOfEntries { get => numberOfEntries; set { numberOfEntries = value; } }
        public float Threshold { get => threshold; set { threshold = value; } }

        public void Add(Vector2 data)
        {
            var closestNode = ClosestNode(data, root);
            if (closestNode.Equals(root))
            {
                root.Add(new Leaf(root, data));
                return;
            }
            var radius = closestNode is Leaf ? ((Leaf)closestNode).RadiusWithData(data) 
                : throw new InvalidOperationException("Không phải nút lá");
            if (radius <= threshold)
            {
                if (closestNode.Count < numberOfEntries)
                {
                    ((Leaf)closestNode).Add(data);
                    return;
                }
            }
            var parentNode = ((Leaf)closestNode).Parent;
            while (parentNode != null)
            {
                if (parentNode.Children.Count < branchingFactor)
                {
                    parentNode.Children.Add(new Leaf(parentNode, data));
                    return;
                }
                parentNode = parentNode.Parent;
            }
            parentNode = root;
            var tmp = new List<IClusterFeature>(parentNode.Children);
            parentNode.Children.Clear();
            var newNode1 = new Node(parentNode);
            newNode1.Children.AddRange(tmp);
            tmp.ForEach(c => c.UpdateParent(newNode1));
            var newNode2 = new Node(parentNode);
            newNode2.Add(new Leaf(newNode2, data));
            parentNode.Children.Add(newNode1);
            parentNode.Children.Add(newNode2);
            root = parentNode;
        }

        public List<PoolResult> Clustering()
        {
            List<Cluster> clusters = new List<Cluster>();
            List<PoolResult> results = new List<PoolResult>();
            bool isInit = true;
            ClusterSet(root, clusters);
            int n = clusters.Sum(c => c.Children.Count);
            var nClusters = clusters.Count;
            var clusterCentroid = ClusterCentroid(clusters);
            while (clusters.Count > 2)
            {
                if (!isInit)
                {
                    float minDistance = -1;
                    int iInx = -1;
                    int jInx = -1;
                    for (int i = 0; i < clusters.Count - 1; i++)
                    {
                        for (int j = i + 1; j < clusters.Count; j++)
                        {
                            if (i != j)
                            {
                                var distance = Vector2.Distance(clusters[i].Centroid, clusters[j].Centroid);
                                if (distance < minDistance || minDistance == -1)
                                {
                                    minDistance = distance;
                                    iInx = i;
                                    jInx = j;
                                }
                            }
                        }
                    }
                    clusters[iInx].Children.AddRange(clusters[jInx].Children);
                    clusters[iInx].Centroid = clusters[iInx].Children.Average();
                    clusters.RemoveAt(jInx);
                }
                int k = clusters.Count;
                var SSB = clusters.Sum(c => c.Children.Count * Vector2.DistanceSquared(c.Centroid, clusterCentroid));
                var SSE = clusters.Sum(c => c.Children.Sum(ch => Vector2.DistanceSquared(ch, c.Centroid)));
                var MSB = SSB / (k - 1);
                var MSE = SSE / (n - k);
                var pseudoFValue = MSB / MSE;
                var pvalue = (float)GetPvalue(k, n, pseudoFValue);
                results.Add(new PoolResult()
                {
                    RawData = JsonConvert.SerializeObject(clusters.Select(c => new { c.Children, c.Centroid })),
                    MSB = (float)Math.Round(MSB, 3),
                    MSE = (float)Math.Round(MSE, 3),
                    PseudoF = (float)Math.Round(pseudoFValue, 3),
                    Pvalue = (float)Math.Round(pvalue, 5),
                    K = k,
                    N = n,
                    Centroid = clusterCentroid,
                });
                isInit = false;
            }
            return results;
        }

        private IClusterFeature ClosestNode(Vector2 data, Node node)
        {
            IClusterFeature closestNode = node;
            float minDistance = -1;
            if (node.Count == 0)
                return node;
            while (closestNode is not Leaf)
            {
                foreach (var child in ((Node)closestNode).Children)
                {
                    var distance = Vector2.Distance(child.LS / child.Count, data);
                    if (distance < minDistance || minDistance == -1)
                    {
                        minDistance = distance;
                        closestNode = child;
                    }
                }
                minDistance = -1;
            }
            return closestNode;
        }

        private void ClusterSet(Node root, List<Cluster> clusters)
        {
            if (root == null)
            {
                throw new ArgumentNullException("Root không được phép null");
            }
            if (root.Children == null)
            {
                throw new ArgumentNullException("Các cụm con của Root không được phép null");
            }
            foreach (var i in root.Children)
            {
                if (i is Leaf leaf)
                {
                    clusters.Add(new Cluster()
                    {
                        Children = leaf.Entries,
                        Centroid = leaf.Entries.Average(),
                    });
                }
                else
                {
                    ClusterSet((Node)i, clusters);
                }
            }
        }

        private Vector2 ClusterCentroid(IEnumerable<Cluster> clusters)
        {
            Vector2 sum = Vector2.Zero;
            int n = 0;
            foreach (Cluster cluster in clusters)
            {
                sum = sum + cluster.Children.Sum(c => c);
                n += cluster.Children.Count;
            }
            return sum / n;
        }

        private double GetPvalue(int k, int n, float pseudoValue)
        {
            var df1 = k - 1;
            var df2 = n - k;
            FDistribution fdis = new FDistribution(df1, df2);
            var pvalue = fdis.RightTailProbability(pseudoValue);
            return pvalue;
        }
    }
}
