#ifndef _DATABASE_COMPONENT_H_
#define _DATABASE_COMPONENT_H_

#include "Helper.h"
#include "SHA512.h"
#include "Server/dto/DatabaseDTO.hpp"
#include "oatpp-mongo/bson/mapping/ObjectMapper.hpp"
#include "oatpp/parser/json/mapping/ObjectMapper.hpp"

#include <bsoncxx/document/value.hpp>
#include <mongocxx/pool.hpp>
#include <string>

struct r_CreateUser
{
    ulong puid;
    std::string code;
};

struct r_LoginUser
{
    bool successful;
    ulong puid;
    std::string sid;
    int error_code;
};

class Database
{
  private:
    Database( ) {}

    std::shared_ptr<mongocxx::pool> m_pool;
    std::string m_databaseName;
    std::string m_collectionName;
    oatpp::mongo::bson::mapping::ObjectMapper m_objectMapper;
    std::shared_ptr<oatpp::parser::json::mapping::ObjectMapper> json_mapper;

    bsoncxx::document::value
    createMongoDocument( const oatpp::Void &polymorph );
    std::string generateSID( );

    UIDGenerator playerIDGenerator;

  public:
    Database( const mongocxx::uri &uri, const std::string &dbName,
              const std::string &collectionName );

    r_CreateUser createUser( std::string user_name, std::string user_email,
                             std::string password );
    bool verifyUser( ulong puid, std::string code );
    r_LoginUser loginUser( std::string email, std::string password );
    bool deleteUser( ulong puid, std::string password );
    oatpp::Object<UserDTO> getUser( ulong puid );
    oatpp::Object<UserDTO> getUser( std::string field, std::string value );
    bool entryExists( std::string field, std::string value );
    void updateStringField( std::string filter_field, std::string filter_value,
                            std::string field, std::string value );
    void updateBooleanField( std::string filter_field, std::string filter_value,
                             std::string field, bool value );
    bool verifieAndUseBetaCode( std::string beta_code );

    uint32_t getPlayerLeaderboardPosition( uint64_t puid );

    oatpp::Object<LeaderBoardDTO> getLeaderBoard( );

    uint32_t incrementWins( uint64_t puid );
};

#endif // _DATABASE_COMPONENT_H_
