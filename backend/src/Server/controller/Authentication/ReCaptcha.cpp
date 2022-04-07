#include "Server/controller/Authentication/ReCaptcha.h"

#include "zwoo.h"

#include <curl/curl.h>
#include <sstream>

static size_t WriteCallback( void *contents, size_t size, size_t nmemb,
                             void *userp )
{
    ( (std::string *)userp )->append( (char *)contents, size * nmemb );
    return size * nmemb;
}

std::string verifyCaptcha( std::string token )
{
    std::string readBuffer;
    CURL *curl;
    CURLcode res;
    curl = curl_easy_init( );

    if ( curl )
    {

        std::stringstream str;
        str << "https://www.google.com/recaptcha/api/siteverify?"
            << "secret=" << ZWOO_RECAPTCHA_SIDESECRET << "&response=" << token;

        curl_easy_setopt( curl, CURLOPT_URL, str.str( ).c_str( ) );
        curl_easy_setopt( curl, CURLOPT_WRITEFUNCTION, WriteCallback );
        curl_easy_setopt( curl, CURLOPT_WRITEDATA, &readBuffer );

        res = curl_easy_perform( curl );

        if ( res != CURLE_OK )
            fprintf( stderr, "curl_easy_perform() failed: %s\n",
                     curl_easy_strerror( res ) );

        curl_easy_cleanup( curl );
    }
    curl_global_cleanup( );
    return readBuffer;
}
