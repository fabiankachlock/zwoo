using Zwoo.GameEngine.Helper;

namespace Zwoo.GameEngine.Tests.Helper;

public class AsyncExecutionQueueTests
{
    [Test]
    public void ShouldBeAsync()
    {
        var q = new AsyncExecutionQueue();
        List<int> nums = [0];

        var _ = q.Enqueue(() =>
        {
            nums.Add(1);
        });
        _ = q.Enqueue(() =>
        {
            Thread.Sleep(20);
            nums.Add(10);
        });

        Assert.That(nums, Is.EqualTo(new List<int>() { 0 }));
        q.Start();
        Assert.That(nums, Is.EqualTo(new List<int>() { 0 }));
        Thread.Sleep(10);
        Assert.That(nums, Is.EqualTo(new List<int>() { 0, 1 }));
        Thread.Sleep(20);
        Assert.That(nums, Is.EqualTo(new List<int>() { 0, 1, 10 }));
    }
}
