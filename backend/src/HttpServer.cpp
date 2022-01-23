#include "HttpServer.h"

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

