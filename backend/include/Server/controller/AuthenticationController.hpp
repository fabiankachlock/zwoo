#ifndef _AUTHENTICATION_CONTROLLER_HPP_
#define _AUTHENTICATION_CONTROLLER_HPP_

#include "Authentication/reCpatcha.h"
#include "Authentication/SMTPClient.h"

#include "oatpp/web/server/api/ApiController.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/core/macro/component.hpp"

#include "Server/dto/AuthenticationDTO.hpp"
#include "Server/Database/Database.h"
#include "utils/RandomString.h"
#include "utils/SHA512.h"

#include OATPP_CODEGEN_BEGIN(ApiController) // <- Begin Codegen

class AuthenticationController : public oatpp::web::server::api::ApiController {
private:
    OATPP_COMPONENT(std::shared_ptr<Backend::Database>, m_database);

    SHA512 sha512 = SHA512();

public:

    AuthenticationController(const std::shared_ptr<ObjectMapper>& objectMapper)
      : oatpp::web::server::api::ApiController(objectMapper)
    {}

public:

    static std::shared_ptr<AuthenticationController> createShared(
            OATPP_COMPONENT(std::shared_ptr<ObjectMapper>, objectMapper) // Inject objectMapper component here as default parameter
    ){
        return std::make_shared<AuthenticationController>(objectMapper);
    }

    ENDPOINT("GET", "/hello-world", helloWorld) {
        auto response = createResponse(Status::CODE_200, R"({"message": "Hello Wolrd!"})");
        return response;
    }
    ENDPOINT_INFO(helloWorld) {
        info->summary = "Hello World Testendpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ENDPOINT("POST", "auth/recaptcha", reCaptcha, BODY_STRING(String, token)) {
        std::string readBuffer = Backend::Authentication::VerifiyCaptcha(token.get()->c_str());
        auto res = createResponse(Status::CODE_200, readBuffer);
        return res;
    }
    ENDPOINT_INFO(reCaptcha) {
        info->description = "verify the reCaptcha with this Endpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ENDPOINT("POST", "auth/create", create, BODY_DTO(Object<CreateUserBodyDTO>, create_user_dto))
    {

        if (create_user_dto->email != "" && create_user_dto->password != "" && create_user_dto->username != "")
        {
            // TODO: validate values (regex)
            // TODO: Check if Email or Username exists already

            std::string salt = Backend::randomString(16);
            std::string hash = sha512.hash(salt + create_user_dto->password.getValue(""));
            std::string hash_str = "sha512:" + salt + ":" + hash;

            bool status = m_database->createUser(create_user_dto->username.getValue(""), create_user_dto->email.getValue(""), hash_str);
            if (status)
            {
                SendVerificationEmail(create_user_dto->email.getValue("").c_str(), "https://zwoo/auth/verify?code=000000");
                return createResponse(Status::CODE_200, "User created Email Send");
            }
            else
                return createResponse(Status::CODE_500, "Unkown Error");
        }
        else
        {
            return createResponse(Status::CODE_400, "No Email, password or username"); 
        }
        return createResponse(Status::CODE_501, "Not Implemented");
    }
    ENDPOINT_INFO(create) {
        info->description = "Endpoint for creating Users.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_400, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    void SendVerificationEmail(const char *email_address, const char *link)
    {
        auto client = Backend::Authentication::SMTPClient();
        client.m_password = ZWOO_PASSWORD;
        client.m_username = ZWOO_EMAIL;

        auto email = Backend::Authentication::Email();

        email.from = ZWOO_USERNAME;
        email.to = email_address;

        email.subject = "Authenticate your zwoo Account";
        email.header = "Your verification code!";

        email.AddLine("Klick the link below to verify your zwoo account");
        email.AddLine(link);

        client.SendEmail(email);
    }
};

#include OATPP_CODEGEN_END(ApiController) // <- End Codegen

#endif // _AUTHENTICATION_CONTROLLER_HPP_