#ifndef _DATABASE_HANDLER_H_
#define _DATABASE_HANDLER_H_

#include <string>
#include <iostream>

#include "oatpp-mongo/bson/mapping/ObjectMapper.hpp"

#include <mongocxx/pool.hpp>
#include <bsoncxx/document/value.hpp>

namespace Backend::Database
{
    class DatabaseHandler
    {
    public:
    private:
    std::shared_ptr<mongocxx::pool> m_pool;
    std::string m_databaseName;
    std::string m_collectionName;
    oatpp::mongo::bson::mapping::ObjectMapper m_objectMapper;
    };
}
#endif // _DATABASE_HANDLER_H_