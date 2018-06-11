using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering
{
    class Client : ICloneable
    {
        public Dictionary<int, double> vector = new Dictionary<int, double>();
        public double distanceToCentroid = Double.MaxValue;
        public object Clone() => new Client()
        {
            vector = new Dictionary<int, double>(vector),
        };
    }
}
