#ifndef _SERVER_COMPONENT_HPP_
#define _SERVER_COMPONENT_HPP_

#include "Server/AuthorizationHandler.hpp"
#include "Server/DatabaseComponent.hpp"
#include "Server/ErrorHandler.hpp"
#include "Server/ZwooRequestInterceptor.hpp"
#include "Server/ZwooResponseInterceptor.hpp"
#include "Server/controller/Authentication/Email.h"
#include "Server/logger/logger.h"
#include "controller/GameManager/websocket/ZRPConnector.hpp"
#include "controller/GameManager/websocket/ZwooInstanceListener.hpp"
#include "oatpp-websocket/ConnectionHandler.hpp"
#include "oatpp/core/base/CommandLineArguments.hpp"
#include "oatpp/core/macro/component.hpp"
#include "oatpp/network/tcp/server/ConnectionProvider.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"
#include "oatpp/web/server/HttpConnectionHandler.hpp"
#include "oatpp/web/server/HttpRouter.hpp"
#include "utils/Queue.hpp"
#include "zwoo.h"
#ifdef BUILD_SWAGGER
#include "Server/SwaggerComponent.hpp"
#endif

class ServerComponent
{
  private:
    std::shared_ptr<Logger> p_logger;

  public:
    ServerComponent( std::shared_ptr<Logger> _logger ) : p_logger( _logger ) {}

#ifdef BUILD_SWAGGER
    SwaggerComponent swaggerComponent;
#endif
    /**
     * Create ObjectMapper component to serialize/deserialize DTOs in
     * Contoller's API
     */
    OATPP_CREATE_COMPONENT( std::shared_ptr<oatpp::data::mapping::ObjectMapper>,
                            apiObjectMapper )
    (
        []
        {
            auto objectMapper =
                oatpp::parser::json::mapping::ObjectMapper::createShared( );
            objectMapper->getDeserializer( )->getConfig( )->allowUnknownFields =
                false;
            return objectMapper;
        }( ) );

    /**
     *  Create Router component
     */
    OATPP_CREATE_COMPONENT( std::shared_ptr<oatpp::web::server::HttpRouter>,
                            httpRouter )
    ( [] { return oatpp::web::server::HttpRouter::createShared( ); }( ) );

    /**
     *  Create ConnectionHandler component which uses Router component to route
     * requests
     */
    OATPP_CREATE_COMPONENT( std::shared_ptr<oatpp::network::ConnectionHandler>,
                            httpConnectionHandler )
    ( "http" /* qualifier */,
      []
      {
          OATPP_COMPONENT( std::shared_ptr<oatpp::web::server::HttpRouter>,
                           router ); // get Router component
          OATPP_COMPONENT( std::shared_ptr<oatpp::data::mapping::ObjectMapper>,
                           objectMapper ); // get ObjectMapper component

          auto httpconnectionHandler =
              oatpp::web::server::HttpConnectionHandler::createShared( router );
          httpconnectionHandler->setErrorHandler(
              std::make_shared<ErrorHandler>( objectMapper ) );

          /* Add CORS-enabling interceptors */
          httpconnectionHandler->addRequestInterceptor(
              std::make_shared<ZwooRequestInterceptor>( ) );
          httpconnectionHandler->addResponseInterceptor(
              std::make_shared<ZwooResponseInterceptor>( ) );

          return httpconnectionHandler;
      }( ) );

    OATPP_CREATE_COMPONENT( std::shared_ptr<Logger>, be_logger )
    ( "Backend", [ this ] { return p_logger; }( ) );

    OATPP_CREATE_COMPONENT( std::shared_ptr<Logger>, ws_logger )
    ( "Websocket",
      [ this ]
      {
          auto ws_logger = std::make_shared<Logger>( );
          ws_logger->init( "WBS" );
          return ws_logger;
      }( ) );

    OATPP_CREATE_COMPONENT( std::shared_ptr<Database>, database )
    (
        [ this ]
        {
            p_logger->log->info( "Mongodb: {0}",
                                 ZWOO_DATABASE_CONNECTION_STRING );
            return std::make_shared<Database>(
                mongocxx::uri( ZWOO_DATABASE_CONNECTION_STRING ), "zwoo",
                "users" );
        }( ) );

    /**
     *  Create websocket connection handler
     */
    OATPP_CREATE_COMPONENT( std::shared_ptr<oatpp::network::ConnectionHandler>,
                            websocketConnectionHandler )
    ( "websocket",
      []
      {
          OATPP_COMPONENT( std::shared_ptr<Logger>, m_logger_websocket,
                           "Websocket" );
          auto connectionHandler =
              oatpp::websocket::ConnectionHandler::createShared( );
          auto zrpc = std::make_shared<ZRPConnector>( );
          auto zil = std::make_shared<ZwooInstanceListener>( m_logger_websocket,
                                                             zrpc );

          connectionHandler->setSocketInstanceListener( zil );

          return connectionHandler;
      }( ) );

    OATPP_CREATE_COMPONENT( std::shared_ptr<ZwooAuthorizationHandler>,
                            authHandler )
    (
        []
        {
            OATPP_COMPONENT( std::shared_ptr<Database>, database );
            auto _authHandler =
                std::make_shared<ZwooAuthorizationHandler>( database );
            return _authHandler;
        }( ) );

    OATPP_CREATE_COMPONENT( std::shared_ptr<SynchronizedQueue<Email>>,
                            emailQueue )
    ( [] { return std::make_shared<SynchronizedQueue<Email>>( ); }( ) );
};

#endif // _SERVER_COMPONENT_HPP_
