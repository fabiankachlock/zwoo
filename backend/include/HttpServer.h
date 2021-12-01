#ifndef _HTTP_SERVER_H_
#define _HTTP_SERVER_H_

#include "oatpp/network/Server.hpp"

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

        void RunServer()
        {
            oatpp::base::Environment::init();

            ServerComponent components;

            /* Get router component */
            OATPP_COMPONENT(std::shared_ptr<oatpp::web::server::HttpRouter>, router);

#ifdef BUILD_SWAGGER
            oatpp::web::server::api::Endpoints docEndpoints;

            // All API Controllers here
            docEndpoints.append(router->addController(AuthenticationController::createShared())->getEndpoints());

            router->addController(oatpp::swagger::Controller::createShared(docEndpoints));
#endif // BUILD_SWAGGER

            router->addController(AuthenticationController::createShared());

            /* Get connection handler component */
            OATPP_COMPONENT(std::shared_ptr<oatpp::network::ConnectionHandler>, connectionHandler);

            /* Get connection provider component */
            OATPP_COMPONENT(std::shared_ptr<oatpp::network::ServerConnectionProvider>, connectionProvider);

            /* create server */
            oatpp::network::Server server(connectionProvider,
                                          connectionHandler);

            OATPP_LOGD("Server", "Running on port %s...", connectionProvider->getProperty("port").toString()->c_str());

            server.run();

            oatpp::base::Environment::destroy();
        }
    };
} // namespace Backend

#endif