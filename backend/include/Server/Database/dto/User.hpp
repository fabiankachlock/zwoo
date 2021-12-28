#ifndef _USERDTO_HPP_
#define _USERDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN(DTO)

class UserDTO : public oatpp::DTO {

    DTO_INIT(UserDTO, DTO)

    DTO_FIELD(String, _id);
    DTO_FIELD(String, username);
    DTO_FIELD(String, email);
    DTO_FIELD(String, password);
    DTO_FIELD(Int32, wins);
    DTO_FIELD(String, validation_code);
    DTO_FIELD(Boolean, verified);
};

#include OATPP_CODEGEN_END(DTO)

#endif// _USERDTO_HPP_