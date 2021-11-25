#include "Database/DatabaseHandler.h"

namespace Backend::Database
{
    DatabaseHandler::DatabaseHandler(std::string host, int port, std::string db_name, std::string user, std::string password)
    {
        
    }

    DatabaseHandler::~DatabaseHandler()
    {

    }

    std::string DatabaseHandler::GetId(std::string field_name, std::String value)
    {

    }

    bsoncxx::document::element DatabaseHandler::GetElement(std::string &id, const char* field_name)
    {

    }
    
    bool DatabaseHandler::SetElement(std::string &id, std::string field_name, std::string data)
    {

    }
}