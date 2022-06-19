#include "Server/DatabaseComponent.hpp"

#include "Server/controller/error.h"
#include "oatpp/core/data/stream/BufferStream.hpp"

#include <bsoncxx/json.hpp>
#include <chrono>
#include <mongocxx/client.hpp>
#include <mongocxx/collection.hpp>
#include <mongocxx/options/find.hpp>
#include <bsoncxx/builder/stream/helpers.hpp>
#include <bsoncxx/builder/stream/document.hpp>

std::string Database::generateSID( )
{
    SHA512
    sha512 = SHA512( );
    std::string sid = sha512.hash( std::to_string(
        std::chrono::steady_clock::now( ).time_since_epoch( ).count( ) ) );
    sid.resize( 24 );
    return sid;
}

Database::Database( const mongocxx::uri &uri, const std::string &dbName,
                    const std::string &collectionName )
    : m_pool( std::make_shared<mongocxx::pool>( uri ) ),
      m_databaseName( dbName ), m_collectionName( collectionName )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    mongocxx::options::find opts;
    opts.sort( createMongoDocument(
        oatpp::Fields<oatpp::Int32>( { { "_id", -1 } } ) ) );
    opts.limit( 1 );
    auto res = collection.find( { }, opts );

    bsoncxx::document::view doc = *res.begin( );

    if ( res.begin( ) != res.end( ) )
    {
        auto bson = oatpp::String( (const char *)doc.data( ), doc.length( ) );
        auto user =
            m_objectMapper.readFromString<oatpp::Object<UserDTO>>( bson );
        playerIDGenerator = UIDGenerator( user->_id );
    }
    else
        playerIDGenerator = UIDGenerator( 0 );
    json_mapper = oatpp::parser::json::mapping::ObjectMapper::createShared( );
}

bsoncxx::document::value
Database::createMongoDocument( const oatpp::Void &polymorph )
{
    oatpp::data::stream::BufferOutputStream stream;
    m_objectMapper.write( &stream, polymorph );
    bsoncxx::document::view view( stream.getData( ),
                                  stream.getCurrentPosition( ) );
    return bsoncxx::document::value( view );
}

bool Database::entryExists( std::string field, std::string value )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    auto result = collection.find_one( createMongoDocument( // <-- Filter
        oatpp::Fields<oatpp::String>( { { field, value } } ) ) );

    return result ? true : false;
}

r_CreateUser Database::createUser( std::string user_name,
                                   std::string user_email,
                                   std::string password )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];
    std::string code = randomNDigitNumber( 6 );
    ulong puid = playerIDGenerator.GetID( );

    auto usr = UserDTO::createShared( );

    SHA512
    sha512 = SHA512( );

    std::string salt = randomString( 16 );
    std::string hash = salt + password;
    for ( int i = 0; i < 10000; i++ )
        hash = sha512.hash( hash );
    hash.resize( 24 );
    std::string pw = "sha512:" + salt + ":" + hash;

    usr->_id = puid;
    usr->sid = "";
    usr->username = user_name;
    usr->email = user_email;
    usr->password = pw;
    usr->wins = 0;
    usr->verified = false;
    usr->validation_code = code;

    collection.insert_one( createMongoDocument( usr ) );

    return { puid, code };
}

r_LoginUser Database::loginUser( std::string email, std::string password )
{
    auto usr = getUser( "email", email );
    if ( !usr )
        return { false, 0, "", e_Errors::USER_NOT_FOUND };

    SHA512
    sha512 = SHA512( );

    std::string salt = usr->password.getValue( "" ).substr( 7, 16 );
    std::string hash = salt + password;
    for ( int i = 0; i < 10000; i++ )
        hash = sha512.hash( hash );
    hash.resize( 24 );
    std::string pw = "sha512:" + salt + ":" + hash;

    if ( pw == usr->password.getValue( "" ) && usr->verified )
    {
        std::string sid = generateSID( );
        updateStringField( "email", usr->email, "sid", sid );
        return { true, usr->_id, sid };
    }
    else
        return { false, 0, "", e_Errors::PASSWORD_NOT_MATCHING };
}

bool Database::deleteUser( ulong puid, std::string password )
{
    auto usr = getUser( puid );
    if ( !usr )
        return false;

    SHA512
    sha512 = SHA512( );

    std::string salt = usr->password.getValue( "" ).substr( 7, 16 );
    std::string hash = salt + password;
    for ( int i = 0; i < 10000; i++ )
        hash = sha512.hash( hash );
    hash.resize( 24 );
    std::string pw = "sha512:" + salt + ":" + hash;

    if ( pw == usr->password.getValue( "" ) && usr->sid != "" )
    {
        auto conn = m_pool->acquire( );
        auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

        collection.delete_one( createMongoDocument(
            oatpp::Fields<oatpp::String>( { { "email", usr->email } } ) ) );

        return true;
    }
    else
        return false;
}

