#ifndef _CREATEUSERDTO_HPP_
#define _CREATEUSERDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN(DTO)

class CreateUserDTO : public oatpp::DTO
{
    DTO_INIT(CreateUserDTO, DTO)

    DTO_FIELD(String, _id);
    DTO_FIELD(String, username);
    DTO_FIELD(String, email);
    DTO_FIELD(String, password);
};

#include OATPP_CODEGEN_END(DTO)

#endif 