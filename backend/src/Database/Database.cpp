#include "Server/Database/Database.h"
#include "Server/Database/dto/CreateUser.hpp"

#include "oatpp/core/data/stream/BufferStream.hpp"

#include <mongocxx/client.hpp>
#include <mongocxx/exception/operation_exception.hpp>
#include <mongocxx/options/insert.hpp>

namespace Backend {
        

    bsoncxx::document::value Database::createMongoDocument(const oatpp::Void &polymorph)
    {
        oatpp::data::stream::BufferOutputStream stream;
        m_objectMapper.write(&stream, polymorph);
        bsoncxx::document::view view(stream.getData(), stream.getCurrentPosition());
        return bsoncxx::document::value(view);
    }

    bool Database::createUser(std::string user_name, std::string email, std::string password) 
    {
        auto conn = m_pool->acquire();
        auto collection = (*conn)[m_databaseName][m_collectionName];
        
        auto usr = CreateUserDTO::createShared();

        usr->username = user_name;
        usr->email = email;
        usr->password = password;
        usr->verified = false;

        collection.insert_one(createMongoDocument(usr));

        return true;
    }
}// namespace Backend::Database