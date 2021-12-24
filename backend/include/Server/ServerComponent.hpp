#ifndef _SERVER_COMPONENT_HPP_
#define _SERVER_COMPONENT_HPP_

#include "zwoo.h"

#include "ErrorHandler.hpp"

#include "oatpp/web/server/HttpConnectionHandler.hpp"
#include "oatpp/web/server/HttpRouter.hpp"
#include "oatpp/network/tcp/server/ConnectionProvider.hpp"

#include "oatpp/parser/json/mapping/ObjectMapper.hpp"

#include "oatpp/core/macro/component.hpp"

#ifdef BUILD_SWAGGER
#include "SwaggerComponent.hpp"
#endif

#include "Database/Database.h"

class ServerComponent {
public:
#ifdef BUILD_SWAGGER
    SwaggerComponent swaggerComponent;
#endif
    /**
   * Create ObjectMapper component to serialize/deserialize DTOs in Contoller's API
   */
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::data::mapping::ObjectMapper>, apiObjectMapper)([] {
        auto objectMapper = oatpp::parser::json::mapping::ObjectMapper::createShared();
        objectMapper->getDeserializer()->getConfig()->allowUnknownFields = false;
        return objectMapper;
    }());

    /**
   *  Create ConnectionProvider component which listens on the port
   */
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::network::ServerConnectionProvider>, serverConnectionProvider)([] {
        return oatpp::network::tcp::server::ConnectionProvider::createShared({DOMAIN, PORT, oatpp::network::Address::IP_4});
    }());

    /**
   *  Create Router component
   */
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::web::server::HttpRouter>, httpRouter)([] {
        return oatpp::web::server::HttpRouter::createShared();
    }());

    /**
   *  Create ConnectionHandler component which uses Router component to route requests
   */
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::network::ConnectionHandler>, serverConnectionHandler)([] {

        OATPP_COMPONENT(std::shared_ptr<oatpp::web::server::HttpRouter>, router); // get Router component
        OATPP_COMPONENT(std::shared_ptr<oatpp::data::mapping::ObjectMapper>, objectMapper); // get ObjectMapper component

        auto connectionHandler = oatpp::web::server::HttpConnectionHandler::createShared(router);
        connectionHandler->setErrorHandler(std::make_shared<ErrorHandler>(objectMapper));
        return connectionHandler;

    }());

    OATPP_CREATE_COMPONENT(std::shared_ptr<Backend::Database>, database)([this] {

    oatpp::String connectionString = std::getenv("MONGO_CONN_STR");
    if(!connectionString){
      connectionString = m_cmdArgs.getNamedArgumentValue("--conn-str", "mongodb://localhost/zwoo");
    }

    mongocxx::uri uri(*connectionString);
    return std::make_shared<Backend::Database>(uri, "zwoo", "users");

  }());
};

#endif // _SERVER_COMPONENT_HPP_