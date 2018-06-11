using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering
{
    static class Euclidean
    {
        public static double CalculateDistance(Dictionary<int, double> vector1, Dictionary<int, double> vector2)
        {
            double sumRating = 0;
            foreach (KeyValuePair<int, double> coordinate in vector1)
            {
                sumRating += Math.Pow(coordinate.Value - vector2[coordinate.Key], 2);    
            }

            return Math.Sqrt(sumRating);
        }
    }
}
