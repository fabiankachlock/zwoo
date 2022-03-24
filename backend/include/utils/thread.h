#ifndef _THREAD_H_
#define _THREAD_H_

#include <chrono>
#include <time.h>
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

void timedFunction(std::function<void(void)> func, uint8_t hour, uint8_t minute)
{
    int t = (minute + (hour * 60)) * 60;
    std::thread([func, t]()
    {
        while (true)
        {
            time_t theTime = time(NULL);
            struct tm *aTime = localtime(&theTime);

            int ct = aTime->tm_sec + ((aTime->tm_min + (aTime->tm_hour * 60)) * 60);
            int rt;

            if (ct < t)
            {
                rt = t - ct;
            }
            else if (ct > t)
            {
                rt = 86400 - ct + t;
            }
            else
            {
                rt = 86400;
            }

            std::this_thread::sleep_for(std::chrono::seconds(rt));

            func();
        }
    }).detach();
}

#endif // _THREAD_H_
