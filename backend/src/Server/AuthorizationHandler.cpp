#include "Server/AuthorizationHandler.hpp"

#include "oatpp/encoding/Base64.hpp"
#include "oatpp/core/parser/Caret.hpp"
#include "oatpp/web/protocol/http/incoming/Request.hpp"
#include "oatpp/web/protocol/http/Http.hpp"
#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/data/mapping/type/Type.hpp"

#include "Server/controller/error.h"

#include "Helper.h"
#include "zwoo.h"

ZwooAuthorizationHandler::ZwooAuthorizationHandler(std::shared_ptr<Database> db)
    : AuthorizationHandler("Auth", "Zwoo"), db(db)
{}

std::shared_ptr<oatpp::web::server::handler::AuthorizationObject> ZwooAuthorizationHandler::handleAuthorization(const oatpp::String &header)
{
    auto logger = spdlog::get("BED");
    if (header == nullptr)
        logger->info("Header: nullptr");
    else
        logger->info("Header: {}", header->c_str());

    std::string cookie = header.getValue("");
    if (cookie.length() == 0)
        throw HttpError(Status::CODE_401, constructErrorMessage("Cookie Missing", e_Errors::COOKIE_MISSING));
    auto spos = cookie.find("auth=");
    if (spos < 0 || spos > cookie.length())
        throw HttpError(Status::CODE_401, constructErrorMessage("Cookie Missing", e_Errors::COOKIE_MISSING));

    cookie = decrypt(decodeBase64(cookie.substr(spos + 5, cookie.find(';', spos) - spos - 5)));
    auto pos = cookie.find(",");
    std::string p = cookie.substr(0, pos);
    std::string s = cookie.substr(pos + 1, 24);
    uint64_t puid;
    std::stringstream ss(p);
    ss >> puid;

    auto usr = db->getUser(puid);

    if (usr)
    {
        if (usr->sid == s)
        {
            return std::make_shared<UserAuthorizationObject>(puid, usr->username, usr->email, usr->wins);
        }
        else
            throw HttpError(Status::CODE_401, constructErrorMessage("Session ID not Matching", e_Errors::SESSION_ID_NOT_MATCHING), getHeader());
    }
    else
        throw HttpError(Status::CODE_404, constructErrorMessage("User not Found", e_Errors::USER_NOT_FOUND), getHeader());

    return nullptr;
}