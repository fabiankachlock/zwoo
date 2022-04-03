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
    : oatpp::web::server::handler::BearerAuthorizationHandler("zwoo"), db(db)
{}

 std::shared_ptr<oatpp::web::server::handler::AuthorizationObject> ZwooAuthorizationHandler::authorize(const oatpp::String &token)
{
    auto data = decrypt(decodeBase64(token.getValue("")));
    auto pos = data.find(",");
    std::string p = data.substr(0, pos);
    std::string s = data.substr(pos + 1, 24);
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
            throw HttpError(Status::CODE_401, constructErrorMessage("Session ID not Matching", e_Errors::SESSION_ID_NOT_MATCHING));
    }
    else
        throw HttpError(Status::CODE_404, constructErrorMessage("User not Found", e_Errors::USER_NOT_FOUND));

    return nullptr;
}
