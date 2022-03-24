#ifndef _THREAD_H_
#define _THREAD_H_

#include <chrono>
#include <time.h>
#include <thread>
#include <functional>

void periodicFunction(std::function<void(void)> func, unsigned int interval);
void timedFunction(std::function<void(void)> func, uint8_t hour, uint8_t minute);

#endif // _THREAD_H_
