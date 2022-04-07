#include "Server/ZwooResponseInterceptor.hpp"

#include "zwoo.h"

ZwooResponseInterceptor::ZwooResponseInterceptor( ) {}

std::shared_ptr<ZwooResponseInterceptor::OutgoingResponse>
ZwooResponseInterceptor::intercept(
    const std::shared_ptr<IncomingRequest> &request,
    const std::shared_ptr<OutgoingResponse> &response )
{
    response->putHeaderIfNotExists(
        oatpp::web::protocol::http::Header::CORS_ORIGIN, ZWOO_CORS );
    response->putHeaderIfNotExists(
        oatpp::web::protocol::http::Header::CORS_METHODS,
        "GET, POST, OPTIONS" );
    response->putHeaderIfNotExists(
        oatpp::web::protocol::http::Header::CORS_HEADERS,
        "DNT, User-Agent, X-Requested-With, If-Modified-Since, Cache-Control, "
        "Content-Type, Range, Authorization" );
    response->putHeaderIfNotExists(
        oatpp::web::protocol::http::Header::CORS_MAX_AGE, "1728000" );
    response->putHeaderIfNotExists( "Access-Control-Allow-Credentials",
                                    "true" );
    return response;
}
