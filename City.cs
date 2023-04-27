namespace AG_AL
{
    class City
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public City(int id, double x, double y)
        {
            Id = Id;
            X = x;
            Y = y;
        }

        public City(double x, double y, double y1)
        {
            Y1 = y1;
        }

        public double x
        {
            get { return X; }
        }

        public double y
        {
            get { return Y; }
        }

        public double Y1 { get; }

        public double DistanceTo(City city)
        {
            double xDistance = Math.Abs(X - city.x);
            double yDistance = Math.Abs(Y - city.y);
            double distance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
            return distance;
        }
    }
}