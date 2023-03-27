using ZwooGameLogic.Helper;

namespace ZwooGameLogic.Tests.Helper;

public class AsyncExecutionQueueTests
{
    [Fact]
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

        Assert.Equal(0, count);
        q.Start();
        Thread.Sleep(50);
        Assert.Equal(1, count);
        Thread.Sleep(100);
        Assert.Equal(11, count);
        Thread.Sleep(100);
        Assert.Equal(111, count);
    }
}
