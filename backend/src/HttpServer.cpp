#include "HttpServer.h"

#include "oatpp/network/Server.hpp"

#include "oatpp-openssl/server/ConnectionProvider.hpp"
#include "oatpp-openssl/Config.hpp"

#include "Server/ServerComponent.hpp"
#include "Server/controller/Authentication/AuthenticationController.hpp"
#include "Server/controller/GameManager/GameManagerController.hpp"

#include "utils/thread.h"

#ifdef BUILD_SWAGGER
#include "oatpp-swagger/Controller.hpp"
#endif // BUILD_SWAGGER

void databaseCleanup()
{
    auto m_objectMapper = oatpp::mongo::bson::mapping::ObjectMapper::createShared();
    // Connect to DB
    mongocxx::pool = mongocxx::pool(mongocxx::uri(ZWOO_DATABASE_CONNECTION_STRING));
    auto conn = m_pool.acquire();
    auto collection = ( *conn ) ["zwoo"]["users"];
    // Delete unverified users
    oatpp::data::stream::BufferOutputStream stream;
    m_objectMapper.write ( &stream, oatpp::Fields<oatpp::Bool>({{"verified", false}}) );
    bsoncxx::document::view view ( stream.getData(), stream.getCurrentPosition() );
    
    collection.delete(bsoncxx::document::value ( view ));
}

HttpServer::HttpServer()
{
    logger = std::make_shared<Logger>();
    logger->init("BED");
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
    docEndpoints.append(router->addController(GameManagerController::createShared())->getEndpoints());
    router->addController(oatpp::swagger::Controller::createShared(docEndpoints));

    logger->log->info("Added Swagger Endpoint: http://localhost:8000/swagger/ui");
#endif

    router->addController(AuthenticationController::createShared());
    router->addController(GameManagerController::createShared());

    /* Get connection handler component */
    OATPP_COMPONENT(std::shared_ptr<oatpp::network::ConnectionHandler>, connectionHandler, "http");

    if (USE_SSL)
    {
        auto conf = oatpp::openssl::Config::createDefaultServerConfigShared(SSL_PEM, SSL_CERTIFICATE);
        auto connectionProvider = oatpp::openssl::server::ConnectionProvider::createShared(conf, {ZWOO_BACKEND_DOMAIN, ZWOO_BACKEND_PORT, oatpp::network::Address::IP_4});

        /* create server */
        oatpp::network::Server server(connectionProvider, connectionHandler);


        logger->log->info("Running on port {0}...", connectionProvider->getProperty("port").toString()->c_str());

        timedFunction(databaseCleanup, 0, 1);

        server.run();
    }
    else
    {
        auto connectionProvider = oatpp::network::tcp::server::ConnectionProvider::createShared({ZWOO_BACKEND_DOMAIN, ZWOO_BACKEND_PORT, oatpp::network::Address::IP_4});

        /* create server */
        oatpp::network::Server server(connectionProvider, connectionHandler);

        logger->log->info("Running on port {0}...", connectionProvider->getProperty("port").toString()->c_str());

        timedFunction(databaseCleanup, 0, 1);

        server.run();
    }

    oatpp::base::Environment::destroy();
}

