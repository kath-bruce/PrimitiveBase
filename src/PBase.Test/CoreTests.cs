using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PBase.Test
{
    public class CoreTests
    {
        [Fact]
        public void TestSafeLock()
        {
            var timeout = TimeSpan.FromSeconds(15.0);
            SafeLock.Timeout = timeout / 3;

            var safeLock = new SafeLock();

            Assert.True(safeLock.TryEnter());

            safeLock.Exit();

            Task.Run(() =>
            {
                using (safeLock.Enter())
                {
                    Thread.Sleep(timeout);
                }
            });

            Thread.Sleep(1000);

            Assert.False(safeLock.TryEnter());

            Assert.Throws<TimeoutException>(()=> safeLock.Enter());

            Task.Run(() =>
            {
                using (safeLock.Enter())
                {
                    Thread.Sleep(timeout);
                }
            });

            Thread.Sleep(1000);

            Assert.Throws<SafeLockException>(() => safeLock.Exit());
        }
    }
}