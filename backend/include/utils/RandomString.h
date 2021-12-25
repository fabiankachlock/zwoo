#ifndef _RANDOM_STRING_H_
#define _RANDOM_STRING_H_

#include <ctime>
#include <iostream>
#include <unistd.h>

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
}// namespace Backend


#endif