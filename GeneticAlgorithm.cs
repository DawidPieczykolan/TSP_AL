namespace AG_AL
{
    class GeneticAlgorithm
    {
        private readonly int populationSize;
        private readonly double crossoverProbability;
        private readonly double mutationProbability;
        private readonly int tournamentSize;
        private int eliteSize;
        private readonly TSPData tspData;
        private Random random;
        public GeneticAlgorithm(int populationSize, double crossoverProbability, double mutationProbability, int tournamentSize, TSPData tspData)
        {
            this.populationSize = populationSize;
            this.crossoverProbability = crossoverProbability;
            this.mutationProbability = mutationProbability;
            this.tournamentSize = tournamentSize;
            this.tspData = tspData;
            random = new Random();
        }

        public TSPSolution Run(int maxGenerations, TimeSpan timeLimit)
        {
            // Create initial population
            List<List<int>> population = new List<List<int>>();
            for (int i = 0; i < populationSize; i++)
            {
                List<int> order = CreateRandomOrder();
                population.Add(order);
            }

            DateTime startTime = DateTime.Now;
            int generation = 0;
            TSPSolution bestSolution = null;
            double bestFitness = double.MaxValue;

            while (generation < maxGenerations && DateTime.Now - startTime < timeLimit)
            {
                // Evaluate fitness of population
                List<double> fitnesses = new List<double>();
                for (int i = 0; i < populationSize; i++)
                {
                    List<int> order = population[i];
                    double fitness = EvaluateFitness(order);
                    fitnesses.Add(fitness);
                    if (fitness < bestFitness)
                    {
                        bestSolution = new TSPSolution(order, fitness, tspData.Cities);
                        bestFitness = fitness;
                    }
                }
                // Add elite individuals to the next generation
                List<List<int>> nextGeneration = new List<List<int>>();
                int eliteCount = (int)Math.Round(populationSize * 0.1); // Select top 10% as elite

                // Get the indices of the top elite individuals
                List<int> eliteIndices = new List<int>();
                List<double> sortedFitnesses = fitnesses.OrderBy(x => x).ToList();
                for (int i = 0; i < eliteCount; i++)
                {
                    eliteIndices.Add(fitnesses.IndexOf(sortedFitnesses[i]));
                }

                // Add elite individuals to the next generation
                for (int i = 0; i < eliteCount; i++)
                {
                    nextGeneration.Add(population[eliteIndices[i]]);
                }



                // Select parents for next generation using tournament selection
                for (int i = eliteCount; i < populationSize; i++)
                {
                    List<int> tournament = new List<int>();
                    for (int j = 0; j < tournamentSize; j++)
                    {
                        int index = random.Next(populationSize);
                        tournament.Add(index);
                    }
                    int winner = tournament[0];
                    double bestFitnessInTournament = fitnesses[winner];
                    for (int j = 1; j < tournamentSize; j++)
                    {
                        int challenger = tournament[j];
                        double challengerFitness = fitnesses[challenger];
                        if (challengerFitness < bestFitnessInTournament)
                        {
                            winner = challenger;
                            bestFitnessInTournament = challengerFitness;
                        }
                    }
                    nextGeneration.Add(population[winner]);
                }


                // Generate offspring through crossover and mutation
                List<List<int>> offspring = new List<List<int>>();
                for (int i = 0; i < populationSize; i += 2)
                {
                    List<int> parent1 = nextGeneration[i];
                    List<int> parent2 = nextGeneration[i + 1];
                    List<int> child1, child2;
                    if (random.NextDouble() < crossoverProbability)
                    {
                        Crossover(parent1, parent2, out child1, out child2);
                    }
                    else
                    {
                        child1 = parent1;
                        child2 = parent2;
                    }
                    Mutate(child1);
                    Mutate(child2);
                    offspring.Add(child1);
                    offspring.Add(child2);
                }


                // Replace current population with offspring
                List<double> offspringFitnesses = new List<double>();
                for (int i = 0; i < populationSize; i++)
                {
                    List<int> order = offspring[i];
                    double fitness = EvaluateFitness(order);
                    offspringFitnesses.Add(fitness);
                }
                List<int> leastFitIndices = Enumerable.Range(0, populationSize).OrderByDescending(x => fitnesses[x]).Take(offspring.Count).ToList();
                for (int i = 0; i < offspring.Count; i++)
                {
                    int index = leastFitIndices[i];
                    population[index] = offspring[i];
                    fitnesses[index] = offspringFitnesses[i];
                }

                generation++;
            }

            return bestSolution;
        }

        private List<int> CreateRandomOrder()
        {
            List<int> order = new List<int>();
            for (int i = 0; i < tspData.Cities.Count; i++)
            {
                order.Add(i);
            }
            Shuffle(order);
            return order;
        }
        private void Shuffle(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = random.Next(i, list.Count);
                int temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        private double EvaluateFitness(List<int> order)
        {
            double distance = 0;
            for (int i = 0; i < order.Count - 1; i++)
            {
                int city1Index = order[i];
                int city2Index = order[i + 1];
                if (city1Index >= tspData.Cities.Count || city2Index >= tspData.Cities.Count)
                {
                    throw new ArgumentException("City index out of range");
                }
                City city1 = tspData.Cities[city1Index];
                City city2 = tspData.Cities[city2Index];
                distance += city1.DistanceTo(city2);
            }
            return distance;
        }

        private void Crossover(List<int> parent1, List<int> parent2, out List<int> child1, out List<int> child2)
        {
            int length = parent1.Count;
            int crossoverPoint = random.Next(length);

            child1 = new List<int>(parent1.Take(crossoverPoint));
            child2 = new List<int>(parent2.Take(crossoverPoint));

            for (int i = crossoverPoint; i < length; i++)
            {
                int gene1 = parent1[i];
                int gene2 = parent2[i];

                if (!child1.Contains(gene2))
                {
                    child1.Add(gene2);
                }

                if (!child2.Contains(gene1))
                {
                    child2.Add(gene1);
                }
            }
        }

        private void Mutate(List<int> order)
        {
            if (order.Count == 0)
            {
                return;
            }
            int index = random.Next(order.Count);
            int otherIndex = random.Next(order.Count);
            if (otherIndex == index)
            {
                otherIndex = (otherIndex + 1) % order.Count;
            }
            int temp = order[index];
            order[index] = order[otherIndex];
            order[otherIndex] = temp;
        }

    }
}