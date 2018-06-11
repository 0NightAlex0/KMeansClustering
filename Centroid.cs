using System;
using System.Collections.Generic;
using System.Text;

namespace KMeansClustering
{
    class Centroid : ICloneable
    {
        public Dictionary<int, double> vector = new Dictionary<int, double>();
        public Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public object Clone() => new Centroid()
        {
            vector = new Dictionary<int, double>(vector),
            clients = new Dictionary<int, Client>(clients)
        };
    }
}
