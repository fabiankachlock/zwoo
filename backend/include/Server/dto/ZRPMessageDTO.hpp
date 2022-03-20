#ifndef _ZRPMESSAGEDTO_HPP_
#define _ZRPMESSAGEDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN(DTO) ///< Begin DTO codegen section

class SendMessage : public oatpp::DTO {

  DTO_INIT(SendMessage, DTO /* extends */)

  DTO_FIELD(String, message);

};

class ReceiveMessage : public oatpp::DTO {

  DTO_INIT(ReceiveMessage, DTO /* extends */)

  DTO_FIELD(String, message);
  DTO_FIELD(String, name);
  DTO_FIELD(UInt8, role);
};

#include OATPP_CODEGEN_END(DTO) ///< End DTO codegen section

#endif // _ZRPMESSAGEDTO_HPP_
