#ifndef _CREATEUSERDTO_HPP_
#define _CREATEUSERDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN( DTO )

class CreateUserDTO : public oatpp::DTO
{
    DTO_INIT( CreateUserDTO, DTO )

    DTO_FIELD( String, username ) = "";
    DTO_FIELD( String, email ) = "";
    DTO_FIELD( String, password ) = "";
    DTO_FIELD( Boolean, verified ) = false;
    DTO_FIELD( String, validation_code ) = "";
};

class UserDTO : public oatpp::DTO
{

    DTO_INIT( UserDTO, DTO )

    DTO_FIELD( UInt32, _id ) = (v_uint32)0;
    DTO_FIELD( String, sid ) = "";
    DTO_FIELD( String, username ) = "";
    DTO_FIELD( String, email ) = "";
    DTO_FIELD( String, password ) = "";
    DTO_FIELD( Int32, wins ) = (v_int32)0;
    DTO_FIELD( String, validation_code ) = "";
    DTO_FIELD( Boolean, verified ) = false;
};

class LeaderBoardUserDTO : public oatpp::DTO
{

    DTO_INIT( LeaderBoardUserDTO, DTO )

    DTO_FIELD( String, username ) = "";
    DTO_FIELD( Int32, wins ) = (v_int32)0;
};

class LeaderBoardDTO : public oatpp::DTO
{

    DTO_INIT( LeaderBoardDTO, DTO )

    DTO_FIELD( List<oatpp::Object<LeaderBoardUserDTO>>, top_players,
               "leaderboard" );
};

#include OATPP_CODEGEN_END( DTO )

#endif
