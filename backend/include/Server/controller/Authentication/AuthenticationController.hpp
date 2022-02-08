#ifndef _AUTHENTICATION_CONTROLLER_HPP_
#define _AUTHENTICATION_CONTROLLER_HPP_

#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/macro/component.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/web/server/api/ApiController.hpp"

#include "mailio/message.hpp"
#include "mailio/mime.hpp"
#include "mailio/smtp.hpp"

#include "Server/logger/logger.h"
#include "Server/DatabaseComponent.hpp"
#include "Server/controller/Authentication/ReCaptcha.h"
#include "Server/dto/AuthenticationDTO.hpp"
#include "Server/controller/Authentication/AuthenticationValidators.h"

#include OATPP_CODEGEN_BEGIN(ApiController)// <- Begin Codegen

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
        auto response = createResponse(Status::CODE_200, R"({"message": "Hello Wolrd!"})");
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
        if (m_database->entrieExists("username", data->username.getValue("")))
            return createResponse(Status::CODE_400, "Username Already Exists!");
        if (m_database->entrieExists("email", data->email.getValue("")))
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
        info->description = "verifie Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(login, ZWOO_CORS)
    ENDPOINT("POST", "auth/login", login, BODY_DTO(Object<LoginUserDTO>, data)) {
        m_logger->log->debug("/POST login");
        if (!isValidEmail(data->email.getValue("")))
            return createResponse(Status::CODE_400, "Email Invalid!");
        if (!isValidPassword(data->password.getValue("")))
            return createResponse(Status::CODE_400, "Password Invalid!");

        auto login = m_database->loginUser(data->email, data->password);

        if (login.successful)
        {
            std::string out = "{\"puid\": " + std::to_string(login.puid) + ", \"sid\": \"" + login.sid + "\"}";
            return createResponse(Status::CODE_200, out);
        }
        else
            return createResponse(Status::CODE_401, "failed to login");

        return createResponse(Status::CODE_501, "Not Implemented!");
    }
    ENDPOINT_INFO(login) {
        info->description = "Login Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(user, ZWOO_CORS)
    ENDPOINT("GET", "auth/user", user, BODY_DTO(Object<GetUserDTO>, data)) {
        m_logger->log->debug("/GET User");

        auto usr = m_database->getUser(data->puid);

        if (usr)
        {
            if (usr->sid == data->sid && usr->sid != "")
            {
                auto rusr = GetUserResponseDTO::createShared();
                rusr->username = usr->username;
                rusr->email = usr->email;
                rusr->wins = usr->wins;
                return createDtoResponse(Status::CODE_200, rusr);
            }
            else
                return createResponse(Status::CODE_401, "session id not matching!");
        }
        else
            return createResponse(Status::CODE_404, "User Not Found!");
        return createResponse(Status::CODE_501, "Not Implemented!");
    }
    ENDPOINT_INFO(user) {
        info->description = "Get User data with this Endpoint.";

        info->addResponse<Object<GetUserResponseDTO>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(logout, ZWOO_CORS)
    ENDPOINT("GET", "auth/logout", logout, BODY_DTO(Object<LogoutUserDTO>, data)) {
        m_logger->log->debug("/GET Logout");

        auto usr = m_database->getUser(data->puid);

        if (usr)
        {
            if (usr->sid == data->sid && usr->sid != "")
            {
                m_database->updateStringField("email", usr->email, "sid", "");
                return createResponse(Status::CODE_200, "user logged out");
            }
            else
                return createResponse(Status::CODE_401, "session id not matching!");
        }
        else
            return createResponse(Status::CODE_404, "User Not Found!");

        return createResponse(Status::CODE_501, "Not Implemented!");
    }
    ENDPOINT_INFO(logout) {
        info->description = "Get User data with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(delete, ZWOO_CORS)
    ENDPOINT("GET", "auth/delete", delte, BODY_DTO(Object<DeleteUserDTO>, data)) {
        m_logger->log->debug("/GET delete");
        if (m_database->deleteUser(data->puid, data->sid, data->password))
            return createResponse(Status::CODE_200, "User Deleted!");
        else
            return createResponse(Status::CODE_200, "Could not Deleted!");
        return createResponse(Status::CODE_501, "Not Implemented!");
    }
    ENDPOINT_INFO(delte) {
        info->description = "Login Users with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }


};

#include OATPP_CODEGEN_END(ApiController)// <- End Codegen

#endif
