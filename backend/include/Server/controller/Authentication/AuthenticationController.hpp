#ifndef _AUTHENTICATION_CONTROLLER_HPP_
#define _AUTHENTICATION_CONTROLLER_HPP_

#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/macro/component.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/web/server/api/ApiController.hpp"

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

    ENDPOINT("POST", "auth/recaptcha", reCaptcha, BODY_STRING(String, token)) {
        return createResponse(Status::CODE_200, verifyCaptcha(token));
    }
    ENDPOINT_INFO(reCaptcha) {
        info->description = "verify the reCaptcha with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ENDPOINT("POST", "auth/create", create, BODY_DTO(Object<CreateUserBodyDTO>, data)) {
        m_logger->log->debug("/POST create");
        if (m_database->entrieExists("username", data->username.getValue("")))
            return createResponse(Status::CODE_400, "Username Already Exists!");
        if (m_database->entrieExists("email", data->email.getValue("")))
            return createResponse(Status::CODE_400, "Email Already Exists!");
        if (!isValidEmail(data->email.getValue("")))
            return createResponse(Status::CODE_400, "Email Invalid!");
        if (!isValidUsername(data->username.getValue("")))
            return createResponse(Status::CODE_400, "Username Invalid!");
        if (!isValidPassword(data->password.getValue("")))
            return createResponse(Status::CODE_400, "Password Invalid!");

        r_CreateUser ret = m_database->createUser(data->username.getValue(""), data->email.getValue(""), data->password.getValue(""));

        return createResponse(Status::CODE_200, "Account Created");
    }

    ENDPOINT("GET", "auth/verify", verify, QUERY(String, code, "code"), QUERY(UInt64, puid, "id")) {
        m_logger->log->debug("/GET verify");
        if (m_database->verifyUser(puid, code))
            return createResponse(Status::CODE_200, "Account Verified");
        else
            return createResponse(Status::CODE_400, "Account failed to verify");
    }
};

#include OATPP_CODEGEN_END(ApiController)// <- End Codegen

#endif
