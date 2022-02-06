#ifndef _ZWOO_H_
#define _ZWOO_H_

#include <string>
#include <algorithm>
#include <sstream>

std::string get_env(const char* env_var);
bool str2b(std::string str);
int str2int(std::string str);

#define ZWOO_DOMAIN get_env("ZWOO_DOMAIN")

#define SMTP_HOST_URL get_env("SMTP_HOST_URL")
#define SMTP_HOST_PORT str2int(get_env("SMTP_HOST_PORT"))
#define SMTP_HOST_EMAIL get_env("SMTP_HOST_EMAIL")
#define SMTP_USERNAME get_env("SMTP_USERNAME")
#define SMTP_PASSWORD get_env("SMTP_PASSWORD")

#define ZWOO_BACKEND_DOMAIN "0.0.0.0"
#define ZWOO_BACKEND_PORT 8000

#define ZWOO_DATABASE_CONNECTION_STRING get_env("ZWOO_DATABASE_CONNECTION_STRING")

#define ZWOO_RECAPTCHA_SIDESECRET get_env("ZWOO_RECAPTCHA_SIDESECRET")

#define USE_SSL str2b(get_env("USE_SSL"))
#define SSL_PEM get_env("SSL_PEM")
#define SSL_CERTIFICATE get_env("SSL_CERTIFICATE")

#endif
