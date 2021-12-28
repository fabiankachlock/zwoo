#ifndef _GETUSERDTO_HPP_
#define _GETUSERDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN(DTO)

class GetUserDTO : public oatpp::DTO {

    DTO_INIT(GetUserDTO, DTO)

    DTO_FIELD(String, _id);
    DTO_FIELD(String, username);
    DTO_FIELD(String, email);
    DTO_FIELD(Int32, wins);
    DTO_FIELD(Boolean, verified);
};

#include OATPP_CODEGEN_END(DTO)

#endif// _GETUSERDTO_HPP_