#include "Helper.h"

#include <zwoo.h>
#include <ctime>
#include <iostream>
#include <unistd.h>
#include <stdlib.h>
#include <chrono>
#include <random>
#include <fmt/format.h>

std::string randomString(const int len)
{
    static const char alphanum[] =
        "0123456789"
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        "abcdefghijklmnopqrstuvwxyz";
    std::string tmp_s;
    tmp_s.reserve(len);

    std::mt19937 rng(std::chrono::steady_clock::now().time_since_epoch().count());

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

std::string generateVerificationEmailText(ulong puid, std::string code, std::string username)
{
    std::string link = ZWOO_DOMAIN + "/auth/verify?id=" + std::to_string(puid) + "&code=" + code;

    std::string text = fmt::format("\r\nHello {0},\r\nplease klick the link to verify your zwoo account.\r\n{1}\r\n\r\nThe confirmation expires with the end of the day\r\n(UTC + 01:00).\r\n\r\nIf you've got this E-Mail by accident or don't want to\r\nregister, please ignore it.\r\n\r\nⒸ ZWOO 2022", username, link);

    return text;
}
