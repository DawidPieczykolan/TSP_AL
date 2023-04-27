namespace AG_AL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load TSP data from file
            string filePath = "bier127.tsp";
            TSPData tspData = LoadTSPData(filePath);

            // Run genetic algorithm to solve TSP problem
            int populationSize = 100;
            double crossoverProbability = 0.4;
            double mutationProbability = 0.03;
            int tournamentSize = 5;
            int maxGenerations = 1000;
            int eliteSize = 30;


            GeneticAlgorithm ga = new GeneticAlgorithm(populationSize, crossoverProbability, mutationProbability, tournamentSize, tspData);
            TSPSolution solution = ga.Run(maxGenerations, TimeSpan.FromMinutes(5));

            // Save solution to file
            string outputPath = "solution.txt";
            SaveSolution(outputPath, solution);

            Console.WriteLine("Zapisany w : {0}", outputPath);
            Console.WriteLine("Dystans : {0}", solution.TotalDistance);
        }

        static TSPData LoadTSPData(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            int dimension = 0;
            List<City> cities = new List<City>();
            foreach (string line in lines)
            {
                string[] fields = line.Split();
                if (fields.Length < 3)
                    continue;
                if (fields[0] == "DIMENSION")
                    dimension = int.Parse(fields[2]);
                else if (fields[0] == "NODE_COORD_SECTION")
                {
                    for (int i = 0; i < dimension; i++)
                    {
                        string[] cityFields = lines[i + 6].Split();
                        int id = int.Parse(cityFields[0]);
                        double x = double.Parse(cityFields[1]);
                        double y = double.Parse(cityFields[2]);
                        cities.Add(new City(id, x, y));
                    }
                }
            }
            return new TSPData(cities);
        }

        static void SaveSolution(string filePath, TSPSolution solution)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("NAME : TSP solution");
                writer.WriteLine("COMMENT : Total distance = {0}", solution.TotalDistance);
                writer.WriteLine("TYPE : TOUR");
                writer.WriteLine("DIMENSION : {0}", solution.Order.Count);
                writer.WriteLine("TOUR_SECTION");
                foreach (int cityId in solution.Order)
                {
                    writer.WriteLine(cityId);
                }
                writer.WriteLine("TTE");
            }
        }
    }
}