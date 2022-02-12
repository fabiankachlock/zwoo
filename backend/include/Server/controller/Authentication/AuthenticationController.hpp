#pragma once

#ifndef _AUTHENTICATION_CONTROLLER_HPP_
#define _AUTHENTICATION_CONTROLLER_HPP_

#include <sstream>
#include <iostream>

#include <boost/beast/core/detail/base64.hpp>
#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/macro/component.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/web/server/api/ApiController.hpp"

#include "mailio/message.hpp"
#include "mailio/mime.hpp"
#include "mailio/smtp.hpp"

#include "fmt/format.h"

#include "Server/logger/logger.h"
#include "Server/DatabaseComponent.hpp"
#include "Server/controller/Authentication/ReCaptcha.h"
#include "Server/dto/AuthenticationDTO.hpp"
#include "Server/controller/Authentication/AuthenticationValidators.h"

#include OATPP_CODEGEN_BEGIN(ApiController)// <- Begin Codegen

struct r_cookieData {
    ulong puid;
    std::string sid;
};

r_cookieData getCookieAuthData(std::string cookie)
{
    auto pos = cookie.find(",");
    std::string puid = cookie.substr(0, pos);
    std::string sid = cookie.substr(pos + 1, 24);
    ulong i;
    std::stringstream ss(puid);
    ss >> i;
    return {i, sid};
}

class AuthenticationController : public oatpp::web::server::api::ApiController {
private:
    OATPP_COMPONENT(std::shared_ptr<Logger>, m_logger);
    OATPP_COMPONENT(std::shared_ptr<Database>, m_database);

public:
    AuthenticationController(const std::shared_ptr<ObjectMapper> &objectMapper)
        : oatpp::web::server::api::ApiController(objectMapper)
    {}
public:
    static std::shared_ptr<AuthenticationController> createShared(
        OATPP_COMPONENT(std::shared_ptr<ObjectMapper>, objectMapper) // Inject objectMapper component here as default parameter
    ) {
        return std::make_shared<AuthenticationController>(objectMapper);
    }

