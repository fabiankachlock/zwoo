#ifndef _HTTP_SERVER_H_
#define _HTTP_SERVER_H_

#include "served/multiplexer.hpp"
#include "served/uri.hpp"
#include "served/net/server.hpp"

#include "SimpleJSON/json.hpp"

namespace Backend
{
    // Authentication Callbacks
    constexpr char *kCreateAccount = "api/authentication/create";
    constexpr char *kVerifyAccount = "api/authentication/verify";
    constexpr char *kLoginAccount = "api/authentication/login";

    constexpr char *kHelloWorld = "hello-world";

    constexpr char kIpAddress[] = "0.0.0.0";
    constexpr char kPort[] = "5000";
    constexpr char kThreads = 10;

    class HttpServer
    {
    private:
        served::multiplexer multiplexer;

    public:
        HttpServer(served::multiplexer m) : multiplexer(m) {}

        auto HelloWorld()
        {
            return [&](served::response &response, const served::request &request)
            {
                response << "{ \"message\": \"Hello World!\" }";
                // return if instert was successesful or not
                return served::response::stock_reply(200, response);
            };
        }

        void InitEndpoints()
        {
            multiplexer.handle(kHelloWorld).get(HelloWorld());
        }

        void StartServer()
        {
            served::net::server server("0.0.0.0", "5000", multiplexer);
            std::cout << "Starting Server on Port " << kPort << " ...\n";
            server.run(10);
        }
    };
} // namespace Backend

#endif