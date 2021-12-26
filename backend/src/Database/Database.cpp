#include "Server/Database/Database.h"
#include "Server/Database/dto/CreateUser.hpp"

#include "oatpp/core/data/stream/BufferStream.hpp"

#include <mongocxx/client.hpp>
#include <mongocxx/exception/operation_exception.hpp>
#include <mongocxx/options/insert.hpp>

namespace Backend {


    bsoncxx::document::value Database::createMongoDocument(const oatpp::Void &polymorph) {
        oatpp::data::stream::BufferOutputStream stream;
        m_objectMapper.write(&stream, polymorph);
        bsoncxx::document::view view(stream.getData(), stream.getCurrentPosition());
        return bsoncxx::document::value(view);
    }

    bool Database::createUser(std::string user_name, std::string email, std::string password, std::string code) {
        auto conn = m_pool->acquire();
        auto collection = (*conn)[m_databaseName][m_collectionName];

        auto usr = UserDTO::createShared();

        usr->_id = email;
        usr->username = user_name;
        usr->email = email;
        usr->password = password;
        usr->wins = 0;
        usr->verified = false;
        usr->validation_code = code;

        collection.insert_one(createMongoDocument(usr));

        return true;
    }

    bool Database::updateUserField(std::string filter_field, std::string filter_value, std::string field, std::string value) {
        auto conn = m_pool->acquire();
        auto collection = (*conn)[m_databaseName][m_collectionName];

        auto filter = createMongoDocument(// <-- Filter
                oatpp::Fields<oatpp::String>({{filter_field, filter_value}}));

        auto doc = createMongoDocument(// <-- Set
                oatpp::Fields<oatpp::Any>({
                        // map
                        {                                                       // pair
                         "$set", oatpp::Fields<oatpp::String>({                 // you can also define a "strict" DTO for $set operation.
                                                               {field, value}})}// pair
                })                                                              // map
        );
        collection.update_one(createMongoDocument(// <-- Filter
                                      oatpp::Fields<oatpp::String>({{filter_field, filter_value}})),
                              createMongoDocument(// <-- Set
                                      oatpp::Fields<oatpp::Any>({
                                              // map
                                              {                                                       // pair
                                               "$set", oatpp::Fields<oatpp::String>({                 // you can also define a "strict" DTO for $set operation.
                                                                                     {field, value}})}// pair
                                      })                                                              // map
                                      ));
        return true;
    }

    bool Database::entrieExists(std::string field, std::string value) {
        auto conn = m_pool->acquire();
        auto collection = (*conn)[m_databaseName][m_collectionName];

        auto result =
                collection.find_one(createMongoDocument(// <-- Filter
                        oatpp::Fields<oatpp::String>({{field, value}})));

        return result ? true : false;
    }

    oatpp::Object<GetUserDTO> Database::getUser(std::string email) {
        auto conn = m_pool->acquire();
        auto collection = (*conn)[m_databaseName][m_collectionName];

        auto result =
                collection.find_one(createMongoDocument(// <-- Filter
                        oatpp::Fields<oatpp::String>({{"email", email}})));

        if (result) {
            auto view = result->view();
            auto bson = oatpp::String((const char *) view.data(), view.length());
            auto user = m_objectMapper.readFromString<oatpp::Object<GetUserDTO>>(bson);
            return user;
        }

        return nullptr;
    }

    oatpp::Object<GetUserDTO> Database::getUser(std::string field, std::string value) {
        auto conn = m_pool->acquire();
        auto collection = (*conn)[m_databaseName][m_collectionName];

        auto result =
                collection.find_one(createMongoDocument(// <-- Filter
                        oatpp::Fields<oatpp::String>({{field, value}})));

        if (result) {
            auto view = result->view();
            auto bson = oatpp::String((const char *) view.data(), view.length());
            auto user = m_objectMapper.readFromString<oatpp::Object<GetUserDTO>>(bson);
            return user;
        }

        return nullptr;
    }
}// namespace Backend