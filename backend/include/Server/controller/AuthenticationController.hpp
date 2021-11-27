#ifndef _AUTHENTICATION_CONTROLLER_HPP_
#define _AUTHENTICATION_CONTROLLER_HPP_

#include <stdio.h>
#include <sstream>

#include <curl/curl.h>

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

    static size_t WriteCallback(void *contents, size_t size, size_t nmemb, void *userp)
    {
        ((std::string*)userp)->append((char*)contents, size * nmemb);
        return size * nmemb;
    }

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
        auto dto = reCaptchaDto::createShared();
        std::string readBuffer;
        // request to google servers
        {
            CURL *curl;
            CURLcode res;


            curl = curl_easy_init();
            if (curl)
            {
                curl_easy_setopt(curl, CURLOPT_URL, "https://www.google.com/recaptcha/api/siteverify");
                // TODO: hmmmm
                std::stringstream str;
                str << "secret=" << SITESECRET << "&response=" << token.getValue("token");
                curl_easy_setopt(curl, CURLOPT_POSTFIELDS, str.get());
                curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
                curl_easy_setopt(curl, CURLOPT_WRITEDATA, &readBuffer);
                res = curl_easy_perform(curl);
                if(res != CURLE_OK)
                    fprintf(stderr, "curl_easy_perform() failed: %s\n",
                            curl_easy_strerror(res));

                curl_easy_cleanup(curl);
            }
            curl_global_cleanup();
        }

        auto res = createResponse(Status::CODE_200, readBuffer);
        return res;
    }
    ENDPOINT_INFO(reCaptcha) {
        info->description = "verify the reCaptcha with this Endpoint.";

        info->addResponse<Object<reCaptchaDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }
};

#include OATPP_CODEGEN_END(ApiController) // <- End Codegen

#endif // _AUTHENTICATION_CONTROLLER_HPP_