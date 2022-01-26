using NUnit.Framework;

namespace Tsp.Data.UnitTests.SimulatedAnnealingTests
{
    public class AnnealResultsAreValid
    {
        private const double Temperature = 10;
        private const double CoolingRate = 0.9999;
        private const double AbsoluteTemperature = 0.00001;
        private const int ExpectedIterationCount = 138149;
        private const int ExpectedDistance = 816;

        private SimulatedAnnealing.Results _simulatedAnnealingResults;

        private readonly int[] _expectedCitiesOrder = { 0, 1, 2, 3, 4 };
        private readonly double[,] _adjacencyMatrix = {
            {0, 82, 136, 133, 236},
            {82, 0, 144, 195, 317},
            {136, 114, 0, 158, 337},
            {133, 195, 158, 0, 196},
            {236, 317, 337, 196, 0}
        };

        [SetUp]
        public void Setup()
        {
            _simulatedAnnealingResults = new SimulatedAnnealing(_adjacencyMatrix).Anneal(Temperature, CoolingRate, AbsoluteTemperature);
        }

        [Test]
        public void Test()
        {
            Assert.That(_simulatedAnnealingResults.CitiesOrder, Is.EquivalentTo(_expectedCitiesOrder));
            Assert.That(_simulatedAnnealingResults.IterationCount, Is.EqualTo(ExpectedIterationCount));
            Assert.That(_simulatedAnnealingResults.ShortestDistance, Is.EqualTo(ExpectedDistance));
        }
    }
}