#ifndef _ZRPCODES_H_
#define _ZRPCODES_H_

enum e_Roles {
    HOST = 1,
    PLAYER = 2,
    SPECTATOR = 3
};

enum e_ZRPOpCodes {
    PLAYER_JOINED = 100,
    SPECTATOR_JOINED = 101,
    PLAYER_LEFT = 102,
    SPECTATOR_LEFT = 103,
    SEND_MESSAGE = 104,
    RECEIVE_MESSAGE = 105,
    GET_ALL_PLAYERS_IN_LOBBY = 108,
    ALL_PLAYERS_IN_LOBBY = 109
};

#endif
