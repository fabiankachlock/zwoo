#include "Server/ErrorHandler.hpp"

#include "Server/controller/error.h"
#include "spdlog/spdlog.h"
#include "zwoo.h"

ErrorHandler::ErrorHandler(
    const std::shared_ptr<oatpp::data::mapping::ObjectMapper> &objectMapper )
    : m_objectMapper( objectMapper )
{
}

std::shared_ptr<ErrorHandler::OutgoingResponse>
ErrorHandler::handleError( const Status &status, const oatpp::String &message,
                           const Headers &headers )
{

    std::shared_ptr<OutgoingResponse> response;

    if ( status.code < 500 )
        response =
            ResponseFactory::createResponse( status, message, m_objectMapper );
    else
    {
        auto logger = spdlog::get( "BED" );
        logger->error( "Backend error: {}", message->c_str( ) );

        auto error = StatusDto::createShared( );
        error->status = "ERROR";
        error->code = status.code;
        error->message =
            constructErrorMessage( "Backend Error", e_Errors::BACKEND_ERROR );

        response =
            ResponseFactory::createResponse( status, error, m_objectMapper );
    }

    for ( const auto &pair : headers.getAll( ) )
        response->putHeader( pair.first.toString( ), pair.second.toString( ) );

    return response;
}
