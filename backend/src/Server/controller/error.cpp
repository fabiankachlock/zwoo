#include "Server/controller/error.h"

std::string constructErrorMessage(std::string message, e_Errors code)
{
    return "{\"message\": " + message + ", \"code\": " + std::to_string((int)code) + "}";
}
