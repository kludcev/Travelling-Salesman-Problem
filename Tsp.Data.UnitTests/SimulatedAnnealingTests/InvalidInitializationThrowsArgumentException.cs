using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tsp.Data.UnitTests.SimulatedAnnealingTests
{
    public class InvalidInitializationThrowsArgumentException
    {
        [Test]
        public void Test()
        {
            Assert.That(() => { new SimulatedAnnealing(null); }, Throws.ArgumentException);
            Assert.That(() => { new SimulatedAnnealing(new double[0,0]); }, Throws.ArgumentException);
        }
    }
}
