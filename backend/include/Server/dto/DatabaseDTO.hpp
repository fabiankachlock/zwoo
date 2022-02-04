#ifndef _CREATEUSERDTO_HPP_
#define _CREATEUSERDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN(DTO)

class CreateUserDTO : public oatpp::DTO
{
    DTO_INIT(CreateUserDTO, DTO)

    DTO_FIELD(String, username);
    DTO_FIELD(String, email);
    DTO_FIELD(String, password);
    DTO_FIELD(Boolean, verified);
    DTO_FIELD(String, validation_code);
};


class UserDTO : public oatpp::DTO {

    DTO_INIT(UserDTO, DTO)

    DTO_FIELD(UInt32, _id);
    DTO_FIELD(String, username);
    DTO_FIELD(String, email);
    DTO_FIELD(String, password);
    DTO_FIELD(Int32, wins);
    DTO_FIELD(String, validation_code);
    DTO_FIELD(Boolean, verified);
};

#include OATPP_CODEGEN_END(DTO)

#endif
