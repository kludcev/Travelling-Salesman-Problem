using System;
using System.Linq;

namespace Tsp.Data
{
    public class SimulatedAnnealing
    {
        private int[] _citiesOrder;

        private readonly double[,] _adjacencyMatrix;
        private readonly Random _random = new Random();

        public SimulatedAnnealing(double[,] adjacencyMatrix)
        {
            if (adjacencyMatrix == null || adjacencyMatrix.GetLength(0) == 0)
                throw new ArgumentException(nameof(adjacencyMatrix));

            _adjacencyMatrix = adjacencyMatrix;
            _citiesOrder = Enumerable.Range(0, _adjacencyMatrix.GetLength(0)).ToArray();
        }

        public Results Anneal(double temperature, double coolingRate, double absoluteTemperature)
        {
            var iterationCount = 0;
            var distance = GetTotalDistance(_citiesOrder);

            while (temperature > absoluteTemperature)
            {
                var nextOrder = GetNextArrangement(_citiesOrder);
                var deltaDistance = GetTotalDistance(nextOrder) - distance;

                //if a new order has smaller distance
                //or if a new order has larger distance but satisfies the Boltzman condition
                if ((deltaDistance < 0) || distance > 0 && Math.Exp(-deltaDistance / temperature) > _random.NextDouble())
                {
                    _citiesOrder = nextOrder;
                    distance = deltaDistance + distance;
                }

                //cool down the temperature
                temperature *= coolingRate;

                iterationCount++;
            }

            return new Results
            {
                IterationCount = iterationCount,
                ShortestDistance = distance,
                CitiesOrder = _citiesOrder,
            };
        }

        /// <summary>
        /// Calculate total distance between cities
        /// </summary>
        /// <param name="citiesOrder"></param>
        /// <returns></returns>
        private double GetTotalDistance(int[] citiesOrder)
        {

            var distance = citiesOrder.Select((c, i) =>
            {
                while (i < citiesOrder.Length - 1)
                {
                    return _adjacencyMatrix[c, citiesOrder[i + 1]];
                }

                return 0;
            }).Sum();

            if (citiesOrder.Length > 0)
                distance += _adjacencyMatrix[citiesOrder[citiesOrder.Length - 1], 0];

            return distance;
        }

        /// <summary>
        /// Get random cities order
        /// </summary>
        /// <param name="citiesOrder">current cities order</param>
        /// <returns></returns>
        private int[] GetNextArrangement(int[] citiesOrder)
        {
            var newOrder = citiesOrder.Select(c => c).ToArray();

            var firstRandomCityIndex = _random.Next(1, newOrder.Length);
            var secondRandomCityIndex = _random.Next(1, newOrder.Length);

            var dummy = newOrder[firstRandomCityIndex];
            newOrder[firstRandomCityIndex] = newOrder[secondRandomCityIndex];
            newOrder[secondRandomCityIndex] = dummy;

            return newOrder;
        }

        public class Results
        {
            public double ShortestDistance { get; set; }
            public int[] CitiesOrder { get; set; }
            public int IterationCount { get; set; }
        }
    }
}