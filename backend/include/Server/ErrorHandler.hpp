#ifndef _ERRORHANDLER_HPP_
#define _ERRORHANDLER_HPP_

#include "dto/StatusDTO.hpp"
#include "oatpp/web/protocol/http/outgoing/ResponseFactory.hpp"
#include "oatpp/web/server/handler/ErrorHandler.hpp"

class ErrorHandler : public oatpp::web::server::handler::ErrorHandler
{
  private:
    using OutgoingResponse = oatpp::web::protocol::http::outgoing::Response;
    using Status = oatpp::web::protocol::http::Status;
    using ResponseFactory =
        oatpp::web::protocol::http::outgoing::ResponseFactory;

    std::shared_ptr<oatpp::data::mapping::ObjectMapper> m_objectMapper;

  public:
    ErrorHandler( const std::shared_ptr<oatpp::data::mapping::ObjectMapper>
                      &objectMapper );

    std::shared_ptr<OutgoingResponse>
    handleError( const Status &status, const oatpp::String &message,
                 const Headers &headers ) override;
};

#endif // _ERRORHANDLER_HPP_
