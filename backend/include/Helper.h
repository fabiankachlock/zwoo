#ifndef _HELPER_H_
#define _HELPER_H_

#include <string>

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

#endif // _HELPER_H_
