#ifndef _USERDTO_HPP_
#define _USERDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

class UserDTO : public oatpp::DTO {

    DTO_INIT(User, DTO)

    DTO_FIELD(String, _id);
    DTO_FIELD(String, username);
    DTO_FIELD(String, email);
    DTO_FIELD(String, password);
    DTO_FIELD(Int, wins);
    DTO_FIELD(Boolean, verified)
};

class CreateUserDTO : public oatpp::DTO
{
    DTO_INIT(User, DTO)

    DTO_FIELD(String, _id);
    DTO_FIELD(String, username);
    DTO_FIELD(String, email);
    DTO_FIELD(String, password);
};


#endif// _USERDTO_HPP_