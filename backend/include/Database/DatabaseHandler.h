#ifndef _DATABASE_HANDLER_H_
#define _DATABASE_HANDLER_H_

#include <string>
#include <iostream>

#include "bsoncxx/builder/stream/document.hpp"
#include "bsoncxx/json.hpp"
#include "bsoncxx/oid.hpp"
#include "mongocxx/client.hpp"
#include "mongocxx/database.hpp"
#include "mongocxx/uri.hpp"

namespace Backend::Database
{
    class DatabaseHandler
    {
    public:
        DatabaseHandler(std::string host, int port, std::string db_name, std::string user, std::string password);
        ~DatabaseHandler();

        std::string GetId(std::string field_name, std::String value);

        bsoncxx::document::element GetElement(std::string &id, const char* field_name);
        bool SetElement(std::string &id, std::string field_name, std::string data);

        bool DeleteRow(std::string &id);

    private:
        const char *m_dbUri;
        const char *m_dbName;
        const char *m_dbCollectionName;
    };
}
#endif _DATABASE_HANDLER_H_