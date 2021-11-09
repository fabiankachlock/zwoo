#include <iostream>
#include "sqlite/sqlite3.h"

int main()
{
    sqlite3* con = NULL;
    con = (sqlite3*)sqlite3_open("./test.db", &con);

    return 0;
}