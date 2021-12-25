#ifndef _ZWOO_H_
#define _ZWOO_H_

#define SMTP_HOST "smtp.gmail.com"
#define SMTP_PORT 465

#define ZWOO_USERNAME std::getenv("ZWOO_USERNAME")
#define ZWOO_EMAIL std::getenv("ZWOO_EMAIL")
#define ZWOO_PASSWORD std::getenv("ZWOO_PASSWORD")

#define DOMAIN "0.0.0.0"
#define PORT 8000

#define SITESECRET "<sitesecret here>"

#include <iostream>
#include <stdio.h>
#include <vector>
#include <assert.h>
#include <string>
#include <chrono>

#endif