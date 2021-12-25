#ifndef _DATABASE_HANDLER_H_
#define _DATABASE_HANDLER_H_

#include <string>
#include <iostream>

#include "oatpp-mongo/bson/mapping/ObjectMapper.hpp"

#include <mongocxx/pool.hpp>
#include <bsoncxx/document/value.hpp>

#include "dto/User.hpp"
#include "dto/GetUser.hpp"

namespace Backend
{
    class Database
    {
    private:
        std::shared_ptr<mongocxx::pool> m_pool;
        std::string m_databaseName;
        std::string m_collectionName;
        oatpp::mongo::bson::mapping::ObjectMapper m_objectMapper;

        bsoncxx::document::value createMongoDocument(const oatpp::Void &polymorph);
    public:
        Database() {}
        Database(const mongocxx::uri &uri, const std::string &dbName, const std::string &collectionName) 
        : m_pool(std::make_shared<mongocxx::pool>(uri)), m_databaseName(dbName), m_collectionName(collectionName) {}

        bool createUser(std::string user_name, std::string email, std::string password, std::string code);
        oatpp::Object<GetUserDTO> getUser(std::string email);
        bool entrieExists(std::string field, std::string value);
    };
}
#endif // _DATABASE_HANDLER_H_