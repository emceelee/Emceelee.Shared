using Microsoft.VisualStudio.TestTools.UnitTesting;

using Emceelee.Shared;

namespace Emceelee.Shared.Test
{
    [TestClass]
    public class PerformanceLoggerTest
    {
        [TestMethod]
        public void CalculateMetricsTest()
        {
            var count = 100;
            var logger = new PerformanceLogger();
            logger.Start();

            for(int i = 0; i < count; ++i)
            {
                logger.Start("Fast");
                logger.Stop("Fast");
                logger.Start("Slow");
                System.Threading.Thread.Sleep(1);
                logger.Stop("Slow");
            }
            logger.Stop();

            var metrics = logger.CalculateMetrics();

            Assert.AreEqual(2, metrics.Count);
            foreach(var m in metrics)
            {
                Assert.AreEqual(count, m.Value.Count);
                Assert.IsTrue(m.Value.PercentTotal >= 0);
                Assert.IsTrue(m.Value.PercentTotal <= 100);
            }
        }
    }
}
