#ifndef _AUTHENTICATIONDTO_HPP_
#define _AUTHENTICATIONDTO_HPP_

#include "oatpp/core/macro/codegen.hpp"
#include "oatpp/core/Types.hpp"

#include OATPP_CODEGEN_BEGIN(DTO)

class CreateUserBodyDTO : public oatpp::DTO {

    DTO_INIT(CreateUserBodyDTO, DTO)

    DTO_FIELD_INFO(username) {
        info->description = "Username";
    }
    DTO_FIELD(String, username);

    DTO_FIELD_INFO(email) {
        info->description = "Users email";
    }
    DTO_FIELD(String, email);

    DTO_FIELD_INFO(password) {
        info->description = "Users Password";
    }
    DTO_FIELD(String, password);

};

class LoginUserDTO : public oatpp::DTO {

    DTO_INIT(LoginUserDTO, DTO)

    DTO_FIELD_INFO(email) {
        info->description = "Users email";
    }
    DTO_FIELD(String, email);

    DTO_FIELD_INFO(password) {
        info->description = "Users Password";
    }
    DTO_FIELD(String, password);

};

class LogoutUserDTO : public oatpp::DTO {

    DTO_INIT(LogoutUserDTO, DTO)

    DTO_FIELD_INFO(sid) {
        info->description = "Players session ID";
    }
    DTO_FIELD(String, sid);

    DTO_FIELD_INFO(puid) {
        info->description = "Players unique ID";
    }
    DTO_FIELD(UInt32, puid);
};

class GetUserDTO : public oatpp::DTO {

    DTO_INIT(GetUserDTO, DTO)

    DTO_FIELD_INFO(sid) {
        info->description = "Players session ID";
    }
    DTO_FIELD(String, sid);

    DTO_FIELD_INFO(puid) {
        info->description = "Players unique ID";
    }
    DTO_FIELD(UInt32, puid);
};

class GetUserResponseDTO : public oatpp::DTO {

    DTO_INIT(GetUserResponseDTO, DTO)

    DTO_FIELD_INFO(username) {
        info->description = "Username";
    }
    DTO_FIELD(String, username);

    DTO_FIELD_INFO(email) {
        info->description = "Users email";
    }
    DTO_FIELD(String, email);

    DTO_FIELD_INFO(wins) {
        info->description = "wins";
    }
    DTO_FIELD(Int32, wins);
};



class DeleteUserDTO : public oatpp::DTO {
    DTO_INIT(DeleteUserDTO, DTO)

    DTO_FIELD_INFO(sid) {
        info->description = "Players session ID";
    }
    DTO_FIELD(String, sid);

    DTO_FIELD_INFO(puid) {
        info->description = "Players unique ID";
    }
    DTO_FIELD(UInt32, puid);

    DTO_FIELD_INFO(password) {
        info->description = "Users Password";
    }
    DTO_FIELD(String, password);
};

#include OATPP_CODEGEN_END(DTO)

#endif // _AUTHENTICATIONDTO_HPP_

