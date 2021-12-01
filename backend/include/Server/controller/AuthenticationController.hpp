#ifndef _AUTHENTICATION_CONTROLLER_HPP_
#define _AUTHENTICATION_CONTROLLER_HPP_

#include "Authentication/reCpatcha.h"

#include "oatpp/web/server/api/ApiController.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/core/macro/component.hpp"

#include "Server/dto/AuthenticationDTO.hpp"

#include OATPP_CODEGEN_BEGIN(ApiController) // <- Begin Codegen

class AuthenticationController : public oatpp::web::server::api::ApiController {
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
};

#include OATPP_CODEGEN_END(ApiController) // <- End Codegen

#endif // _AUTHENTICATION_CONTROLLER_HPP_