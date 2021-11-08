#include <iostream>
#include "../lib/sqlite/sqlite3.h"

int main()
{
    sqlite3* con = NULL;
    sqlite3_open("../test.db", &con);

    return 0;
}