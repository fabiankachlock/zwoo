#ifndef _SWAGGER_COMPONENT_HPP_
#define _SWAGGER_COMPONENT_HPP_

#include "oatpp-swagger/Model.hpp"
#include "oatpp-swagger/Resources.hpp"
#include "oatpp/core/macro/component.hpp"

class SwaggerComponent {
public:
    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::swagger::DocumentInfo>, swaggerDocumentInfo)([] {
        oatpp::swagger::DocumentInfo::Builder builder;

        builder
        .setTitle("Zwoo API Reference")
        .setDescription("Zwoo API Reference.")
        .setVersion("0.0.1")
        .addServer("http://localhost:8000", "Server on localhost");

        return builder.build();
     }());

    OATPP_CREATE_COMPONENT(std::shared_ptr<oatpp::swagger::Resources>, swaggerResources)([] {
        // Make sure to specify correct full path to oatpp-swagger/res folder !!!
        return oatpp::swagger::Resources::loadResources(OATPP_SWAGGER_RES_PATH);
    }());
};

#endif // _SWAGGER_COMPONENT_HPP_