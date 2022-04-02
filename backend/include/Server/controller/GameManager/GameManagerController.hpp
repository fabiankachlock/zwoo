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
#include "Server/controller/error.h"

#include <string>
#include <unordered_map>
#include <vector>

#include OATPP_CODEGEN_BEGIN(ApiController) // <- Begin Codegen

struct s_Game {
    //                   puid  ,  role
    std::unordered_map<uint32_t, uint8_t> player; // Only Players who can join not all players

    std::string password;
    bool is_private;
};

uint32_t createGame()
{
    return 1;
}

class GameManagerController : public oatpp::web::server::api::ApiController {
private:
    OATPP_COMPONENT(std::shared_ptr<Logger>, m_logger_backend, "Backend");
    OATPP_COMPONENT(std::shared_ptr<Logger>, m_logger_websocket, "Websocket");
    OATPP_COMPONENT(std::shared_ptr<Database>, m_database);
    OATPP_COMPONENT(std::shared_ptr<oatpp::network::ConnectionHandler>, websocketConnectionHandler, "websocket");

    std::unordered_map<uint32_t, s_Game> games;

    void printGames()
    {
        for (const auto&[k2, v2] : games)
        {
            m_logger_backend->log->info("{0}:", k2);
            for (const auto&[k1, v1] : v2.player)
                m_logger_backend->log->info("  {0}: {1},", k1, v1);
        }
    }

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

    ADD_CORS(join_game, ZWOO_CORS)
    ENDPOINT("POST", "game/join", join_game, HEADER(String, ocookie, "Cookie"), BODY_DTO(Object<JoinGameDTO>, data))
    {
        if (data->opcode == 0)
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Opcode Missing", e_Errors::OPCODE_MISSING)));
        else if (data->opcode == 1)
        {
            if (data->name.getValue("") == "")
                return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Game Name Missing", e_Errors::GAME_NAME_MISSING)));
        }
        else if (data->opcode == 2 || data->opcode == 3)
        {
            if (data->guid == 0)
                return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Gameid 0 is not valid!", e_Errors::INVALID_GAMEID)));
        }
        else
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Invalid Opcode", e_Errors::INVALID_OPCODE)));

        std::string cookie = ocookie.getValue("");
        if (cookie.length() == 0)
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Cookie Missing", e_Errors::COOKIE_MISSING)));
        auto spos = cookie.find("auth=");
        if (spos < 0 || spos > cookie.length())
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Cookie Missing", e_Errors::COOKIE_MISSING)));

        auto usrc = getCookieAuthData(decrypt(decodeBase64(cookie.substr(spos + 5, cookie.find(';', spos) - spos - 5))));
        auto usr = m_database->getUser(usrc.puid);

        if (usr)
        {
            if (usr->sid.getValue("") == usrc.sid && usr->sid != "")
            {
                uint32_t guid = 0;

                if (data->opcode == 1)
                {
                    guid = createGame(); // Create Game | TODO: use GameManager when finished (with player data)
                    s_Game g = { { { usr->_id, 1 } }, data->password.getValue(""), data->use_password };
                    games.insert({ guid, g });
                    printGames();
                }
                else
                {
                    guid = data->guid;
                    auto game = games.find(guid);
                    if (game != games.end())
                    {
                        if (game->second.is_private)
                            if (game->second.password != data->password.getValue(""))
                                return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Password not matching!", e_Errors::PASSWORD_NOT_MATCHING)));

                        auto p = game->second.player.find(usr->_id);
                        if (p == game->second.player.end())
                            game->second.player.insert({ usr->_id, data->opcode });
                        else
                            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Already in this Game!",e_Errors::ALREADY_INGAME)));
                        printGames();
                    }
                }

                return setupResponseWithCookieHeaders(createResponse(Status::CODE_200, "{\"guid\":\"" + std::to_string(guid) + "\"}"));
            }
            else
                return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("session id not matching!", e_Errors::SESSION_ID_NOT_MATCHING)));
        }
        else
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_404, constructErrorMessage("User Not Found!", e_Errors::USER_NOT_FOUND)));
    }
    ENDPOINT_INFO(join_game) {
        info->summary = "An Endpoint to join a game.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(join, ZWOO_CORS)
    ENDPOINT("GET", "game/join/{guid}", join, HEADER(String, ocookie, "Cookie"), REQUEST(std::shared_ptr<IncomingRequest>, request), PATH(UInt32, guid, "guid"))
    {
        printGames();
        std::string cookie = ocookie.getValue("");
        if (cookie.length() == 0)
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Cookie Missing", e_Errors::COOKIE_MISSING)));
        auto spos = cookie.find("auth=");
        if (spos < 0 || spos > cookie.length())
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Cookie Missing", e_Errors::COOKIE_MISSING)));

        auto usrc = getCookieAuthData(decrypt(decodeBase64(cookie.substr(spos + 5, cookie.find(';', spos) - spos - 5))));

        uint8_t role = 0;
        auto game = games.find(guid);
        if (game != games.end())
        {
            auto p = game->second.player.find(usrc.puid);
            if (p != game->second.player.end())
            {
                role = p->second;
                game->second.player.erase(p);
            }
            else
                 return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("Can't join game!", e_Errors::JOIN_FAILED)));
        }
        else
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_404, constructErrorMessage("Game Not Found!", e_Errors::GAME_NOT_FOUND)));

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
                (*parameters)["role"] = std::to_string(role);
                (*parameters)["wins"] = std::to_string((int32_t)usr->wins);
                res->setConnectionUpgradeParameters(parameters);
                return res;
            }
            else
                return setupResponseWithCookieHeaders(createResponse(Status::CODE_401, constructErrorMessage("session id not matching!", e_Errors::SESSION_ID_NOT_MATCHING)));
        }
        else
            return setupResponseWithCookieHeaders(createResponse(Status::CODE_404, constructErrorMessage("User Not Found!", e_Errors::USER_NOT_FOUND)));

    }
    ENDPOINT_INFO(join) {
        info->summary = "An Endpoint to establish a Websocket connection.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

    ADD_CORS(leaderboard, ZWOO_CORS)
    ENDPOINT("GET", "game/leaderboard", leaderboard)
    {
        return setupResponseWithCookieHeaders(createDtoResponse(Status::CODE_200, m_database->getLeaderBoard()));
    }
    ENDPOINT_INFO(leaderboard) {
        info->summary = "An Endpoint to get the top 100.";

        info->addResponse<Object<StatusDto>>(Status::CODE_200, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_404, "application/json");
        info->addResponse<Object<StatusDto>>(Status::CODE_500, "application/json");
    }

};

#include OATPP_CODEGEN_END(ApiController)// <- End Codegen

#endif
