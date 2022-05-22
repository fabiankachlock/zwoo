#include "HttpServer.h"

#include "Server/ServerComponent.hpp"
#include "Server/controller/Authentication/AuthenticationController.hpp"
#include "Server/controller/GameManager/GameManagerController.hpp"
#include "oatpp-mongo/bson/mapping/ObjectMapper.hpp"
#include "oatpp-openssl/Config.hpp"
#include "oatpp-openssl/server/ConnectionProvider.hpp"
#include "oatpp/network/Server.hpp"
#include "utils/thread.h"

#include <bsoncxx/document/value.hpp>
#include <bsoncxx/json.hpp>
#include <mongocxx/client.hpp>
#include <mongocxx/collection.hpp>
#include <mongocxx/pool.hpp>

#ifdef BUILD_SWAGGER
#include "oatpp-swagger/Controller.hpp"
#endif // BUILD_SWAGGER

void databaseCleanup( )
{
    auto logger = spdlog::get( "BED" );
    logger->info( "Database cleanup started..." );

    auto m_objectMapper =
        oatpp::mongo::bson::mapping::ObjectMapper::createShared( );
    // Connect to DB
    auto m_pool = std::make_shared<mongocxx::pool>(
        mongocxx::uri( ZWOO_DATABASE_CONNECTION_STRING ) );
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ "zwoo" ][ "users" ];
    // Delete unverified users
    oatpp::data::stream::BufferOutputStream stream;
    m_objectMapper->write(
        &stream, oatpp::Fields<oatpp::Boolean>( { { "verified", false } } ) );
    bsoncxx::document::view view( stream.getData( ),
                                  stream.getCurrentPosition( ) );

    collection.delete_many( bsoncxx::document::value( view ) );

    logger->info( "Database cleanup finished!" );
}

HttpServer::HttpServer( )
{
    logger = std::make_shared<Logger>( );
    logger->init( "BED" );
}

void HttpServer::RunServer( )
{
    oatpp::base::Environment::init( );

    auto server_component = ServerComponent( logger );

    /* Get router component */
    OATPP_COMPONENT( std::shared_ptr<oatpp::web::server::HttpRouter>, router );

#ifdef BUILD_SWAGGER
    oatpp::web::server::api::Endpoints docEndpoints;

    docEndpoints.append(
        router->addController( AuthenticationController::createShared( ) )
            ->getEndpoints( ) );
    docEndpoints.append(
        router->addController( GameManagerController::createShared( ) )
            ->getEndpoints( ) );
    router->addController(
        oatpp::swagger::Controller::createShared( docEndpoints ) );

    logger->log->info(
        "Added Swagger Endpoint: http://localhost:8000/swagger/ui" );
#endif

    router->addController( AuthenticationController::createShared( ) );
    router->addController( GameManagerController::createShared( ) );

    std::atomic<bool> stop_email_sender = false;
    OATPP_COMPONENT( std::shared_ptr<SynchronizedQueue<Email>>, emailQueue );
    std::thread email_thread(
        [ & ]( )
        {
            Email email;
            while ( !stop_email_sender.load( ) && emailQueue->empty( ) )
            {
                email = emailQueue->pop();

                if (email.email == "")
                    return;

                try
                {
                    logger->log->debug( "new user:\n puid: {},\n code: {}",
                                          email.puid, email.code );
                    // create mail message
                    mailio::message msg;
                    msg.from( mailio::mail_address(
                        "zwoo auth",
                        SMTP_HOST_EMAIL ) ); // set the correct sender
                                             // name and address
                    msg.add_recipient( mailio::mail_address(
                        "recipient",
                        email.email ) ); // set the correct recipent name and
                                         // address
                    msg.subject( "Verify your ZWOO Account" );
                    msg.content( generateVerificationEmailText(
                        email.puid, email.code, email.username ) );
                    // msg.content("Hello World!");
                    //  connect to server
                    mailio::smtps conn( SMTP_HOST_URL, SMTP_HOST_PORT );
                    // modify username/password to use real credentials
                    conn.authenticate(
                        SMTP_USERNAME, SMTP_PASSWORD,
                        mailio::smtps::auth_method_t::START_TLS );
                    conn.submit( msg );
                }
                catch ( mailio::smtp_error &exc )
                {
                    logger->log->error( "Email failed to send: {0}",
                                          exc.what( ) );
                }
                catch ( mailio::dialog_error &exc )
                {
                    logger->log->error( "Email failed to send: {0}",
                                          exc.what( ) );
                }
            }
        } );

    /* Get connection handler component */
    OATPP_COMPONENT( std::shared_ptr<oatpp::network::ConnectionHandler>,
                     connectionHandler, "http" );

    if ( ZWOO_BETA )
        logger->log->info( "Running in Beta Mode." );

    if ( USE_SSL )
    {
        auto conf = oatpp::openssl::Config::createDefaultServerConfigShared(
            SSL_CERTIFICATE, SSL_PEM );
        auto connectionProvider =
            oatpp::openssl::server::ConnectionProvider::createShared(
                conf, { ZWOO_BACKEND_DOMAIN, ZWOO_BACKEND_PORT,
                        oatpp::network::Address::IP_4 } );

        /* create server */
        oatpp::network::Server server( connectionProvider, connectionHandler );

        logger->log->info(
            "Running on port {0}...",
            connectionProvider->getProperty( "port" ).toString( )->c_str( ) );

        timedFunction( databaseCleanup, 0, 1 );

        server.run( );
    }
    else
    {
        auto connectionProvider =
            oatpp::network::tcp::server::ConnectionProvider::createShared(
                { ZWOO_BACKEND_DOMAIN, ZWOO_BACKEND_PORT,
                  oatpp::network::Address::IP_4 } );

        /* create server */
        oatpp::network::Server server( connectionProvider, connectionHandler );
        logger->log->info(
            "Running on port {0}...",
            connectionProvider->getProperty( "port" ).toString( )->c_str( ) );

        timedFunction( databaseCleanup, 0, 1 );

        server.run( );
    }

    stop_email_sender.store(true);
    emailQueue->push({ "", "", "", 0 });
    oatpp::base::Environment::destroy( );
}
