#include "HttpServer.h"

namespace Backend
{
    void HttpServer::RunServer(const oatpp::base::CommandLineArguments& args)
        {
            mongocxx::instance instance{};

            oatpp::base::Environment::init();

            ServerComponent components(args);

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
}
