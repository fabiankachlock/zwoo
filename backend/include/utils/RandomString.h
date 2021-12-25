#ifndef _RANDOM_STRING_H_
#define _RANDOM_STRING_H_

#include <ctime>
#include <iostream>
#include <unistd.h>
#include <random>

namespace Backend {
    std::string randomString(const int len) 
    {
        static const char alphanum[] =
                "0123456789"
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                "abcdefghijklmnopqrstuvwxyz";
        std::string tmp_s;
        tmp_s.reserve(len);

        for (int i = 0; i < len; ++i) {
            tmp_s += alphanum[rand() % (sizeof(alphanum) - 1)];
        }

        return tmp_s;
    }

    int randomNumberInRange(int min, int max)
    {
        std::random_device rd;
        std::mt19937 gen(rd());
        std::uniform_int_distribution<> dis(min, max);
        return dis(gen);
    }

    std::string randomNDigitNumber(int n)
    {
        std::string out = "";
        out.reserve(n);

        std::random_device rd;
        std::mt19937 gen(rd());
        std::uniform_int_distribution<> dis(0, 9);

        for (int i = 0; i < n; ++i)
            out += (char)(dis(gen) + 48);

        return out;
    }
}// namespace Backend


#endif