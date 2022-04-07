#ifndef _ZRPMESSAGEDTO_HPP_
#define _ZRPMESSAGEDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN( DTO ) ///< Begin DTO codegen section

class ZwooUser : public oatpp::DTO
{
    DTO_INIT( ZwooUser, DTO )

    DTO_FIELD( String, name ) = "";
    DTO_FIELD( UInt32, wins ) = (v_uint32)0;
    DTO_FIELD( UInt8, role ) = (v_uint8)0;
};

class UserJoined : public oatpp::DTO
{
    DTO_INIT( UserJoined, DTO )

    DTO_FIELD( String, name ) = "";
    DTO_FIELD( UInt32, wins ) = (v_uint32)0;
    DTO_FIELD( UInt8, role ) = (v_uint8)0;
};

class SendMessage : public oatpp::DTO
{

    DTO_INIT( SendMessage, DTO )

    DTO_FIELD( String, message ) = "";
};

class ReceiveMessage : public oatpp::DTO
{

    DTO_INIT( ReceiveMessage, DTO /* extends */ )

    DTO_FIELD( String, message ) = "";
    DTO_FIELD( String, name ) = "";
    DTO_FIELD( UInt8, role ) = (v_uint8)0;
};

class PlayersInLobby : public oatpp::DTO
{

    DTO_INIT( PlayersInLobby, DTO /* extends */ )

    DTO_FIELD( List<oatpp::Object<ZwooUser>>, players );
};

#include OATPP_CODEGEN_END( DTO ) ///< End DTO codegen section

#endif // _ZRPMESSAGEDTO_HPP_
