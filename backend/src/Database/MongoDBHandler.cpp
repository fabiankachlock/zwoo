#include "Database/MongoDBHandler.h"

namespace Backend::Database
{

    MongoDBHandler::MongoDBHandler(const char* _db_uri, const char* _db_name, const char* _collection_name)
                                  : m_dbUri(_db_uri)  , m_dbName(_db_name)  ,  m_dbCollectionName(_collection_name)
    {

    }

    MongoDBHandler::~MongoDBHandler()
    {

    }

    template <class T>
    bool MongoDBHandler::GetElement(const std::string &id, const char* field_name, T &result)
    {
        return true;
    }

    template <class T>
    bool MongoDBHandler::SetElement(const std::string &id, const char* field_name, T data)
    {
        return true;
    }

    bool MongoDBHandler::DeleteRow(const std::string &id)
    {
        return true;
    }

}