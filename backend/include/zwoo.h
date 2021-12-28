#ifndef _ZWOO_H_
#define _ZWOO_H_

#define SMTP_HOST "smtp.gmail.com"
#define SMTP_PORT 465

#define ZWOO_USERNAME std::getenv("ZWOO_USERNAME")
#define ZWOO_EMAIL std::getenv("ZWOO_EMAIL")
#define ZWOO_PASSWORD std::getenv("ZWOO_PASSWORD")

#define DOMAIN std::getenv("ZWOO_DOMAIN")
#define PORT std::getenv("ZWOO_PORT")

#define SITESECRET std::getenv("ZWOO_SITESECRET")

#include <iostream>
#include <stdio.h>
#include <vector>
#include <assert.h>
#include <string>
#include <chrono>

#endif