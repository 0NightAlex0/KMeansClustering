using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace KMeansClustering
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, Client> dataSet = ParseDataSet(@"D:\github\KMeansClustering\KMeansClustering\WineData.csv", ",");
            Algorithm clustering = new Algorithm(dataSet);
            clustering.Run(4, 25);
            Console.WriteLine("Hi");

        }

        public static Dictionary<int, Client> ParseDataSet(string path, string seperator)
        {
            // 100 clients and 32 offers for each client
            string[] lines = File.ReadAllLines(path);
            Dictionary<int, Client> dataSet = new Dictionary<int, Client>();
            // row
            for (int wineId = 1; wineId < lines.Length + 1; wineId++)
            {
                // -1 since index starts at 0
                string[] lineSplit = lines[wineId - 1].Split(",");
                // column
                for (int clientId = 1; clientId < lineSplit.Length + 1; clientId++)
                {
                    if (!dataSet.ContainsKey(clientId))
                    {
                        dataSet.Add(clientId, new Client());
                    }
                    double offer = double.Parse(lineSplit[clientId - 1]);
                    dataSet[clientId].vector.Add(wineId, offer);

                }
            }
            return dataSet.OrderBy(x => x.Key).ToDictionary(k => k.Key, k => k.Value);
        }

        //Function to get random number
        private static readonly Random getRandom = new Random();

        // min =  start and max = until
        public static int GetRandomNumber(int min, int max)
        {
            // prevent another thread from using this while this thread is active
            // synchronize
            lock (getRandom)
            {
                return getRandom.Next(min, max);
            }
        }

        public static Dictionary<TKey, TValue> DeepCloneDictionaryWithReferences<TKey, TValue>(Dictionary<TKey, TValue> original) where TValue : ICloneable
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value.Clone());
            }
            return ret;
        }

        public static bool AreCentroidsEqual(Dictionary<int, Centroid> centroids, Dictionary<int, Centroid> centroids2)
        {
            if (centroids.Count == centroids2.Count) // Require equal count.
            {
                foreach (KeyValuePair<int, Centroid> centroid in centroids)
                {
                    if (centroids2.ContainsKey(centroid.Key))
                    {
                        Dictionary<int, double> centroid2Vector = centroids2[centroid.Key].vector;
                        if (centroid.Value.vector.Count == centroid2Vector.Count)
                        {
                            foreach (KeyValuePair<int, double> coordinate in centroid.Value.vector)
                            {

                                if (!centroid2Vector.ContainsKey(coordinate.Key) || coordinate.Value != centroid2Vector[coordinate.Key])
                                {
                                    // if it doesnt contain a certain coordinate
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            // vectors doesnt have the same length
                            return false;
                        }

                    }
                    else
                    {
                        // if it doesnt contain a certain centroid
                        return false;
                    }
                }
                // if everything is the same
                return true;
            }
            // if length of both dictionairy not equal
            return false;
        }
    }
}
