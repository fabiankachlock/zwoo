#include "Helper.h"

#include <boost/algorithm/string.hpp>
#include <boost/archive/iterators/base64_from_binary.hpp>
#include <boost/archive/iterators/binary_from_base64.hpp>
#include <boost/archive/iterators/transform_width.hpp>
#include <chrono>
#include <cstdlib>
#include <ctime>
#include <fmt/format.h>
#include <iostream>
#include <random>
#include <unistd.h>
#include <zwoo.h>

std::string randomString( const int len )
{
    static const char alphanum[] = "0123456789"
                                   "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                                   "abcdefghijklmnopqrstuvwxyz";
    std::string tmp_s;
    tmp_s.reserve( len );

    std::mt19937 rng(
        std::chrono::steady_clock::now( ).time_since_epoch( ).count( ) );

    for ( int i = 0; i < len; ++i )
    {
        tmp_s += alphanum[ randomNumberInRange( 0, 512 ) %
                           ( sizeof( alphanum ) - 1 ) ];
    }

    return tmp_s;
}

int randomNumberInRange( int min, int max )
{
    std::mt19937 gen(
        std::chrono::steady_clock::now( ).time_since_epoch( ).count( ) );
    std::uniform_int_distribution<> dis( min, max );
    return dis( gen );
}

std::string randomNDigitNumber( int n )
{
    std::string out = "";
    out.reserve( n );

    std::random_device rd;
    std::mt19937 gen( rd( ) );
    std::uniform_int_distribution<> dis( 0, 9 );

    for ( int i = 0; i < n; ++i )
        out += (char)( dis( gen ) + 48 );

    return out;
}

std::string generateVerificationEmailText( ulong puid, std::string code,
                                           std::string username )
{
    std::string link = ( USE_SSL ? "https://" : "http://" ) + ZWOO_DOMAIN +
                       "/auth/verify?id=" + std::to_string( puid ) +
                       "&code=" + code;

    // std::string text = fmt::format(
    //     "\r\nHello {0},\r\nplease click the link to verify your zwoo "
    //     "account.\r\n{1}\r\n\r\nThe confirmation expires with the end of this
    //     " "day\r\n(UTC + 01:00).\r\n\r\nIf you've got this E-Mail by accident
    //     or " "don't want to\r\nregister, please ignore it.\r\n\r\nⒸ ZWOO
    //     2022", username, link );

    std::string text = fmt::format(
        /*        "--section\r\n"
                "Content-Type: text/plain\r\n"
                "\r\nHello {0},\r\nplease click the link to verify your zwoo "
                "account.\r\n{1}\r\n\r\nThe confirmation expires with the end of
           this " "day\r\n(UTC + 01:00).\r\n\r\nIf you've got this E-Mail by
           accident or " "don't want to\r\nregister, please ignore it.\r\n\r\nⒸ
           ZWOO 2022\r\n"
                "--section\r\n"
                "Content-Type: text/html\r\n"*/
        "\r\n<html style=\"background-color: #363847; font-size: 16px; "
        "font-family: sans-serif; width: 100%; height: 100%;\">\r\n"
        "<head><meta charset=\"utf-8\"><title>ZWOO "
        "VERFICATION</title></head>\r\n"
        "<body style=\"margin-top: 7%\">\r\n"
        "<h1 style=\"color: #3066BE; text-align: center; margin-bottom: 0; "
        "margin-top: 1%; font-size: 450%\">ZWOO</h1>\r\n"
        "<p style=\"color: #3066BE; text-align: center; margin-top: 0; "
        "font-size: 280%\">the second challenge</p>\r\n)"
        "<p style=\"color: white; text-align: left; margin-top: 7%;  "
        "font-size: 250%;  margin-left: 10%\"> Dear {0},</br>please verify "
        "your account via the <b>button</b> </br> or press <b>the link</b> "
        "below:</p>\r\n"
        "<div style=\"display: flex; justify-content: center; margin-top: "
        "5%\"><a href=\"{1}\" target=\"_blank\"> <button style=\"background: "
        "#3066BE; border: 0;color: white; text-align: center; font-size: 250%; "
        "padding: 0.5em 1em; border-radius: "
        "0.35em;\">verify</button></a></div>\r\n"
        "<div> <a href=\"{1}\" target=\"_blank\"><p style=\"color: white; "
        "text-align: left; margin-top: 7%;  font-size: 180%;  margin-left: "
        "10%\">{1}</p>\r\n"
        "</a><p style=\"color: white; text-align: left; margin-top: 13%;  "
        "font-size: 200%;  margin-left: 10%\"> The confirmation expires with "
        "the end of the day</br>(UTC + 01:00). </p> <p style=\"color: white; "
        "text-align: left; margin-top: 7%;  font-size: 200%;  margin-left: "
        "10%\"> If you've got this E-Mail by accident or don't want to</br> "
        "register, please ignore it. </p>\r\n"
        "</div><div style=\"color: white; text-align: left; margin-top: 18%;  "
        "font-size: 150%;  margin-left: 10%\"> &#x24B8; ZWOO "
        "2022</div></body></html>\r\n"
        //         "--section\r\n"
        ,
        username, link );

    return text;
}

std::string encrypt( std::string str )
{
    std::string key = ZWOO_ENCRYPTION_KEY;
    std::string out = str;
    for ( int i = 0; i < out.size( ); i++ )
        out[ i ] = out[ i ] ^ key[ i % key.length( ) ];
    return out;
}

std::string decrypt( std::string str ) { return encrypt( str ); }

const std::string base64_padding[] = { "", "==", "=" };

std::string decodeBase64( const std::string &val )
{
    using namespace boost::archive::iterators;
    using It =
        transform_width<binary_from_base64<std::string::const_iterator>, 8, 6>;
    return boost::algorithm::trim_right_copy_if(
        std::string( It( std::begin( val ) ), It( std::end( val ) ) ),
        []( char c ) { return c == '\0'; } );
}
std::string encodeBase64( std::vector<uint8_t> data )
{
    using namespace boost::archive::iterators;
    typedef std::vector<uint8_t>::const_iterator iterator_type;
    typedef base64_from_binary<transform_width<iterator_type, 6, 8>> base64_enc;
    std::stringstream ss;
    std::copy( base64_enc( data.begin( ) ), base64_enc( data.end( ) ),
               std::ostream_iterator<char>( ss ) );
    ss << base64_padding[ data.size( ) % 3 ];
    return ss.str( );
}
