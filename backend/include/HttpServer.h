#ifndef _HTTP_SERVER_H_
#define _HTTP_SERVER_H_

#include "served/multiplexer.hpp"
#include "served/uri.hpp"
#include "served/net/server.hpp"

// #include "SimpleJSON/json.hpp"

#include "Game/GameManager.h"

namespace Backend
{
    // Authentication Callbacks
    constexpr char *kCreateAccount = "auth/create";
    constexpr char *kVerifyAccount = "auth/verify";
    constexpr char *kLoginAccount = "auth/login";
    // GameId && PlayerId (Player who wants to delete Game)
    constexpr char *kDeleteGame = "gamemanager/delete";
    // GameId && PlayerId
    constexpr char *kPlayerJoin = "gamemanager/join";
    // GameId && PlayerId
    constexpr char *kPlayerLeave = "gamemanager/leave";

    constexpr char *kChangeGameSettings = "game/settings";

    constexpr char kIpAddress[] = "0.0.0.0";
    constexpr char kPort[] = "5000";
    constexpr char kThreads = 10;

    class HttpServer
    {
    private:
        served::multiplexer multiplexer;

        Backend::Game::GameManager gamemanager;


    public:
        HttpServer(served::multiplexer m);

        void InitEndpoints();

        void StartServer()
        {
            served::net::server server("0.0.0.0", "5000", multiplexer);
            std::cout << "Starting Server on Port " << kPort << " ...\n";
            server.run(10);
        }

        auto HelloWorld()
        {
            return [&](served::response &response, const served::request &request)
            {
                response << "{ \"message\": \"Hello World!\" }";
                // return if insert was successful or not
                return served::response::stock_reply(200, response);
            };
        }

        auto CreateGame()
        {
            return [&](served::response &response, const served::request &request)
            {
                if (request.header("player_id") != "" && request.header("game_name") != "")
                {
                    auto game = Game::Game();
                    game.game_name = request.header("game_name").c_str();

                    response << "{ \"game_id\": \"" << std::to_string(gamemanager.AddGame(&game)) << "\" }";

                    std::cout << game.game_name << " created!" << std::endl;

                    return served::response::stock_reply(200, response);
                }
                else
                {
                    return served::response::stock_reply(400, response);
                }
            };
        }

    };
} // namespace Backend

#endif