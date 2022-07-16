using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace shared._2d
{
    public static class Utils
    {
        public static Vector2 Sum(this IEnumerable<IClusterFeature> clusterFeatures, Func<IClusterFeature, Vector2> func)
        {
            var sum = Vector2.Zero;
            foreach (var feature in clusterFeatures)
            {
                sum = sum + func(feature);
            }
            return sum;
        }

        public static Vector2 Sum(this IEnumerable<Vector2> vector2s, Func<Vector2, Vector2> func)
        {
            var sum = Vector2.Zero;
            foreach (var v in vector2s)
            {
                sum = sum + func(v);
            }
            return sum;
        }
        public static Vector2 Average(this IEnumerable<Vector2> vector2s)
        {
            var sum = Vector2.Zero;
            foreach (var v in vector2s)
            {
                sum = sum + v;
            }
            return sum / vector2s.Count();
        }
        public static float GetRadius(this IClusterFeature clusterFeature)
        {
            var ts = clusterFeature.SS - (clusterFeature.LS * clusterFeature.LS / clusterFeature.Count);
            var ms = clusterFeature.Count;
            return Vector2.SquareRoot(ts / ms).Length();
        }
    }
}
