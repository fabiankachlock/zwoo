#include "mongo.h"

#include <cstdint>
#include <string>

#include "bsoncxx/builder/stream/document.hpp"
#include "bsoncxx/json.hpp"
#include "bsoncxx/oid.hpp"
#include "mongocxx/client.hpp"
#include "mongocxx/database.hpp"
#include "mongocxx/uri.hpp"

void write_codes_to_db(std::vector<std::string> codes, std::string connection_string)
{
    auto uri = mongocxx::uri(connection_string);
    auto client = mongocxx::client(uri);
    mongocxx::collection collection = client["zwoo"]["betacodes"];
    auto builder = bsoncxx::builder::stream::document{};

    for (auto code : codes)
    {
        bsoncxx::document::value doc = builder << "code" << code << bsoncxx::builder::stream::finalize;
        collection.insert_one(doc.view());
    }
}