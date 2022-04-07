#ifndef _ZWOO_REQUEST_INTERCEPTOR_HPP
#define _ZWOO_REQUEST_INTERCEPTOR_HPP

#include "oatpp/web/server/interceptor/RequestInterceptor.hpp"

class ZwooRequestInterceptor
    : public oatpp::web::server::interceptor::RequestInterceptor
{
  public:
    using IncomingRequest = oatpp::web::protocol::http::incoming::Request;
    using OutgoingResponse = oatpp::web::protocol::http::outgoing::Response;

    std::shared_ptr<OutgoingResponse>
    intercept( const std::shared_ptr<IncomingRequest> &request ) override;
};

#endif // _ZWOO_REQUEST_INTERCEPTOR_HPP
