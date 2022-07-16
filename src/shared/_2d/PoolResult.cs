using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace shared._2d
{
    public class PoolResult
    {
        public string? RawData { get; set; }
        public float MSB { get; set; }
        public float MSE { get; set; }
        public float Pvalue { get; set; }
        public float PseudoF { get; set; }
        public int K { get; set; }
        public int N { get; set; }
        public Vector2 Centroid { get; set; }
    }
}
