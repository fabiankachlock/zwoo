#include "Server/DatabaseComponent.hpp"

#include "oatpp/core/data/stream/BufferStream.hpp"

#include <chrono>

#include <mongocxx/client.hpp>
#include <mongocxx/exception/operation_exception.hpp>
#include <mongocxx/options/insert.hpp>
#include <mongocxx/collection.hpp>
#include <mongocxx/options/find.hpp>

std::string Database::generateSID()
{
    std::string sid = sha512.hash(std::to_string(std::chrono::steady_clock::now().time_since_epoch().count()));
    sid.resize(24);
    return sid;
}

Database::Database(const mongocxx::uri& uri, const std::string& dbName, const std::string& collectionName): m_pool(std::make_shared<mongocxx::pool>(uri)), m_databaseName(dbName), m_collectionName(collectionName)
{
    auto conn = m_pool->acquire();
    auto collection = ( *conn ) [m_databaseName][m_collectionName];

    mongocxx::options::find opts;
    opts.sort(createMongoDocument(oatpp::Fields<oatpp::Int8>({{"_id", -1}})));
    opts.limit(1);
    auto res = collection.find({}, opts);

    bsoncxx::document::view doc = *res.begin();

    if (res.begin() != res.end())
    {
        auto bson = oatpp::String((const char *) doc.data(), doc.length());
        auto user = m_objectMapper.readFromString<oatpp::Object<UserDTO>>(bson);
        playerIDGenerator = UIDGenerator(user->_id);
    }
    else
        playerIDGenerator = UIDGenerator(0);

}

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
    auto conn = m_pool->acquire();
    auto collection = ( *conn ) [m_databaseName][m_collectionName];
    std::string code = randomNDigitNumber ( 6 );
    ulong puid = playerIDGenerator.GetID();

    auto usr = UserDTO::createShared();

    std::string salt = randomString(16);
    std::string hash = salt + password;
    for ( int i = 0; i < 10000; i++ )
        hash = sha512.hash(hash);
    hash.resize(24);
    std::string pw = "sha512:" + salt + ":" + hash;

    usr->_id = puid;
    usr->sid = "0";
    usr->username = user_name;
    usr->email = user_email;
    usr->password = pw;
    usr->wins = 0;
    usr->verified = false;
    usr->validation_code = code;

    collection.insert_one ( createMongoDocument ( usr ) );

    return { puid, code };
}

r_LoginUser Database::loginUser(std::string email, std::string password)
{
    auto usr = getUser("email", email);
    if (!usr)
        return { false, 0, "" };
    std::string salt = usr->password.getValue("").substr(7, 16);
    std::string hash = salt + password;
    for ( int i = 0; i < 10000; i++ )
        hash = sha512.hash(hash);
    hash.resize(24);
    std::string pw = "sha512:" + salt + ":" + hash;

    if (pw == usr->password.getValue("") && usr->verified)
    {
        std::string sid = generateSID();
        updateField("email", usr->email, "sid", sid);
        return { true, usr->_id, sid };
    }
    else
        return { false, 0, "" };

}

bool Database::verifyUser(ulong puid, std::string code)
{
    auto usr = getUser(puid);
    if (usr != nullptr)
    {
        if (usr->validation_code == code)
        {
            updateField("email", usr->email, "verified", true);
            return true;
        }
        return false;
    }
    else
        return false;
}

void Database::updateField( std::string filter_field, std::string filter_value, std::string field, bool value)
{
    auto conn = m_pool->acquire();
    auto collection = (*conn)[m_databaseName][m_collectionName];

    collection.update_one(
        createMongoDocument( // <-- Filter
    oatpp::Fields<oatpp::String>({
        { filter_field, filter_value }
    })
        ),
    createMongoDocument( // <-- Set
    oatpp::Fields<oatpp::Any>({ // map
        {   // pair
            "$set", oatpp::Fields<oatpp::Boolean>({{field, value}})
        } // pair
    }) // map906394
    )
    );
}

void Database::updateField( std::string filter_field, std::string filter_value, std::string field, std::string value)
{
    auto conn = m_pool->acquire();
    auto collection = (*conn)[m_databaseName][m_collectionName];

    collection.update_one(
        createMongoDocument( // <-- Filter
    oatpp::Fields<oatpp::String>({
        { filter_field, filter_value }
    })
        ),
    createMongoDocument( // <-- Set
    oatpp::Fields<oatpp::Any>({ // map
        {   // pair
            "$set", oatpp::Fields<oatpp::String>({{field, value}})
        } // pair
    }) // map
    )
    );
}

oatpp::Object<UserDTO> Database::getUser(ulong puid)
{
    auto conn = m_pool->acquire();
    auto collection = (*conn)[m_databaseName][m_collectionName];

    auto result =
        collection.find_one(createMongoDocument(// <-- Filter
    oatpp::Fields<oatpp::UInt32>({{"_id", puid}})));

    if (result) {
        auto view = result->view();
        auto bson = oatpp::String((const char *) view.data(), view.length());
        auto user = m_objectMapper.readFromString<oatpp::Object<UserDTO>>(bson);
        return user;
    }

    return nullptr;
}

oatpp::Object<UserDTO> Database::getUser(std::string field, std::string value)
{
    auto conn = m_pool->acquire();
    auto collection = (*conn)[m_databaseName][m_collectionName];

    auto result =
        collection.find_one(createMongoDocument(// <-- Filter
    oatpp::Fields<oatpp::String>({{field, value}})));

    if (result) {
        auto view = result->view();
        auto bson = oatpp::String((const char *) view.data(), view.length());
        auto user = m_objectMapper.readFromString<oatpp::Object<UserDTO>>(bson);
        return user;
    }

    return nullptr;
}
