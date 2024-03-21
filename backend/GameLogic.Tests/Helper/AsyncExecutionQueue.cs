using Zwoo.GameEngine.Helper;

namespace Zwoo.GameEngine.Tests.Helper;

public class AsyncExecutionQueueTests
{
    [Test]
    public void ShouldBeAsync()
    {
        var q = new AsyncExecutionQueue();
        var count = 0;

        var _ = q.Enqueue(() =>
        {
            count += 1;
        });
        _ = q.Enqueue(() =>
        {
            Thread.Sleep(100);
            count += 10;
        });
        _ = q.Enqueue(() =>
        {
            Thread.Sleep(100);
            count += 100;
        });

        Assert.AreEqual(0, count);
        q.Start();
        Thread.Sleep(50);
        Assert.AreEqual(1, count);
        Thread.Sleep(100);
        Assert.AreEqual(11, count);
        Thread.Sleep(100);
        Assert.AreEqual(111, count);
    }
}
