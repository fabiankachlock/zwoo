#include "Server/DatabaseComponent.hpp"

#include "oatpp/core/data/stream/BufferStream.hpp"

#include <mongocxx/client.hpp>
#include <mongocxx/exception/operation_exception.hpp>
#include <mongocxx/options/insert.hpp>

#include "Helper.h"
#include "Server/dto/DatabaseDTO.hpp"

bsoncxx::document::value Database::createMongoDocument ( const oatpp::Void &polymorph )
{
    oatpp::data::stream::BufferOutputStream stream;
    m_objectMapper.write ( &stream, polymorph );
    bsoncxx::document::view view ( stream.getData(), stream.getCurrentPosition() );
    return bsoncxx::document::value ( view );
}

bool Database::entrieExists ( std::string field, std::string value )
{
    auto conn = m_pool->acquire();
    auto collection = ( *conn ) [m_databaseName][m_collectionName];

    auto result =
        collection.find_one ( createMongoDocument ( // <-- Filter
    oatpp::Fields<oatpp::String> ( {{field, value}} ) ) );

    return result ? true : false;
}

r_CreateUser Database::createUser ( std::string user_name, std::string user_email, std::string password )
{
    std::string code = randomNDigitNumber ( 6 );
    std::string puid = generateUniqueHash();
    auto conn = m_pool->acquire();
    auto collection = ( *conn ) [m_databaseName][m_collectionName];

    auto usr = UserDTO::createShared();

    usr->_id = puid;
    usr->username = user_name;
    usr->email = user_email;
    usr->password = password;
    usr->wins = 0;
    usr->verified = false;
    usr->validation_code = code;

    collection.insert_one ( createMongoDocument ( usr ) );

    return { puid, code };
}
