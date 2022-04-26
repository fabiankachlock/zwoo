#ifndef _AUTHENTICATION_CONTROLLER_HPP_
#define _AUTHENTICATION_CONTROLLER_HPP_

#include "Server/DatabaseComponent.hpp"
#include "Server/controller/Authentication/AuthenticationValidators.h"
#include "Server/controller/Authentication/ReCaptcha.h"
#include "Server/controller/error.h"
#include "Server/dto/AuthenticationDTO.hpp"
#include "Server/logger/logger.h"
#include "fmt/format.h"
#include "mailio/message.hpp"
#include "mailio/mime.hpp"
#include "mailio/smtp.hpp"
#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/macro/component.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/web/server/api/ApiController.hpp"

#include <boost/beast/core/detail/base64.hpp>
#include <iostream>
#include <sstream>

#include OATPP_CODEGEN_BEGIN( ApiController ) // <- Begin Codegen

class AuthenticationController : public oatpp::web::server::api::ApiController
{
  private:
    OATPP_COMPONENT( std::shared_ptr<Logger>, m_logger, "Backend" );
    OATPP_COMPONENT( std::shared_ptr<ZwooAuthorizationHandler>, authHandler );
    OATPP_COMPONENT( std::shared_ptr<Database>, m_database );

  public:
    AuthenticationController(
        const std::shared_ptr<ObjectMapper> &objectMapper )
        : oatpp::web::server::api::ApiController( objectMapper )
    {
        setDefaultAuthorizationHandler( authHandler );
    }

  public:
    static std::shared_ptr<AuthenticationController> createShared(
        OATPP_COMPONENT( std::shared_ptr<ObjectMapper>,
                         objectMapper ) // Inject objectMapper component here as
                                        // default parameter
    )
    {
        return std::make_shared<AuthenticationController>( objectMapper );
    }

    ENDPOINT( "GET", "/", helloWorld )
    {
        m_logger->log->debug( "/GET hello-world" );
        return createResponse( Status::CODE_200,
                               R"({"message": "Hello World!"})" );
    }
    ENDPOINT_INFO( helloWorld )
    {
        info->summary = "Hello World Testendpoint.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );
    }

    ENDPOINT( "POST", "auth/recaptcha", reCaptcha,
              BODY_STRING( String, token ) )
    {
        return createResponse( Status::CODE_200, verifyCaptcha( token ) );
    }
    ENDPOINT_INFO( reCaptcha )
    {
        info->description = "verify the reCaptcha with this Endpoint.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );
    }

    ENDPOINT( "POST", "auth/create", create,
              BODY_DTO( Object<CreateUserBodyDTO>, data ) )
    {
        m_logger->log->debug( "/POST create" );
        if ( !isValidEmail( data->email.getValue( "" ) ) )
            return createResponse(
                Status::CODE_400,
                constructErrorMessage( "Email Invalid!",
                                       e_Errors::INVALID_EMAIL ) );
        if ( !isValidUsername( data->username.getValue( "" ) ) )
            return createResponse(
                Status::CODE_400,
                constructErrorMessage( "Username Invalid!",
                                       e_Errors::INVALID_USERNAME ) );
        if ( !isValidPassword( data->password.getValue( "" ) ) )
            return createResponse(
                Status::CODE_400,
                constructErrorMessage( "Password Invalid!",
                                       e_Errors::INVALID_PASSWORD ) );
        if ( m_database->entryExists( "username",
                                      data->username.getValue( "" ) ) )
            return createResponse(
                Status::CODE_400,
                constructErrorMessage( "Username Already Exists!",
                                       e_Errors::USERNAME_ALREADY_TAKEN ) );
        if ( m_database->entryExists( "email", data->email.getValue( "" ) ) )
            return createResponse(
                Status::CODE_400,
                constructErrorMessage( "Email Already Exists!",
                                       e_Errors::EMAIL_ALREADY_TAKEN ) );


        if ( ZWOO_BETA )
        {
            if (data->code.getValue("") == "")
                return createResponse( Status::CODE_400,
                                        R"({"message": "no zwoo beta code!"})" );
            if (!m_database->verifieAndUseBetaCode(data->code.getValue("")))
                return createResponse( Status::CODE_400,
                                        R"({"message": "invalid BETA code!"})" );
        }
        auto ret = m_database->createUser( data->username.getValue( "" ),
                                           data->email.getValue( "" ),
                                           data->password.getValue( "" ) );

        try
        {
            m_logger->log->info( "new user:\n puid: {},\n code: {}", ret.puid,
                                 ret.code );
            // create mail message
            mailio::message msg;
            msg.from( mailio::mail_address(
                "zwoo auth",
                SMTP_HOST_EMAIL ) ); // set the correct sender
                                     // name and address
            msg.add_recipient( mailio::mail_address(
                "recipient",
                data->email ) ); // set the correct recipent name and address
            msg.subject( "Verify your ZWOO Account" );
            msg.content( generateVerificationEmailText( ret.puid, ret.code,
                                                        data->username ) );
            // msg.content("Hello World!");
            //  connect to server
            mailio::smtps conn( SMTP_HOST_URL, SMTP_HOST_PORT );
            // modify username/password to use real credentials
            conn.authenticate( SMTP_USERNAME, SMTP_PASSWORD,
                               mailio::smtps::auth_method_t::START_TLS );
            conn.submit( msg );
        }
        catch ( mailio::smtp_error &exc )
        {
            m_logger->log->error( "Email failed to send: {0}", exc.what( ) );
        }
        catch ( mailio::dialog_error &exc )
        {
            m_logger->log->error( "Email failed to send: {0}", exc.what( ) );
        }
        m_logger->log->info( "User successfully created!" );
        return createResponse( Status::CODE_200,
                               R"({"message": "Account Created"})" );
    }
    ENDPOINT_INFO( create )
    {
        info->description = "create a new User with this Endpoint.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );
    }

