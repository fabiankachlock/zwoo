using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwooGameLogic.Helper;


public sealed class AsyncExecutionQueue
{

    private List<Action> _queue;

    private bool _isRunning = false;
    private WaitLock<bool> _isExecuting = new WaitLock<bool>(false);

    public AsyncExecutionQueue()
    {
        _queue = new List<Action>();
    }

    /// <summary>
    /// append an action onto the queue and wait until its completed
    /// </summary>
    /// <param name="executeable"></param>
    /// <returns></returns>
    public async Task Enqueue(Action executeable)
    {
        Awaitable<bool> awaiter = new Awaitable<bool>();

        lock (_queue)
        {
            _queue.Add(() =>
            {
                executeable();
                awaiter.Complete(true);
            });
        }
        TryExecute();

        await awaiter.Wait();
    }

    /// <summary>
    /// add an action to be executed next and wait until its completed
    /// </summary>
    /// <param name="executeable"></param>
    /// <returns></returns>
    public async Task Intercept(Action executeable)
    {
        Awaitable<bool> awaiter = new Awaitable<bool>();

        lock (_queue)
        {
            _queue.Insert(0, () =>
            {
                executeable();
                awaiter.Complete(true);
            });
        }
        TryExecute();

        await awaiter.Wait();
    }

    /// <summary>
    /// start executing the items in the queue
    /// </summary>
    public void Start()
    {
        if (_isRunning == false)
        {
            _isRunning = true;
            TryExecute();
        }
    }

    /// <summary>
    /// stop the execution of items in the queue
    /// </summary>
    public void Stop()
    {
        _isRunning = false;
    }

    /// <summary>
    /// clear all currently queued items
    /// </summary>
    public void Clear()
    {
        lock (_queue)
        {
            _queue.Clear();
        }
    }

    private void TryExecute()
    {
        lock (_isExecuting)
        {
            if (!_isExecuting.Value)
            {
                // is not already executing
                Task.Run(() => Execute());
            }
        }
    }

    private void Execute()
    {
        // start execution
        lock (_isExecuting)
        {
            _isExecuting.Value = true;
        }

        Action item;
        lock (_queue)
        {
            // check if execution is allowed
            if (!_isRunning || _queue.Count == 0)
            {
                lock (_isExecuting)
                {
                    _isExecuting.Value = false;
                }
                return;
            }
            else
            {
                // dequeue first item
                item = _queue[0];
                _queue.RemoveAt(0);
            }
        }

        // execute item if exists
        if (item != null)
        {
            item();
        }

        // start next execution
        Execute();
    }
}
