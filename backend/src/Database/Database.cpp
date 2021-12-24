#include "Database/Database.h"

#include "oatpp/core/data/stream/BufferStream.hpp"

#include <mongocxx/client.hpp>
#include <mongocxx/exception/operation_exception.hpp>
#include <mongocxx/options/insert.hpp>

namespace Backend::Database {
    Database::Database(const mongocxx::uri &uri, const std::string &dbName, const std::string &collectionName)
        : m_pool(std::make_shared<mongocxx::pool>(uri)), m_databaseName(dbName), m_collectionName(collectionName) {}

    bsoncxx::document::value Database::createMongoDocument(const oatpp::Void &polymorph)
    {
        oatpp::data::stream::BufferOutputStream stream;
        m_objectMapper.write(&stream, polymorph);
        bsoncxx::document::view view(stream.getData(), stream.getCurrentPosition());
        return bsoncxx::document::value(view);
    }

    oatpp::Object<UserDTO> Database::createUser(std::string user_name, std::string email, std::string password) 
    {
        auto conn = m_pool->acquire();
        auto collection = (*conn)[m_databaseName][m_collectionName];

        

        collection.insert_one(createMongoDocument(userFromDto(userDto)));
    }
}// namespace Backend::Database