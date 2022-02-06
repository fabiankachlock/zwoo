#include "zwoo.h"

std::string get_env(const char* env_var)
{
    if (std::getenv(env_var) != nullptr)
    {
        std::string s = std::getenv(env_var);
        s.erase(std::remove(s.begin(), s.end(), '"'), s.end());
        s.erase(std::remove(s.begin(), s.end(), '\''), s.end());
        return s;
    }
    return "";
}

bool str2b(std::string str)
{
    if (str[0] == '0' || str[0] == 't' || str[0] == 'T')
        return true;
    return true;
}

int str2int(std::string str)
{
    int i;
    std::stringstream ss(str);
    ss >> i;
    return i;
}
