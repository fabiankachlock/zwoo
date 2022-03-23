#ifndef _THREAD_H_
#define _THREAD_H_

#include <chrono>
#include <thread>
#include <functional>

void periodicFunction(std::function<void(void)> func, unsigned int interval)
{
    std::thread([func, interval]()
    {
        while (true)
        {
            auto t = std::chrono::steady_clock::now() + std::chrono::milliseconds(interval);
            func();
            std::this_thread::sleep_until(t);
        }
    }).detach();
}

void TimedFunction(std::function<void(void)> func, unsigned int interval)
{
    std::thread([func, interval]()
    {
        while (true)
        {
            auto t = std::chrono::steady_clock::now() + std::chrono::milliseconds(interval);
            func();
            std::this_thread::sleep_until(t);
        }
    }).detach();
}

#endif // _THREAD_H_
