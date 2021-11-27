#ifndef _AUTHENTICATIONDTO_HPP_
#define _AUTHENTICATIONDTO_HPP_

#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/Types.hpp"

#include "Server/dto/AuthenticationDTO.hpp"

#include OATPP_CODEGEN_BEGIN(DTO)

class reCaptchaDto : public oatpp::DTO {

    DTO_INIT(reCaptchaDto, DTO)

    DTO_FIELD_INFO(success) {
        info->description = "";
    }
    DTO_FIELD(Boolean, success);

    DTO_FIELD_INFO(challenge_ts) {
        info->description = "timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)";
    }
    DTO_FIELD(String, challenge_ts);

    DTO_FIELD_INFO(hostname) {
        info->description = "the hostname of the site where the reCAPTCHA was solved";
    }
    DTO_FIELD(String, hostname);

    DTO_FIELD_INFO(errorCode) {
        info->description = "optional";
    }
    DTO_FIELD(Any, errorCode);
};

#include OATPP_CODEGEN_END(DTO)

#endif // _AUTHENTICATIONDTO_HPP_
