#include "HttpServer.h"

#include "oatpp/network/Server.hpp"

#include "Server/ServerComponent.hpp"
#include "Server/controller/Authentication/AuthenticationController.hpp"

#ifdef BUILD_SWAGGER
#include "oatpp-swagger/Controller.hpp"
#endif // BUILD_SWAGGER

HttpServer::HttpServer()
{
    logger = std::make_shared<Logger>();
    logger->init();
}

void HttpServer::RunServer()
{
    oatpp::base::Environment::init();

    auto server_component = ServerComponent(logger);

    /* Get router component */
    OATPP_COMPONENT(std::shared_ptr<oatpp::web::server::HttpRouter>, router);

#ifdef BUILD_SWAGGER
    oatpp::web::server::api::Endpoints docEndpoints;

    docEndpoints.append(router->addController(AuthenticationController::createShared())->getEndpoints());
    router->addController(oatpp::swagger::Controller::createShared(docEndpoints));

    logger->log->info("Added Swagger Endpoint: http://localhost:8000/swagger/ui");
#endif

    router->addController(AuthenticationController::createShared());

    /* Get connection handler component */
    OATPP_COMPONENT(std::shared_ptr<oatpp::network::ConnectionHandler>, connectionHandler);

    /* Get connection provider component */
    OATPP_COMPONENT(std::shared_ptr<oatpp::network::ServerConnectionProvider>, connectionProvider);

    /* create server */
    oatpp::network::Server server(connectionProvider,
                                  connectionHandler);

    logger->log->info("Running on port {0}...", connectionProvider->getProperty("port").toString()->c_str());

    server.run();

    oatpp::base::Environment::destroy();
}

