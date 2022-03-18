#ifndef _ZRPMESSAGEDTO_HPP_
#define _ZRPMESSAGEDTO_HPP_

#include "oatpp/core/Types.hpp"
#include "oatpp/core/macro/codegen.hpp"

#include OATPP_CODEGEN_BEGIN(DTO) ///< Begin DTO codegen section

class User : public oatpp::DTO {

  DTO_INIT(User, DTO /* extends */)

  DTO_FIELD(String, name);
  DTO_FIELD(Int32, age);

};

#include OATPP_CODEGEN_END(DTO) ///< End DTO codegen section

#endif // _ZRPMESSAGEDTO_HPP_
