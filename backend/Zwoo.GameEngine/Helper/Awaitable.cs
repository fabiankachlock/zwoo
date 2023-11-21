using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zwoo.GameEngine.Helper;

internal sealed class Awaitable<T>
{
    private TaskCompletionSource<T> _promise;

    public Awaitable()
    {
        _promise = new TaskCompletionSource<T>();
    }

    public Task<T> Wait()
    {
        return _promise.Task;
    }

    public void Complete(T value)
    {
        _promise.SetResult(value);
    }
}