    ADD_CORS(helloWorld, ZWOO_CORS)
    ENDPOINT("GET", "/hello-world", helloWorld)
    {
        auto response = createResponse(Status::CODE_200, R"({"message": "Hello World!"})");
        m_logger->log->debug("/GET hello-world");
        return response;
    }
    ENDPOINT_INFO(helloWorld) {
        info->summary = "Hello World Testendpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(reCaptcha, ZWOO_CORS)
    ENDPOINT("POST", "auth/recaptcha", reCaptcha, BODY_STRING(String, token)) {
        return createResponse(Status::CODE_200, verifyCaptcha(token));
    }
    ENDPOINT_INFO(reCaptcha) {
        info->description = "verify the reCaptcha with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(create, ZWOO_CORS)
    ENDPOINT("POST", "auth/create", create, BODY_DTO(Object<CreateUserBodyDTO>, data)) {
        m_logger->log->debug("/POST create");
        if (!isValidEmail(data->email.getValue("")))
            return createResponse(Status::CODE_400, "Email Invalid!");
        if (!isValidUsername(data->username.getValue("")))
            return createResponse(Status::CODE_400, "Username Invalid!");
        if (!isValidPassword(data->password.getValue("")))
            return createResponse(Status::CODE_400, "Password Invalid!");
        if (m_database->entryExists("username", data->username.getValue("")))
            return createResponse(Status::CODE_400, "Username Already Exists!");
        if (m_database->entryExists("email", data->email.getValue("")))
            return createResponse(Status::CODE_400, "Email Already Exists!");

        r_CreateUser ret = m_database->createUser(data->username.getValue(""), data->email.getValue(""), data->password.getValue(""));

        try
        {
            // create mail message
            mailio::message msg;
            msg.from(mailio::mail_address("zwoo auth", SMTP_HOST_EMAIL));// set the correct sender name and address
            msg.add_recipient(mailio::mail_address("recipient", data->email));// set the correct recipent name and address
            msg.subject("Verify your ZWOO Account");
            msg.content(generateVerificationEmailText(ret.puid, ret.code, data->username));
            //msg.content("Hello World!");
            // connect to server
            mailio::smtps conn(SMTP_HOST_URL, SMTP_HOST_PORT);
            // modify username/password to use real credentials
            conn.authenticate(SMTP_USERNAME, SMTP_PASSWORD, mailio::smtps::auth_method_t::START_TLS);
            conn.submit(msg);
        }
        catch (mailio::smtp_error& exc)
        {
            m_logger->log->error("Email failed to send: {0}", exc.what());
        }
        catch (mailio::dialog_error& exc)
        {
            m_logger->log->error("Email failed to send: {0}", exc.what());
        }
        return createResponse(Status::CODE_200, "Account Created");
    }
    ENDPOINT_INFO(create) {
        info->description = "create a new User with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(verify, ZWOO_CORS)
    ENDPOINT("GET", "auth/verify", verify, QUERY(String, code, "code"), QUERY(UInt64, puid, "id")) {
        m_logger->log->debug("/GET verify");
        if (m_database->verifyUser(puid, code))
            return createResponse(Status::CODE_200, "Account Verified");
        else
            return createResponse(Status::CODE_400, "Account failed to verify");
    }
    ENDPOINT_INFO(verify) {
        info->description = "verify Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(login, ZWOO_CORS)
    ENDPOINT("POST", "auth/login", login, BODY_DTO(Object<LoginUserDTO>, data)) {
        m_logger->log->debug("/POST login");
        if (!isValidEmail(data->email.getValue("")))
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_400, "Email Invalid!"));

        auto login = m_database->loginUser(data->email, data->password);

        if (login.successful)
        {
            std::string out = encrypt(std::to_string(login.puid) + "," + login.sid);
            std::vector<uint8_t> vec(out.begin(), out.end());
            out = encodeBase64(vec);
            auto c = fmt::format("auth={0};Max-Age=604800;Domain={1};Path=/;HttpOnly{2}", out, ZWOO_DOMAIN, USE_SSL ? ";Secure" : "");
            auto res = createResponse(Status::CODE_200, "Logged In");
            res->putHeader("Set-Cookie", c);
            return setupResponseWithCookieHeaders(res);
        }
        else
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "failed to login"));
    }
    ENDPOINT_INFO(login) {
        info->description = "Login Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(user, ZWOO_CORS)
    ENDPOINT("GET", "auth/user", user, HEADER(String, ocookie, "Cookie")) {
        m_logger->log->debug("/GET User");

        std::string cookie = ocookie.getValue("");
        if (cookie.length() == 0)
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "Cookie Missing"));
        auto spos = cookie.find("auth=");
        if (spos < 0 || spos > cookie.length())
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "Cookie Missing"));

        auto usrc = getCookieAuthData(decrypt(decodeBase64(cookie.substr(spos + 5, cookie.find(';', spos) - spos - 5))));
        auto usr = m_database->getUser(usrc.puid);

        if (usr)
        {
            if (usr->sid.getValue("") == usrc.sid && usr->sid != "")
            {
                auto rusr = GetUserResponseDTO::createShared();
                rusr->username = usr->username;
                rusr->email = usr->email;
                rusr->wins = usr->wins;
                auto res = setupResponseWithCookieHeaders(createDtoResponse(Status::CODE_200, rusr));
                return res;
            }
            else
                return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "session id not matching!"));
        }
        else
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_404, "User Not Found!"));
    }
    ENDPOINT_INFO(user) {
        info->description = "Get User data with this Endpoint.";

        info->addResponse<Object<GetUserResponseDTO>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(logout, ZWOO_CORS)
    ENDPOINT("GET", "auth/logout", logout, HEADER(String, ocookie, "Cookie")) {
        m_logger->log->debug("/GET Logout");

        std::string cookie = ocookie.getValue("");
        if (cookie.length() == 0)
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "Cookie Missing"));
        auto spos = cookie.find("auth=");
        if (spos < 0 || spos > cookie.length())
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "Cookie Missing"));

        auto usrc = getCookieAuthData(decrypt(decodeBase64(cookie.substr(spos + 5, cookie.find(';', spos) - spos - 5))));
        auto usr = m_database->getUser(usrc.puid);

        if (usr)
        {
            if (usr->sid.getValue("") == usrc.sid&& usr->sid != "")
            {
                m_database->updateStringField("email", usr->email, "sid", "");
                auto res = createResponse(Status::CODE_200, "user logged out");
                auto c = fmt::format("auth=;Max-Age=0;Domain={0};Path=/;HttpOnly{1}", ZWOO_DOMAIN, USE_SSL ? ";Secure" : "");
                res->putHeader("Set-Cookie", c);
                return setupResponseWithCookieHeaders(res);
            }
            else
                return createResponse(Status::CODE_401, "session id not matching!");
        }
        else
            return createResponse(Status::CODE_404, "User Not Found!");
    }
    ENDPOINT_INFO(logout) {
        info->description = "Get User data with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(deleteUser, ZWOO_CORS)
    ENDPOINT("GET", "auth/delete", deleteUser, BODY_DTO(Object<DeleteUserDTO>, data), HEADER(String, ocookie, "Cookie")) {
        m_logger->log->debug("/GET delete");

        std::string cookie = ocookie.getValue("");
        if (cookie.length() == 0)
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "Cookie Missing"));
        auto spos = cookie.find("auth=");
        if (spos < 0 || spos > cookie.length())
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "Cookie Missing"));

        auto usrc = getCookieAuthData(decrypt(decodeBase64(cookie.substr(spos + 5, cookie.find(';', spos) - spos - 5))));

        if (m_database->deleteUser(usrc.puid, usrc.sid, data->password))
        {
            auto res = setupResponseWithCookieHeaders(createResponse(Status::CODE_200, "User Deleted!"));
            auto c = fmt::format("auth=;Max-Age=0;Domain={0};Path=/;HttpOnly{1}", ZWOO_DOMAIN, USE_SSL ? ";Secure" : "");
            res->putHeader("Set-Cookie", c);
            return res;
        }
        else
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_200, "Could not Deleted!"));
    }
    ENDPOINT_INFO(deleteUser) {
        info->description = "Login Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

private:
    std::shared_ptr<oatpp::web::protocol::http::outgoing::Response> setupResponseWithCookieHeaders(std::shared_ptr<oatpp::web::protocol::http::outgoing::Response> res)
    {
        res->putHeader("Access-Control-Allow-Origin", ZWOO_CORS);
        res->putHeader("Access-Control-Allow-Methods", "POST, GET");
        res->putHeader("Access-Control-Allow-Credentials", "true");

        return res;
    }
};

#include OATPP_CODEGEN_END(ApiController)// <- End Codegen

#endif
