#ifndef _HTTP_SERVER_H_
#define _HTTP_SERVER_H_

#include "oatpp/network/Server.hpp"

#include <mongocxx/instance.hpp>

#include "Server/ServerComponent.hpp"
#include "Server/controller/AuthenticationController.hpp"

#include "Game/GameManager.h"

#ifdef BUILD_SWAGGER
#include "oatpp-swagger/Controller.hpp"
#endif // BUILD_SWAGGER

namespace Backend
{
    class HttpServer
    {
    private:

        Backend::Game::GameManager gamemanager;

    public:
        HttpServer() 
        {
            gamemanager = Game::GameManager();
        }

        void RunServer(const oatpp::base::CommandLineArguments& args);
    };
} // namespace Backend

#endif