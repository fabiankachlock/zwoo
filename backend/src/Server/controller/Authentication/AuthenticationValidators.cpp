#include "Server/controller/Authentication/AuthenticationValidators.h"

#include <regex>

bool isValidEmail( std::string email )
{
    if ( email.length( ) < 2 )
        return false;
    else if ( email.length( ) > 50 )
        return false;

    const std::regex reg(
        R"(^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$)" );
    std::cmatch m;
    return std::regex_match( email.c_str( ), m, reg );
}

bool isValidPassword( std::string password )
{
    if ( password.length( ) < 8 )
        return false;
    if ( password.length( ) > 50 )
        return false;

    std::cmatch m;
    const std::regex r1( R"([0-9]+)" );
    const std::regex r2( R"([!#$%&'*+/=?^_´{|}\-[\]]+)" );
    const std::regex r3( R"([a-zA-Z]+)" );

    return std::regex_search( password.c_str( ), m, r1 ) &&
           std::regex_search( password.c_str( ), m, r2 ) &&
           std::regex_search( password.c_str( ), m, r3 );
}

bool isValidUsername( std::string username )
{
    if ( username.length( ) < 4 )
        return false;
    if ( username.length( ) > 20 )
        return false;
    else
        return true;
}
