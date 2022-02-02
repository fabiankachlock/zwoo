#include "Helper.h"

#include <ctime>
#include <iostream>
#include <unistd.h>
#include <stdlib.h>
#include <chrono>
#include <random>

std::string generateUniqueHash()
{

    return "sliejfjsdhfoidsjf";
}

std::string randomString(const int len)
{
    static const char alphanum[] =
        "0123456789"
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        "abcdefghijklmnopqrstuvwxyz";
    std::string tmp_s;
    tmp_s.reserve(len);

    std::mt19937 rng();

    for (int i = 0; i < len; ++i) {
        tmp_s += alphanum[randomNumberInRange(0, 512) % (sizeof(alphanum) - 1)];
    }

    return tmp_s;
}

int randomNumberInRange(int min, int max)
{
    std::mt19937 gen(std::chrono::steady_clock::now().time_since_epoch().count());
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
