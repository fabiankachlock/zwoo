#ifndef _GAMEMANAGER_CONTROLLER_HPP_
#define _GAMEMANAGER_CONTROLLER_HPP_

#include "oatpp-websocket/Handshaker.hpp"
#include <boost/beast/core/detail/base64.hpp>
#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/macro/component.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/web/server/api/ApiController.hpp"

#include "Server/controller/Authentication/AuthenticationController.hpp"
#include "Server/dto/GameManagerDTO.hpp"

#include <string>

#include OATPP_CODEGEN_BEGIN(ApiController) // <- Begin Codegen

uint32_t createGame()
{
    return 0;
}

class GameManagerController : public oatpp::web::server::api::ApiController {
private:
    OATPP_COMPONENT(std::shared_ptr<Logger>, m_logger_backend, "Backend");
    OATPP_COMPONENT(std::shared_ptr<Logger>, m_logger_websocket, "Websocket");
    OATPP_COMPONENT(std::shared_ptr<Database>, m_database);
    OATPP_COMPONENT(std::shared_ptr<oatpp::network::ConnectionHandler>, websocketConnectionHandler, "websocket");

public:
    GameManagerController(const std::shared_ptr<ObjectMapper> &objectMapper)
        : oatpp::web::server::api::ApiController(objectMapper)
    {}

public:
    static std::shared_ptr<GameManagerController> createShared(
        OATPP_COMPONENT(std::shared_ptr<ObjectMapper>, objectMapper) // Inject objectMapper component here as default parameter
    )
    {
        return std::make_shared<GameManagerController>(objectMapper);
    }

    ADD_CORS(join, ZWOO_CORS)
    ENDPOINT("GET", "game/join", join, HEADER(String, ocookie, "Cookie"), REQUEST(std::shared_ptr<IncomingRequest>, request))
    {
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
                uint32_t guid = createGame(); // Create Game | TODO: use GameManager when finished (with player data)

                auto res = oatpp::websocket::Handshaker::serversideHandshake(request->getHeaders(), websocketConnectionHandler);
                auto parameters = std::make_shared<oatpp::network::ConnectionHandler::ParameterMap>();
                (*parameters)["puid"] = std::to_string(usr->_id);
                (*parameters)["username"] = usr->username;
                (*parameters)["guid"] = std::to_string(guid);
                (*parameters)["role"] = "0";
                res->setConnectionUpgradeParameters(parameters);
                return res;
            }
            else
                return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, "session id not matching!"));
        }
        else
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_404, "User Not Found!"));

    }
    ENDPOINT_INFO(join) {
        info->summary = "Hello World Testendpoint.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }
};

#include OATPP_CODEGEN_END(ApiController)// <- End Codegen

#endif
