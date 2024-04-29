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

        Assert.That(count, Is.EqualTo(0));
        q.Start();
        Thread.Sleep(50);
        Assert.That(count, Is.EqualTo(1));
        Thread.Sleep(100);
        Assert.That(count, Is.EqualTo(11));
        Thread.Sleep(100);
        Assert.That(count, Is.EqualTo(111));
    }
}
