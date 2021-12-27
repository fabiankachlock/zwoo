#ifndef _AUTHENTICATION_CONTROLLER_HPP_
#define _AUTHENTICATION_CONTROLLER_HPP_

#include <regex>

#include "Authentication/SMTPClient.h"
#include "Authentication/reCpatcha.h"

#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/macro/component.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/web/server/api/ApiController.hpp"

#include "Server/Database/Database.h"
#include "Server/dto/AuthenticationDTO.hpp"
#include "utils/RandomString.h"
#include "utils/SHA512.h"
#include "utils/Validator.h"

#include OATPP_CODEGEN_BEGIN(ApiController)// <- Begin Codegen

class AuthenticationController : public oatpp::web::server::api::ApiController {
private:
    OATPP_COMPONENT(std::shared_ptr<Backend::Database>, m_database);

    std::regex email_regex;
    SHA512 sha512 = SHA512();

public:
    AuthenticationController(const std::shared_ptr<ObjectMapper> &objectMapper)
        : oatpp::web::server::api::ApiController(objectMapper)
        {

        }

public:
    static std::shared_ptr<AuthenticationController> createShared(
            OATPP_COMPONENT(std::shared_ptr<ObjectMapper>, objectMapper)// Inject objectMapper component here as default parameter
    ) {
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

    ENDPOINT("POST", "auth/create", create, BODY_DTO(Object<CreateUserBodyDTO>, create_user_dto)) {

        if (create_user_dto->email != "" && create_user_dto->password != "" && create_user_dto->username != "") {

            // Check for data
            if (!isValidEmail(create_user_dto->email.getValue("").c_str()))
                return createResponse(Status::CODE_400, "invalide Email");
            if (!isValidPassword(create_user_dto->password.getValue("").c_str()))
                return createResponse(Status::CODE_400, "invalide Password");

            if (m_database->entrieExists("username", create_user_dto->username.getValue("")))
                return createResponse(Status::CODE_400, "Username Already Exists");
            if (m_database->entrieExists("email", create_user_dto->email.getValue("")))
                return createResponse(Status::CODE_400, "Username Already Exists");

            // Hashing Password
            std::string salt = Backend::randomString(16);
            std::string hash = sha512.hash(salt + create_user_dto->password.getValue(""));
            std::string hash_str = "sha512:" + salt + ":" + hash;

            std::string code = "000000";

            for (int i = 0; i < 10; ++i)
            {
                code = Backend::randomNDigitNumber(6);
                if (!m_database->entrieExists("validation_code", code))
                    break;
            }

            bool status = m_database->createUser(create_user_dto->username.getValue(""), create_user_dto->email.getValue(""), hash_str, code);
            if (status) {
                std::string d = DOMAIN;
                std::string str = d + "/auth/verify?code=" + code;
                SendVerificationEmail(create_user_dto->email.getValue("").c_str(), str.c_str());
                return createResponse(Status::CODE_200, "User created Email Send");
            } else
                return createResponse(Status::CODE_500, "Unkown Error");
        } else {
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

    ENDPOINT("GET", "auth/verify", verify, QUERY(String, code, "code")) {

        auto user = m_database->getUser("validation_code", code);

        if (user) {

            bool status1 = m_database->updateUserField("email", user->email.getValue(""), "verified", true);
            bool status2 = m_database->updateUserField("email", user->email.getValue(""), "validation_code", "");
            return createResponse(Status::CODE_200, "User verified");
        }
        else
            return createResponse(Status::CODE_400, "invalide code");
        
        return createResponse(Status::CODE_501, "Not Implemented");
    }
    ENDPOINT_INFO(verify) {
        info->description = "Endpoint for verifying the user.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ENDPOINT("GET", "auth/is-verified", verified, QUERY(String, email, "email")) {

        if (!isValidEmail(email.getValue("").c_str()))
            return createResponse(Status::CODE_400, "invalide Email");

        auto user = m_database->getUser(email.getValue(""));
        if (user)
        {
            std::string out = "{\"verified\": ";
            out += user->verified ? "true" : "false";
            out += '}';
            return createResponse(Status::CODE_200, out);
        }
        else
            return createResponse(Status::CODE_400, "No user with this email");
        return createResponse(Status::CODE_501, "Not Implemented");
    }
    ENDPOINT_INFO(verified) {
        info->description = "Endpoint for checking if user is verified.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ENDPOINT("GET", "auth/user", getUser, QUERY(String, email, "email")) {
        if (!isValidEmail(email.getValue("").c_str()))
            return createResponse(Status::CODE_400, "invalide Email");

        auto user = m_database->getUser(email.getValue(""));
        if (user)
        {
            auto usr = GetUserResponseDTO::createShared();
            
            usr->username = user->username;
            usr->email = user->email;
            usr->wins = user->wins;

            return createDtoResponse(Status::CODE_200, usr);
        }
        else
            return createResponse(Status::CODE_400, "No user with this email");
        return createResponse(Status::CODE_501, "Not Implemented");
    }
    ENDPOINT_INFO(getUser) {
        info->description = "Endpoint for getting player data.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    void SendVerificationEmail(const char *email_address, const char *link) {
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

#include OATPP_CODEGEN_END(ApiController)// <- End Codegen

#endif// _AUTHENTICATION_CONTROLLER_HPP_