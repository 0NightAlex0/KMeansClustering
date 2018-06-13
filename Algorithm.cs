using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMeansClustering
{
    class Algorithm
    {
        private readonly Dictionary<int, Client> _clients;
        public Algorithm(Dictionary<int, Client> clients)
        {
            _clients = clients;
        }

        public void Run(int clustersAmount, int iterations)
        {
            Tuple<double, Dictionary<int, Centroid>> bestClustering = new Tuple<double, Dictionary<int, Centroid>>(Double.MaxValue, new Dictionary<int, Centroid>());
            //repeat the whole algorithm to find the lowest SSE and its result
            for (int i = 0; i < iterations; i++)
            {
                // i dont want to change _clients. newClients will be used to execute the algorithm
                Dictionary<int, Client> newClients = Program.DeepCloneDictionaryWithReferences(_clients);
                // intialize centroids
                Dictionary<int, Centroid> originalCentroids = GetForgyCentroids(clustersAmount, 1, 33);
                // to check later on if these 2 centroids are the same
                //Dictionary<int, Centroid> clonedCentroids = Program.DeepCloneDictionaryWithReferences(originalCentroids);
                for (int j = 0; j < 50; j++)
                {
                    AssignToClosestCentroids(newClients, originalCentroids);
                    RecomputeCentroids(originalCentroids);

                    //// stop when the centroids vectors are not changing anymore
                    //if (Program.AreCentroidsEqual(originalCentroids, clonedCentroids))
                    //{
                    //    break;
                    //}
                    //else
                    //{
                    //    originalCentroids = Program.DeepCloneDictionaryWithReferences(clonedCentroids);
                    //}
                }
                double sumOfSquaredErrors = ComputeSumOfSquaredErrors(newClients);
                //Console.WriteLine(sumOfSquaredErrors);
                if (sumOfSquaredErrors < bestClustering.Item1)
                {
                    bestClustering = new Tuple<double, Dictionary<int, Centroid>>(sumOfSquaredErrors, originalCentroids);
                }
                if(i == iterations - 1)
                {
                    foreach (KeyValuePair<int, Client> client in newClients)
                    {
                        Console.WriteLine($"{client.Key}: {newClients[client.Key].distanceToCentroid}");

                    }
                }
            }
            Console.WriteLine(bestClustering.Item1);
            //PrintClientsCluster(bestClustering.Item2);
        }

        private void AssignToClosestCentroids(Dictionary<int, Client> clients, Dictionary<int, Centroid> centroids)
        {
            // remove all old clients
            foreach (KeyValuePair<int, Centroid> centroid in centroids)
            {
                centroid.Value.clients.Clear();
            }
            // find the new clients
            foreach (KeyValuePair<int, Client> client in clients)
            {
                Dictionary<int, double> tempDistancesToCentroid = new Dictionary<int, double>();
                foreach (KeyValuePair<int, Centroid> centroid in centroids)
                {
                    double currentCentroidDistance = Euclidean.CalculateDistance(centroid.Value.vector, client.Value.vector);
                    tempDistancesToCentroid.Add(centroid.Key, currentCentroidDistance);
                }
                KeyValuePair<int, double> closestCentroid = tempDistancesToCentroid.OrderBy(c => c.Value).FirstOrDefault();
                client.Value.distanceToCentroid = closestCentroid.Value;
                centroids[closestCentroid.Key].clients.Add(client.Key, client.Value);
            }
        }

        private void RecomputeCentroids(Dictionary<int, Centroid> centroids)
        {
            //recompute the centroid to the cluster center
            foreach(KeyValuePair<int, Centroid> centroid in centroids)
            {
                Dictionary<int, double> newCentroidVector = new Dictionary<int, double>();
                foreach(KeyValuePair<int, Client> client in centroid.Value.clients)
                {
                    foreach(KeyValuePair<int, double> coordinate in client.Value.vector)
                    {
                        if (!newCentroidVector.ContainsKey(coordinate.Key))
                        {
                            newCentroidVector.Add(coordinate.Key, 0);
                        }
                        newCentroidVector[coordinate.Key] += coordinate.Value;
                    }
                }

                foreach (KeyValuePair<int, double> coordinate in newCentroidVector)
                {
                    centroid.Value.vector[coordinate.Key] = coordinate.Value / centroid.Value.clients.Count;
                }

            }
        }

        private double ComputeSumOfSquaredErrors(Dictionary<int, Client> clients)
        {
            double sum = 0;
            foreach (KeyValuePair<int, Client> client in clients)
            {
                sum += Math.Pow(client.Value.distanceToCentroid, 2);
            }
            return sum;
        }

        private Dictionary<int, Centroid> GetForgyCentroids(int clusters, int min, int max)
        {
            Dictionary<int, Centroid> centroids = new Dictionary<int, Centroid>();
            List<int> usedRandomNumbers = new List<int>();
            int count = 1;

            while (count <= clusters)
            {
                int randomNumber = Program.GetRandomNumber(min, max);
                if (!usedRandomNumbers.Contains(randomNumber))
                {
                    //insert a random existing client inside
                    centroids.Add(count, new Centroid { vector = new Dictionary<int, double>(_clients[randomNumber].vector) });
                    usedRandomNumbers.Add(randomNumber);
                    count++;
                }

            }
            //centroids.Add(1, new Centroid { vector = _clients[11].vector });
            //centroids.Add(2, new Centroid { vector = _clients[22].vector });
            //centroids.Add(3, new Centroid { vector = _clients[13].vector });
            //centroids.Add(4, new Centroid { vector = _clients[14].vector });

            return centroids;
        }




        private void PrintClientsCluster(Dictionary<int, Centroid> centroids)
        {
            // centroidId, offer, times bought
            Dictionary<int, Dictionary<int, int>> clusterResult = new Dictionary<int, Dictionary<int, int>>();
            // computing the result to be printed
            // TODO Fix this for loop
            foreach (KeyValuePair<int, Centroid> centroid in centroids)
            {
                if (!clusterResult.ContainsKey(centroid.Key))
                {
                    clusterResult.Add(centroid.Key, new Dictionary<int, int>());
                }

                foreach (KeyValuePair<int, Client> client in centroid.Value.clients)
                {
                    foreach (KeyValuePair<int, double> coordinate in client.Value.vector)
                    {

                        if (!clusterResult[centroid.Key].ContainsKey(coordinate.Key))
                        {
                            clusterResult[centroid.Key].Add(coordinate.Key, 0);
                        }
                        clusterResult[centroid.Key][coordinate.Key]++;
                    }
                }
            }
            // the actual print
            foreach (KeyValuePair<int, Dictionary<int, int>> cluster in clusterResult)
            {
                Console.WriteLine($"cluster: {cluster.Key}");
                var result = from pair in cluster.Value
                             where pair.Value > 3
                             orderby pair.Value descending
                             select pair;

                foreach (KeyValuePair<int, int> pair in result)
                {
                    Console.WriteLine("offer: {0} was bought {1} times", pair.Key, pair.Value);
                }
            }

        }
    }
}

