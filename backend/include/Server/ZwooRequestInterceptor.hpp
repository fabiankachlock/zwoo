#ifndef _ZWOO_REQUEST_INTERCEPTOR_HPP
#define _ZWOO_REQUEST_INTERCEPTOR_HPP

#include "oatpp/web/server/interceptor/RequestInterceptor.hpp"

class ZwooRequestInterceptor : public oatpp::web::server::interceptor::RequestInterceptor {
public:

    typedef oatpp::web::protocol::http::incoming::Request IncomingRequest;
    typedef oatpp::web::protocol::http::outgoing::Response OutgoingResponse;

    std::shared_ptr<OutgoingResponse> intercept(const std::shared_ptr<IncomingRequest>& request) override;
};

#endif // _ZWOO_REQUEST_INTERCEPTOR_HPP
