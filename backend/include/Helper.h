#ifndef _HELPER_H_
#define _HELPER_H_

#include <string>
#include <vector>

typedef unsigned long ulong;

class UIDGenerator
{
private:
    ulong cid;
    bool isInitialized;
public:
    UIDGenerator() {
        isInitialized = false;
    }
    UIDGenerator(ulong start) : cid(start), isInitialized(true) {}

    bool IsInitialized() {
        return isInitialized;
    }

    void Init(ulong start)
    {
        isInitialized = true;
        cid = start;
    }
    ulong GetID()
    {
        if (!isInitialized)
            Init(0);
        return ++cid;
    }

};

std::string generateUniqueHash();
int randomNumberInRange(int min, int max);
std::string randomString(const int len);
std::string randomNDigitNumber(int n);
std::string generateVerificationEmailText(ulong puid, std::string code, std::string username);

std::string encrypt(std::string str);
std::string decrypt(std::string str);

std::string decodeBase64(const std::string &val);
std::string encodeBase64(std::vector<uint8_t> data);

#endif // _HELPER_H_
