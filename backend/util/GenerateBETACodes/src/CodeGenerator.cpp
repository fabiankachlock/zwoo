#include "CodeGenerator.h"

bool contains( std::vector<std::string> v, std::string s );

auto generate_codes( int amount, std::string format )
    -> std::vector<std::string>
{
    auto codes = std::vector<std::string>( );
    codes.reserve( amount );

    for ( int i = 0; i < amount; i++ )
    {
        auto s = generate_code( format );
        int j = 0;
        while ( contains( codes, s ) && j < 100 )
        {
            s = generate_code( format );
            j++;
        }
        if ( j < 100 )
            codes.push_back( s );
        else
            break;
    }
    return codes;
}

bool contains( std::vector<std::string> v, std::string s )
{
    for ( auto str : v )
    {
        if ( str == s )
            return true;
    }
    return false;
}

static auto generate_code( std::string format ) -> std::string
{
    auto s = format;

    auto t = random_num_char( );

    for ( auto it = s.begin( ); it != s.end( ); ++it )
    {
        if ( *it == 'x' )
            s = s.replace( it, it + 1, random_char( ) );
        else if ( *it == 'n' )
            s = s.replace( it, it + 1, random_num_char( ) );
        else if ( *it == 'c' )
            s = s.replace( it, it + 1, random_text_char( true ) );
        else if ( *it == 'C' )
            s = s.replace( it, it + 1, random_text_char( false ) );
        else
            continue;
    }

    return s;
}

static auto random_char( ) -> std::string
{
    static const char alphanum[] = "0123456789"
                                   "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                                   "abcdefghijklmnopqrstuvwxyz";
    return std::string(
        1, alphanum[ random_number( 0, 512 ) % ( sizeof( alphanum ) - 1 ) ] );
}

static auto random_num_char( ) -> std::string
{
    return std::string( 1, (char)random_number( 0, 9 ) + 48 );
}

static auto random_text_char( bool all_chars = true ) -> std::string
{
    return std::string( 1, (char)( random_number( 0, 25 ) +
                                   ( 32 * random_number( 0, all_chars ) ) ) +
                               65 );
}

static auto random_number( int min, int max ) -> int
{
    auto r = std::mt19937(
        std::chrono::steady_clock::now( ).time_since_epoch( ).count( ) );
    std::uniform_int_distribution<> dis( min, max );
    return dis( r );
}