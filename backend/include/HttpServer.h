#ifndef _HTTP_SERVER_H_
#define _HTTP_SERVER_H_

// #include "SimpleJSON/json.hpp"
#include <iostream>

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

        Backend::Game::GameManager gamemanager;


    public:
        HttpServer();

        void InitEndpoints();

        void StartServer()
        {
            std::cout << "Starting Server on Port " << kPort << " ...\n";
        }
    };
} // namespace Backend

#endif