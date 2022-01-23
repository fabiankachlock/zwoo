#ifndef _SERVER_COMPONENT_HPP_
#define _SERVER_COMPONENT_HPP_

#include "zwoo.h"

#include "oatpp/network/tcp/server/ConnectionProvider.hpp"
#include "oatpp/web/server/HttpConnectionHandler.hpp"
#include "oatpp/web/server/HttpRouter.hpp"

#include "oatpp/parser/json/mapping/ObjectMapper.hpp"

#include "oatpp/core/macro/component.hpp"
#include "oatpp/core/base/CommandLineArguments.hpp"

#include "Server/ErrorHandler.hpp"

class ServerComponent {
public:

    ServerComponent()
    {}

public:
    /**
   * Create ObjectMapper component to serialize/deserialize DTOs in Contoller's API
   */
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::data::mapping::ObjectMapper>, apiObjectMapper)
    ([] {
        auto objectMapper = oatpp::parser::json::mapping::ObjectMapper::createShared();
        objectMapper->getDeserializer()->getConfig()->allowUnknownFields = false;
        return objectMapper;
    }());

    /**
   *  Create ConnectionProvider component which listens on the port
   */
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::network::ServerConnectionProvider>, serverConnectionProvider)
    ([] {
        return oatpp::network::tcp::server::ConnectionProvider::createShared({ZWOO_BACKEND_DOMAIN, ZWOO_BACKEND_PORT, oatpp::network::Address::IP_4});
    }());

    /**
   *  Create Router component
   */
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::web::server::HttpRouter>, httpRouter)
    ([] {
        return oatpp::web::server::HttpRouter::createShared();
    }());

    /**
   *  Create ConnectionHandler component which uses Router component to route requests
   */
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::network::ConnectionHandler>, serverConnectionHandler)
    ([] {
        OATPP_COMPONENT(std::shared_ptr<oatpp::web::server::HttpRouter>, router);          // get Router component
        OATPP_COMPONENT(std::shared_ptr<oatpp::data::mapping::ObjectMapper>, objectMapper);// get ObjectMapper component

        auto connectionHandler = oatpp::web::server::HttpConnectionHandler::createShared(router);
        connectionHandler->setErrorHandler(std::make_shared<ErrorHandler>(objectMapper));
        return connectionHandler;
    }());
};

#endif // _SERVER_COMPONENT_HPP_
