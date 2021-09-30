#ifndef _QUEUE_H_
#define _QUEUE_H_

#include <queue>
#include <optional>
#include <mutex>

namespace Backend
{
    template <class T>
    class Queue
    {
    private:
        std::queue<T> _queue;
        mutable std::mutex _mutex;

        // Removed from pubic to prevent races between this and pop()
        bool empty() const { return _queue.empty(); }

    public:
        Queue() = default;
        Queue(const Queue<T> &) = delete;
        Queue &operator=(const Queue<T> &) = delete;

        Queue(Queue<T> &&other)
        {
            std::lock_guard<std::mutex> lock(_mutex);
            _queue = std::move(other._queue);
        }

        virtual ~Queue() {}

    public:
        unsigned long size() const
        {
            std::lock_guard<std::mutex> lock(_mutex);
            return _queue.size();
        }

        std::optional<T> pop()
        {
            std::lock_guard<std::mutex> lock(_mutex);
            if (_queue.empty())
                return {};

            T tmp = _queue.front();
            _queue.pop();
            return tmp;
        }

        void push(const T &item)
        {
            std::lock_guard<std::mutex> lock(_mutex);
            _queue.push(item);
        }

        std::optional<T> peek()
        {
            std::lock_guard<std::mutex> lock(_mutex);
            if (_queue.empty())
                return {};

            return _queue.front();
        }
    };
}
#endif