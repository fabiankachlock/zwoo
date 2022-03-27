#include "Server/ErrorHandler.hpp"

#include "zwoo.h"
#include "Server/controller/error.h"

#include "spdlog/spdlog.h"

ErrorHandler::ErrorHandler(const std::shared_ptr<oatpp::data::mapping::ObjectMapper>& objectMapper)
    : m_objectMapper(objectMapper)
{}

std::shared_ptr<ErrorHandler::OutgoingResponse>
ErrorHandler::handleError(const Status& status, const oatpp::String& message, const Headers& headers) {

    auto error = StatusDto::createShared();
    error->status = "ERROR";
    error->code = status.code;
    error->message = constructZwooErrorMessage("Backend Error", e_Errors::BACKEND_ERROR);

    auto response = ResponseFactory::createResponse(status, error, m_objectMapper);

    auto logger = spdlog::get("BED");
    logger->error("Backend error: {}", message->c_str());

    for(const auto& pair : headers.getAll()) {
        response->putHeader(pair.first.toString(), pair.second.toString());
    }

    response->putHeader("Access-Control-Allow-Origin", ZWOO_CORS);
    response->putHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
    response->putHeader("Access-Control-Allow-Credentials", "true");

    return response;
}
