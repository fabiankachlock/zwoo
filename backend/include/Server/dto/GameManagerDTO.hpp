#ifndef _GAMEMANAGERDTO_HPP_
#define _GAMEMANAGERDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN(DTO)

class CreateGameDTO : public oatpp::DTO
{
    DTO_INIT(CreateGameDTO, DTO)

    DTO_FIELD(String, game_name);
    DTO_FIELD(String, password);
    DTO_FIELD(Boolean, use_password);
};

#include OATPP_CODEGEN_END(DTO)

#endif // _GAMEMANAGERDTO_HPP_
