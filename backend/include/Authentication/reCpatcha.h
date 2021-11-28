#ifndef _RECPATCHA_H
#define _RECPATCHA_H

#include <string>
#include <sstream>

#include <curl/curl.h>
#include "zwoo.h"

namespace Backend::Authentication {
    std::string VerifiyCaptcha(std::string token);
}

#endif // _RECPATCHA_H