using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tsp.Data.UnitTests.AdjacencyMatrixProviderTests
{
    public class GetAdjacencyMatrixResultsAreValid
    {
        private const int Expected11Value = 0;
        private const int Expected12Value = 131;

        private readonly IAdjacencyMatrixProvider _adjacencyMatrixProvider = new AdjacencyMatrixProvider();

        private Point2d[] _points;

        [SetUp]
        public void Setup()
        {
            _points = new[]
            {
                new Point2d(286, 142),
                new Point2d(361, 83),
                new Point2d(234, 48),
                new Point2d(156, 61),
            };
        }

        [Test]
        public void FromPointArray()
        {
            var matrix = _adjacencyMatrixProvider.GetAdjacencyMatrix(_points);

            Assert.That(matrix[1, 1], Is.EqualTo(Expected11Value));
            Assert.That((int)matrix[1, 2], Is.EqualTo(Expected12Value));
        }
    }
}
