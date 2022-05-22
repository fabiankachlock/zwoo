#ifndef _QUEUE_HPP_
#define _QUEUE_HPP_

#include <queue>
#include <mutex>
#include <condition_variable>

template<typename T>
class SynchronizedQueue
{
public:
    SynchronizedQueue() : _queue(std::queue<T>()) {}
    ~SynchronizedQueue() {}

    bool empty() 
    {
        std::lock_guard<std::mutex> l(_mutex);
        return _queue.empty();
    }

    void push(T const& val)
    {
        std::lock_guard<std::mutex> l(_mutex);
        bool wake_up = _queue.empty();
        _queue.push(val);
        if (wake_up) _convar.notify_one();
    }

    T pop()
    {
        std::unique_lock<std::mutex> u(_mutex);
        while (_queue.empty())
            _convar.wait(u);
        
        T val = _queue.front();
        _queue.pop();
        return val;
    }

private:
    std::queue<T> _queue;
    std::mutex _mutex;
    std::condition_variable _convar;
};

#endif