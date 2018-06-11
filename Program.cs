using System;

namespace KMeansClustering
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, ClientInfo> dataSet = ParseDataSet(@"D:\github\DataScience2\DataScience2\DataScience2\WineData.csv", ",");
            KMeansClustering clustering = new KMeansClustering(dataSet);
            clustering.KClustering(4, 50);
            Console.WriteLine("Hi");

        }

        public static Dictionary<int, ClientInfo> ParseDataSet(string path, string seperator)
        {
            // 100 clients and 32 offers for each client
            string[] lines = File.ReadAllLines(path);
            Dictionary<int, ClientInfo> dataSet = new Dictionary<int, ClientInfo>();
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
                        dataSet.Add(clientId, new ClientInfo());
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
    }
}
