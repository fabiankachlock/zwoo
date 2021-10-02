#ifndef _MONGO_DB_HANDLER_H_
#define _MONGO_DB_HANDLER_H_

#include <string>

#include "bsoncxx/builder/stream/document.hpp"
#include "bsoncxx/json.hpp"
#include "bsoncxx/oid.hpp"
#include "mongocxx/client.hpp"
#include "mongocxx/database.hpp"
#include "mongocxx/uri.hpp"


namespace Backend::Database
{
    class MongoDBHandler
    {
    private:
        const char* m_dbUri;
        const char* m_dbName;
        const char* m_dbCollectionName;
    public:
        MongoDBHandler(const char* _db_uri, const char* _db_name, const char* _collection_name);
        ~MongoDBHandler();

        template <class T>
        bool GetElement(const std::string &id, const char* field_name, T &result);
        
        template <class T>
        bool SetElement(const std::string &id, const char* field_name, T data);

        bool DeleteRow(const std::string &id);
    };
} // namespace Backend::Database
#endif