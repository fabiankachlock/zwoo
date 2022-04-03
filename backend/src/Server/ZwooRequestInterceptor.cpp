#include "Server/ZwooRequestInterceptor.hpp"

#include <spdlog/spdlog.h>

std::shared_ptr<ZwooRequestInterceptor::OutgoingResponse> ZwooRequestInterceptor::intercept(const std::shared_ptr<IncomingRequest>& request)
{
    // Replace AUTHORIZATION header with auth Cookie data
    auto cookie_header = request->getHeader("Cookie");
    if (cookie_header != nullptr)
    {
        std::string cookie = cookie_header.getValue("");
        auto spos = cookie.find("auth=");
        if (spos < 0 || spos > cookie.length())
            return nullptr;

        auto data = cookie.substr(spos + 5, cookie.find(';', spos) - spos - 5);

        request->putOrReplaceHeader(oatpp::web::protocol::http::Header::AUTHORIZATION, "Bearer " + data);
    }
    return nullptr;
}