bool Database::verifyUser( ulong puid, std::string code )
{
    auto usr = getUser( puid );
    if ( usr != nullptr )
    {
        if ( usr->validation_code == code )
        {
            updateBooleanField( "email", usr->email, "verified", true );
            return true;
        }
        return false;
    }
    else
        return false;
}

void Database::updateBooleanField( std::string filter_field,
                                   std::string filter_value, std::string field,
                                   bool value )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    collection.update_one(
        createMongoDocument( // <-- Filter
            oatpp::Fields<oatpp::String>(
                { { filter_field, filter_value } } ) ),
        createMongoDocument( // <-- Set
            oatpp::Fields<oatpp::Any>( {
                // map
                { // pair
                  "$set", oatpp::Fields<oatpp::Boolean>(
                              { { field, value } } ) } // pair
            } )                                        // map906394
            ) );
}

bool Database::verifieAndUseBetaCode( std::string beta_code )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ "betacodes" ];

    auto result = collection.find_one( createMongoDocument( // <-- Filter
        oatpp::Fields<oatpp::String>( { { "code", beta_code } } ) ) );

    if ( result )
    {
        collection.delete_one( createMongoDocument(
            oatpp::Fields<oatpp::String>( { { "code", beta_code } } ) ) );

        return true;
    }

    return false;
}

void Database::updateStringField( std::string filter_field,
                                  std::string filter_value, std::string field,
                                  std::string value )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    collection.update_one(
        createMongoDocument( // <-- Filter
            oatpp::Fields<oatpp::String>(
                { { filter_field, filter_value } } ) ),
        createMongoDocument( // <-- Set
            oatpp::Fields<oatpp::Any>( {
                // map
                { // pair
                  "$set",
                  oatpp::Fields<oatpp::String>( { { field, value } } ) } // pair
            } )                                                          // map
            ) );
}

oatpp::Object<UserDTO> Database::getUser( ulong puid )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    auto result = collection.find_one( createMongoDocument( // <-- Filter
        oatpp::Fields<oatpp::UInt32>( { { "_id", puid } } ) ) );

    if ( result )
    {
        auto view = result->view( );
        auto bson = oatpp::String( (const char *)view.data( ), view.length( ) );
        auto user =
            m_objectMapper.readFromString<oatpp::Object<UserDTO>>( bson );
        return user;
    }

    return nullptr;
}

oatpp::Object<UserDTO> Database::getUser( std::string field, std::string value )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    auto result = collection.find_one( createMongoDocument( // <-- Filter
        oatpp::Fields<oatpp::String>( { { field, value } } ) ) );

    if ( result )
    {
        auto view = result->view( );
        auto bson = oatpp::String( (const char *)view.data( ), view.length( ) );
        auto user =
            m_objectMapper.readFromString<oatpp::Object<UserDTO>>( bson );
        return user;
    }

    return nullptr;
}

oatpp::Object<LeaderBoardDTO> Database::getLeaderBoard( )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    mongocxx::options::find opts;
    opts.sort( createMongoDocument(
        oatpp::Fields<oatpp::Int32>( { { "wins", -1 } } ) ) );
    opts.limit( 100 );
    auto res = collection.find( { }, opts );

    if ( res.begin( ) != res.end( ) )
    {
        auto leaderboard = LeaderBoardDTO::createShared( );
        leaderboard->top_players = { };

        for ( bsoncxx::document::view doc : res )
        {
            oatpp::String bson = bsoncxx::to_json( doc );
            auto user = json_mapper->readFromString<oatpp::Object<UserDTO>>(
                bson->c_str( ) );
            if ( user->verified )
            {
                auto player = LeaderBoardUserDTO::createShared( );
                player->username = user->username;
                player->wins = user->wins;
                leaderboard->top_players->push_back( player );
            }
        }
        return leaderboard;
    }
    else
        return LeaderBoardDTO::createShared( );
}
uint32_t Database::getPlayerLeaderboardPosition( uint64_t puid )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    auto usr = getUser(puid);

    if (usr)
    {
        using namespace bsoncxx::builder::basic;

        mongocxx::pipeline p{ };
        p.match( make_document( kvp(
            "wins", make_document( kvp(
                        "$gte", usr->wins ) ) ) ) );
        p.group( make_document(
            kvp( "_id", 0 ),
            kvp( "pos", make_document( kvp( "$sum", 1 ) ) ) ) );
        auto res = collection.aggregate( p );
        for (auto&& doc : res)
        {
            return doc["pos"].get_int32().value;
            break;
        }
    }
    return 0;
}
uint32_t Database::incrementWins( uint64_t puid )
{
    auto conn = m_pool->acquire( );
    auto collection = ( *conn )[ m_databaseName ][ m_collectionName ];

    using bsoncxx::builder::stream::close_document;
    using bsoncxx::builder::stream::document;
    using bsoncxx::builder::stream::finalize;
    using bsoncxx::builder::stream::open_document;

    collection.update_one(
        document{} << "_id" << open_document << "$eq" << (long)puid << close_document << finalize,
        document{} << "$inc" << open_document << "wins" << 1 << close_document << finalize
    );

    return getUser(puid)->wins;
}
