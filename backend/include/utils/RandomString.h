#ifndef _RANDOM_STRING_H_
#define _RANDOM_STRING_H_

#include <ctime>
#include <iostream>
#include <unistd.h>
#include <stdlib.h>
#include <chrono>
#include <random>

namespace Backend {

    int randomNumberInRange(int min, int max);

    std::string randomString(const int len);

    std::string randomNDigitNumber(int n);

}// namespace Backend


#endif