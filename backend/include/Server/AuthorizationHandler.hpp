#ifndef _AUTHORIZATIONHANDLER_HPP_
#define _AUTHORIZATIONHANDLER_HPP_

#include "Server/DatabaseComponent.hpp"
#include "oatpp/web/server/api/ApiController.hpp"

#include <spdlog/spdlog.h>

class UserAuthorizationObject
    : public oatpp::web::server::handler::AuthorizationObject
{
  public:
    UserAuthorizationObject( uint64_t puid, oatpp::String username,
                             oatpp::String email, uint32_t wins )
        : puid( puid ), username( username ), email( email ), wins( wins )
    {
    }

    uint64_t puid;
    oatpp::String username;
    oatpp::String email;
    uint32_t wins;
};

class ZwooAuthorizationHandler
    : public oatpp::web::server::handler::BearerAuthorizationHandler
{

    using HttpError = oatpp::web::protocol::http::HttpError;
    using Status = oatpp::web::protocol::http::Status;

  public:
    ZwooAuthorizationHandler( std::shared_ptr<Database> db );

    std::shared_ptr<AuthorizationObject>
    authorize( const oatpp::String &token ) override;

  private:
    std::shared_ptr<Database> db;
};
#endif // _AUTHORIZATIONHANDLER_HPP_
