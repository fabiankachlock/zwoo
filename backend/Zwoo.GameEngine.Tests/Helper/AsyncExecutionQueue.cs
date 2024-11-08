using Zwoo.GameEngine.Helper;

namespace Zwoo.GameEngine.Tests.Helper;

public class AsyncExecutionQueueTests
{
    [Test]
    public void ShouldBeAsync()
    {
        var q = new AsyncExecutionQueue();
        List<int> nums = new List<int> { 0 };

        var _ = q.Enqueue(() =>
        {
            Task.Delay(10).Wait();
            nums.Add(1);
        });
        _ = q.Enqueue(() =>
        {
            Task.Delay(50).Wait();
            nums.Add(10);
        });

        Assert.That(nums, Is.EqualTo(new List<int>() { 0 }));
        q.Start();
        Assert.That(nums, Is.EqualTo(new List<int>() { 0 }));

        Task.Delay(20).Wait();
        Assert.That(nums, Is.EqualTo(new List<int>() { 0, 1 }));

        Task.Delay(60).Wait();
        Assert.That(nums, Is.EqualTo(new List<int>() { 0, 1, 10 }));
    }
}
