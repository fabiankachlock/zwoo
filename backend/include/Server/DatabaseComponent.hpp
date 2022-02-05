#ifndef _DATABASE_COMPONENT_H_
#define _DATABASE_COMPONENT_H_

#include <string>

#include "oatpp-mongo/bson/mapping/ObjectMapper.hpp"

#include <mongocxx/pool.hpp>
#include <bsoncxx/document/value.hpp>

#include "Server/dto/DatabaseDTO.hpp"

#include "SHA512.h"
#include "Helper.h"

struct r_CreateUser {
    ulong puid;
    std::string code;
};

struct r_LoginUser {
    bool successful;
    ulong puid;
    std::string sid;
};

class Database {
private:
    Database() {}

    std::shared_ptr<mongocxx::pool> m_pool;
    std::string m_databaseName;
    std::string m_collectionName;
    oatpp::mongo::bson::mapping::ObjectMapper m_objectMapper;

    bsoncxx::document::value createMongoDocument(const oatpp::Void &polymorph);
    std::string generateSID();

    UIDGenerator playerIDGenerator;
    SHA512 sha512 = SHA512();
public:
    Database(const mongocxx::uri &uri, const std::string &dbName, const std::string &collectionName);

    r_CreateUser createUser(std::string user_name, std::string user_email, std::string password);
    bool verifyUser(ulong puid, std::string code);
    r_LoginUser loginUser(std::string email, std::string password);
    bool deleteUser(ulong puid, std::string sid, std::string password);
    oatpp::Object<UserDTO> getUser(ulong puid);
    oatpp::Object<UserDTO> getUser(std::string field, std::string value);
    bool entrieExists(std::string field, std::string value);
    void updateField( std::string filter_field, std::string filter_value, std::string field, bool value);
    void updateField( std::string filter_field, std::string filter_value, std::string field, std::string value);
};

#endif // _DATABASE_COMPONENT_H_
