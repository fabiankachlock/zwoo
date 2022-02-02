#ifndef _DATABASE_COMPONENT_H_
#define _DATABASE_COMPONENT_H_

#include <string>

#include "oatpp-mongo/bson/mapping/ObjectMapper.hpp"

#include <mongocxx/pool.hpp>
#include <bsoncxx/document/value.hpp>

#include "SHA512.h"

struct r_CreateUser {
    std::string puid;
    std::string code;
};

class Database {
public:
    Database() {}

    std::shared_ptr<mongocxx::pool> m_pool;
    std::string m_databaseName;
    std::string m_collectionName;
    oatpp::mongo::bson::mapping::ObjectMapper m_objectMapper;

    bsoncxx::document::value createMongoDocument(const oatpp::Void &polymorph);
public:
    Database(const mongocxx::uri &uri, const std::string &dbName, const std::string &collectionName)
        : m_pool(std::make_shared<mongocxx::pool>(uri)), m_databaseName(dbName), m_collectionName(collectionName) {}

    r_CreateUser createUser(std::string user_name, std::string user_email, std::string password);
    bool entrieExists(std::string field, std::string value);
};

#endif // _DATABASE_COMPONENT_H_
