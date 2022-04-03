#ifndef _ZWOO_RESPONSE_INTERCEPTOR_HPP_
#define _ZWOO_RESPONSE_INTERCEPTOR_HPP_

#include "oatpp/web/server/interceptor/ResponseInterceptor.hpp"

class ZwooResponseInterceptor : public oatpp::web::server::interceptor::ResponseInterceptor {
public:
    ZwooResponseInterceptor();

    typedef oatpp::web::protocol::http::incoming::Request IncomingRequest;
    typedef oatpp::web::protocol::http::outgoing::Response OutgoingResponse;

    std::shared_ptr<OutgoingResponse> intercept(const std::shared_ptr<IncomingRequest>& request,
                                                const std::shared_ptr<OutgoingResponse>& response) override;
};

#endif // _ZWOO_RESPONSE_INTERCEPTOR_HPP_
