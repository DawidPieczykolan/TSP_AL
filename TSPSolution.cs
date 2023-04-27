namespace AG_AL
{
    class TSPSolution
    {
        private List<City> cities;

        public List<int> Order { get; set; }
        public double Fitness { get; set; }

        public TSPSolution(List<int> order, double fitness, List<City> cities)
        {
            Order = order;
            Fitness = fitness;
            this.cities = cities;
        }

        public override string ToString()
        {
            return $"Order: {string.Join(", ", Order)}, Fitness: {Fitness}";
        }

        public double TotalDistance
        {
            get
            {
                double distance = 0;
                int n = Order.Count;
                for (int i = 0; i < n; i++)
                {
                    int j = (i + 1) % n;
                    distance += cities[Order[i]].DistanceTo(cities[Order[j]]);
                }
                return distance;
            }
        }
    }
}