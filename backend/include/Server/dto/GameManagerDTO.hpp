#ifndef _GAMEMANAGERDTO_HPP_
#define _GAMEMANAGERDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN( DTO )

class JoinGameDTO : public oatpp::DTO
{
    DTO_INIT( JoinGameDTO, DTO )

    DTO_FIELD( String, name ) = "";
    DTO_FIELD( String, password ) = "";
    DTO_FIELD( UInt8, opcode ) = (v_uint8)0;
    DTO_FIELD( UInt32, guid ) = (v_uint32)0;
    DTO_FIELD( Boolean, use_password ) = false;
};

class GameDTO : public oatpp::DTO
{
    DTO_INIT( GameDTO, DTO )

    DTO_FIELD( String, name ) = "";
    DTO_FIELD( Boolean, isPublic ) = false;
    DTO_FIELD( UInt32, id ) = (uint32_t)0;
    DTO_FIELD( UInt32, playerCount ) = (uint32_t)0;
};

class GetGameDTO : public oatpp::DTO
{
    DTO_INIT( GetGameDTO, DTO )

    DTO_FIELD( List<oatpp::Object<GameDTO>>, games ) = { };
};

#include OATPP_CODEGEN_END( DTO )

#endif // _GAMEMANAGERDTO_HPP_
