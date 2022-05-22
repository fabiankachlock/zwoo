#ifndef _HELPER_H_
#define _HELPER_H_

#include <string>
#include <vector>

class UIDGenerator
{
  private:
    uint32_t cid;
    bool isInitialized;

  public:
    UIDGenerator( ) { isInitialized = false; }
    UIDGenerator( uint32_t start ) : cid( start ), isInitialized( true ) {}

    bool IsInitialized( ) { return isInitialized; }

    void Init( uint32_t start )
    {
        isInitialized = true;
        cid = start;
    }
    uint32_t GetID( )
    {
        if ( !isInitialized )
            Init( 0 );
        return ++cid;
    }
};

std::string generateUniqueHash( );
int randomNumberInRange( int min, int max );
std::string randomString( const int len );
std::string randomNDigitNumber( int n );

std::string encrypt( std::string str );
std::string decrypt( std::string str );

std::string decodeBase64( const std::string &val );
std::string encodeBase64( std::vector<uint8_t> data );

#endif // _HELPER_H_
