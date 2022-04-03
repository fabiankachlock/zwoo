#ifndef _AUTHORIZATIONHANDLER_HPP_
#define _AUTHORIZATIONHANDLER_HPP_

#include "oatpp/web/server/api/ApiController.hpp"

#include "Server/DatabaseComponent.hpp"

#include <spdlog/spdlog.h>

class UserAuthorizationObject : public oatpp::web::server::handler::AuthorizationObject {
public:

  UserAuthorizationObject(uint64_t puid, oatpp::String username, oatpp::String email, uint32_t wins)
    : puid(puid), username(username), email(email), wins(wins)
  {}

    uint64_t puid;
    oatpp::String username;
    oatpp::String email;
    uint32_t wins;
};

class ZwooAuthorizationHandler : public oatpp::web::server::handler::BearerAuthorizationHandler {

  typedef oatpp::web::protocol::http::HttpError HttpError;
  typedef oatpp::web::protocol::http::Status Status;

public:
    ZwooAuthorizationHandler(std::shared_ptr<Database> db);

    std::shared_ptr<AuthorizationObject> authorize(const oatpp::String& token) override;
private:
    std::shared_ptr<Database> db;
};
#endif // _AUTHORIZATIONHANDLER_HPP_
