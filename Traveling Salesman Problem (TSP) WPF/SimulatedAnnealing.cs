using System;

using System.Collections.Generic;

namespace Traveling_Salesman_Problem__TSP__WPF
{
    public class SimulatedAnnealing
    {
        private List<int> nextOrder = new List<int>();
        private double[,] distances;
        private Random random = new Random();

        public double ShortestDistance { get; set; }

        public List<int> CitiesOrder { get; set; }

        public SimulatedAnnealing(double[,] distaDoubles)
        {
            ShortestDistance = 0;
            CitiesOrder = new List<int>();
            distances = distaDoubles;
            for (int i = 0; i < distances.GetLength(0); i++)
                CitiesOrder.Add(i);
        }

        /// <summary>
        /// Calculate the total distance which is the objective function
        /// </summary>
        /// <param name="order">A list containing the order of cities</param>
        /// <returns></returns>
        private double GetTotalDistance(List<int> order)
        {
            double distance = 0;

            for (int i = 0; i < order.Count - 1; i++)
            {
                distance += distances[order[i], order[i + 1]];
            }

            if (order.Count > 0)
            {
                distance += distances[order[order.Count - 1], 0];
            }

            return distance;
        }

        /// <summary>
        /// Get the next random arrangements of cities
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private List<int> GetNextArrangement(List<int> order)
        {
            List<int> newOrder = new List<int>();

            for (int i = 0; i < order.Count; i++)
                newOrder.Add(order[i]);

            //we will only rearrange two cities by random
            //starting point should be always zero - so zero should not be included

            int firstRandomCityIndex = random.Next(1, newOrder.Count);
            int secondRandomCityIndex = random.Next(1, newOrder.Count);

            int dummy = newOrder[firstRandomCityIndex];
            newOrder[firstRandomCityIndex] = newOrder[secondRandomCityIndex];
            newOrder[secondRandomCityIndex] = dummy;

            return newOrder;
        }

        /// <summary>
        /// Annealing Process
        /// </summary>
        public int Anneal(double temperature, double deltaDistance, double coolingRate, double absoluteTemperature)
        {
            int iteration = -1;

            double distance = GetTotalDistance(CitiesOrder);

            while (temperature > absoluteTemperature)
            {
                nextOrder = GetNextArrangement(CitiesOrder);

                deltaDistance = GetTotalDistance(nextOrder) - distance;

                //if the new order has a smaller distance
                //or if the new order has a larger distance but satisfies Boltzman condition then accept the arrangement
                if ((deltaDistance < 0) || (distance > 0 && Math.Exp(-deltaDistance / temperature) > random.NextDouble()))
                {
                    for (int i = 0; i < nextOrder.Count; i++)
                        CitiesOrder[i] = nextOrder[i];

                    distance = deltaDistance + distance;
                }

                //cool down the temperature
                temperature *= coolingRate;

                iteration++;
            }

            ShortestDistance = distance;
            return iteration;
        }
    }
}