    ENDPOINT( "GET", "auth/verify", verify, QUERY( String, code, "code" ),
              QUERY( UInt64, puid, "id" ) )
    {
        m_logger->log->debug( "/GET verify" );
        if ( m_database->verifyUser( puid, code ) )
            return createResponse( Status::CODE_200,
                                   R"({"message": "Account Verified"})" );
        else
            return createResponse(
                Status::CODE_400,
                constructErrorMessage( "Account failed to verify",
                                       e_Errors::ACCOUNT_FAILED_TO_VERIFIED ) );
    }
    ENDPOINT_INFO( verify )
    {
        info->description = "verify Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );
    }

    ENDPOINT( "POST", "auth/login", login,
              BODY_DTO( Object<LoginUserDTO>, data ) )
    {
        m_logger->log->debug( "/POST login" );
        if ( !isValidEmail( data->email.getValue( "" ) ) )
            return createResponse(
                Status::CODE_400,
                constructErrorMessage( "Email Invalid!",
                                       e_Errors::INVALID_EMAIL ) );

        auto login = m_database->loginUser( data->email, data->password );

        if ( login.successful )
        {
            m_logger->log->info( "User {} successfully logged in!",
                                 login.puid );
            std::string out =
                encrypt( std::to_string( login.puid ) + "," + login.sid );
            std::vector<uint8_t> vec( out.begin( ), out.end( ) );
            out = encodeBase64( vec );
            auto c = fmt::format(
                "auth={0};Max-Age=604800;Domain={1};Path=/;HttpOnly{2}", out,
                ZWOO_DOMAIN, USE_SSL ? ";Secure" : "" );
            auto res = createResponse( Status::CODE_200,
                                       R"({"message": "Logged In"})" );
            res->putHeader( "Set-Cookie", c );
            return res;
        }
        else
        {
            m_logger->log->info( "failed login attempt! User with ID {} failed "
                                 "to login. Login error: {}",
                                 login.puid, login.error_code );
            return createResponse(
                Status::CODE_401,
                constructErrorMessage( "failed to login",
                                       (e_Errors)login.error_code ) );
        }
    }
    ENDPOINT_INFO( login )
    {
        info->description = "Login Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );
    }

    ENDPOINT( "GET", "auth/user", user,
              AUTHORIZATION( std::shared_ptr<UserAuthorizationObject>, usr ) )
    {
        m_logger->log->debug( "/GET User" );
        if ( usr )
        {
            auto rusr = GetUserResponseDTO::createShared( );
            rusr->username = usr->username;
            rusr->email = usr->email;
            rusr->wins = usr->wins;
            auto res = createDtoResponse( Status::CODE_200, rusr );
            return res;
        }
        return createResponse( Status::CODE_501, R"({"code": 100})" );
    }
    ENDPOINT_INFO( user )
    {
        info->description = "Get User data with this Endpoint.";

        info->addResponse<Object<GetUserResponseDTO>>( Status::CODE_200,
                                                       "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );

        info->addSecurityRequirement( "Cookie" );
    }

    ENDPOINT( "GET", "auth/logout", logout,
              AUTHORIZATION( std::shared_ptr<UserAuthorizationObject>, usr ) )
    {
        m_logger->log->debug( "/GET Logout" );

        if ( usr )
        {
            m_database->updateStringField( "email", usr->email, "sid", "" );
            auto res = createResponse( Status::CODE_200,
                                       R"({"message": "user logged out"})" );
            auto c =
                fmt::format( "auth=;Max-Age=0;Domain={0};Path=/;HttpOnly{1}",
                             ZWOO_DOMAIN, USE_SSL ? ";Secure" : "" );
            res->putHeader( "Set-Cookie", c );
            return res;
        }
        else
            return createResponse( Status::CODE_501, R"({"code": 100})" );
    }
    ENDPOINT_INFO( logout )
    {
        info->description = "Get User data with this Endpoint.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );

        info->addSecurityRequirement( "Cookie" );
    }

    ENDPOINT( "POST", "auth/delete", deleteUser,
              BODY_DTO( Object<DeleteUserDTO>, data ),
              AUTHORIZATION( std::shared_ptr<UserAuthorizationObject>, usr ) )
    {
        m_logger->log->debug( "/GET delete" );

        if ( usr )
        {
            if ( m_database->deleteUser( usr->puid, data->password ) )
            {
                m_logger->log->info( "User {} successfully deleted",
                                     usr->puid );
                auto res = createResponse(
                    Status::CODE_200, R"({ "message": "User Deleted!" })" );
                auto c = fmt::format(
                    "auth=;Max-Age=0;Domain={0};Path=/;HttpOnly{1}",
                    ZWOO_DOMAIN, USE_SSL ? ";Secure" : "" );
                res->putHeader( "Set-Cookie", c );
                return res;
            }
        }

        return createResponse( Status::CODE_501, R"({"code": 100})" );
    }
    ENDPOINT_INFO( deleteUser )
    {
        info->description = "Login Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>( Status::CODE_200,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_404,
                                              "application/json" );
        info->addResponse<Object<StatusDto>>( Status::CODE_500,
                                              "application/json" );

        info->addSecurityRequirement( "Cookie" );
    }
};

#include OATPP_CODEGEN_END( ApiController ) // <- End Codegen

#endif
