#ifndef _ZRPMESSAGEDTO_HPP_
#define _ZRPMESSAGEDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN( DTO ) ///< Begin DTO codegen section

class ZwooUser : public oatpp::DTO
{
    DTO_INIT( ZwooUser, DTO )

    DTO_FIELD( String, username ) = "";
    DTO_FIELD( UInt32, wins ) = (v_uint32)0;
    DTO_FIELD( UInt8, role ) = (v_uint8)0;
};

class UserJoined : public oatpp::DTO
{
    DTO_INIT( UserJoined, DTO )

    DTO_FIELD( String, username ) = "";
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
    DTO_FIELD( String, username ) = "";
    DTO_FIELD( UInt8, role ) = (v_uint8)0;
};

class PlayersInLobby : public oatpp::DTO
{

    DTO_INIT( PlayersInLobby, DTO /* extends */ )

    DTO_FIELD( List<oatpp::Object<ZwooUser>>, players );
};

class KickPlayer : public oatpp::DTO
{
    DTO_INIT( KickPlayer, DTO )

    DTO_FIELD( String, username ) = "";
};

class PlayerToHost : public oatpp::DTO
{
    DTO_INIT( PlayerToHost, DTO )

    DTO_FIELD( String, username ) = "";
};

class PlayerToSpectator : public oatpp::DTO
{
    DTO_INIT( PlayerToSpectator, DTO )

    DTO_FIELD( String, username ) = "";
};

class CardDTO : public oatpp::DTO {
    DTO_INIT( CardDTO, DTO )

    DTO_FIELD( UInt8, type ) = (uint8_t)0;
    DTO_FIELD( UInt8, symbol ) = (uint8_t)0;
};

class PlayerHandDTO : public oatpp::DTO {
    DTO_INIT(PlayerHandDTO, DTO );

    DTO_FIELD(List<oatpp::Object<CardDTO>>, hand) = {};
};

class StateChangedDTO : public oatpp::DTO {
    DTO_INIT(StateChangedDTO, DTO );

    DTO_FIELD(oatpp::Object<CardDTO>, pileTop);
    DTO_FIELD(String, activePlayer) = "";
    DTO_FIELD(UInt32 , activePlayerCardAmount) = (uint32_t)0;
    DTO_FIELD(String, lastPlayer) = "";
    DTO_FIELD(UInt32 , lastPlayerCardAmount) = (uint32_t)0;
};

class PlayerDTO : oatpp::DTO {
    DTO_INIT(PlayerDTO, DTO );

    DTO_FIELD(String, username) = "";
    DTO_FIELD(UInt32, cards) = (uint32_t)0;
    DTO_FIELD(Boolean, isActivePlayer) = false;
};

class PlayerCardAmountDTO : public oatpp::DTO {
    DTO_INIT(PlayerCardAmountDTO, DTO );

    DTO_FIELD(List<oatpp::Object<PlayerDTO>>, players) = {};
};

class PlayerDecisionDTO : public oatpp::DTO {
    DTO_INIT(PlayerDecisionDTO, DTO );

    DTO_FIELD(UInt8, type) = (uint8_t)0;
    DTO_FIELD(UInt32, decision) = (uint32_t)0;
};

class PlayerSummaryDTO : public oatpp::DTO {
    DTO_INIT(PlayerSummaryDTO, DTO );

    DTO_FIELD(String, username) = "";
    DTO_FIELD(UInt32, position) = (uint32_t)0;
    DTO_FIELD(UInt32, score) = (uint32_t)0;
};

class PlayerWonDTO : public oatpp::DTO {
    DTO_INIT(PlayerWonDTO, DTO );

    DTO_FIELD(String, username) = "";
    DTO_FIELD(UInt32, wins) = (uint32_t)0;
    DTO_FIELD(List<oatpp::Object<PlayerSummaryDTO>>, summary) = {};
};

#include OATPP_CODEGEN_END( DTO ) ///< End DTO codegen section

#endif // _ZRPMESSAGEDTO_HPP_
