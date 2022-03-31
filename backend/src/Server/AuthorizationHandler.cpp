#include "Server/AuthorizationHandler.hpp"

ZwooAuthorizationHandler::ZwooAuthorizationHandler(std::shared_ptr<Database> db)
    : AuthorizationHandler("Cookie", "zwoo"), db(db)
{}
