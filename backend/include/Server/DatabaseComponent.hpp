#ifndef _DATABASE_COMPONENT_H_
#define _DATABASE_COMPONENT_H_

#include "oatpp-mongo/bson/mapping/ObjectMapper.hpp"

#include <mongocxx/pool.hpp>
#include <bsoncxx/document/value.hpp>

class Database {
public:
    Database() {}
public:
    std::shared_ptr<mongocxx::pool> m_pool;
    std::string m_databaseName;
    std::string m_collectionName;
    oatpp::mongo::bson::mapping::ObjectMapper m_objectMapper;

    Database(const mongocxx::uri &uri, const std::string &dbName, const std::string &collectionName)
        : m_pool(std::make_shared<mongocxx::pool>(uri)), m_databaseName(dbName), m_collectionName(collectionName) {}
};

#endif // _DATABASE_COMPONENT_H_
