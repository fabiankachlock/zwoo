#include "Server/controller/Authentication/Email.h"

#include "zwoo.h"
#include <curl/curl.h>

#include <array>

void send_verification_mail( Email email, std::shared_ptr<Logger> logger )
{
    static std::array<std::string, 5> headers_text = {
        "Date: Tue, 22 Aug 2017 14:08:43 +0100", "To: " + email.email,
        "From: " + SMTP_HOST_EMAIL, "Subject: Verify your ZWOO Account", "" };
    static const char null = '\0';

    static std::string text =
        "\r\nHello {0},\r\nplease click the link to verify your zwoo "
        "account.\r\n{1}\r\n\r\nThe confirmation expires with the end of this "
        "day\r\n(UTC + 01:00).\r\n\r\nIf you've got this E-Mail by accident or "
        "don't want to\r\nregister, please ignore it.\r\n\r\nⒸ ZWOO 2022\r\n";
    static std::string html =
        "\r\n<html style=\"background-color: #363847; font-size: 16px; "
        "font-family: sans-serif; width: 100%; height: 100%;\">\r\n"
        "<head><meta charset=\"utf-8\"><title>ZWOO "
        "VERFICATION</title></head>\r\n"
        "<body style=\"margin-top: 7%\">\r\n"
        "<h1 style=\"color: #3066BE; text-align: center; margin-bottom: 0; "
        "margin-top: 1%; font-size: 450%\">ZWOO</h1>\r\n"
        "<p style=\"color: #3066BE; text-align: center; margin-top: 0; "
        "font-size: 280%\">the second challenge</p>\r\n)"
        "<p style=\"color: white; text-align: left; margin-top: 7%;  "
        "font-size: 250%;  margin-left: 10%\"> Dear {0},</br>please verify "
        "your account via the <b>button</b> </br> or press <b>the link</b> "
        "below:</p>\r\n"
        "<div style=\"display: flex; justify-content: center; margin-top: "
        "5%\"><a href=\"{1}\" target=\"_blank\"> <button style=\"background: "
        "#3066BE; border: 0;color: white; text-align: center; font-size: 250%; "
        "padding: 0.5em 1em; border-radius: "
        "0.35em;\">verify</button></a></div>\r\n"
        "<div> <a href=\"{1}\" target=\"_blank\"><p style=\"color: white; "
        "text-align: left; margin-top: 7%;  font-size: 180%;  margin-left: "
        "10%\">{1}</p>\r\n"
        "</a><p style=\"color: white; text-align: left; margin-top: 13%;  "
        "font-size: 200%;  margin-left: 10%\"> The confirmation expires with "
        "the end of the day</br>(UTC + 01:00). </p> <p style=\"color: white; "
        "text-align: left; margin-top: 7%;  font-size: 200%;  margin-left: "
        "10%\"> If you've got this E-Mail by accident or don't want to</br> "
        "register, please ignore it. </p>\r\n"
        "</div><div style=\"color: white; text-align: left; margin-top: 18%;  "
        "font-size: 150%;  margin-left: 10%\"> &#x24B8; ZWOO "
        "2022</div></body></html>\r\n";

    CURL *curl;
    CURLcode res = CURLE_OK;

    curl = curl_easy_init( );
    if ( curl )
    {
        std::string link = ( USE_SSL ? "https://" : "http://" ) + ZWOO_DOMAIN +
                           "/auth/verify?id=" + std::to_string( email.puid ) +
                           "&code=" + email.code;

        struct curl_slist *headers = NULL;
        struct curl_slist *recipients = NULL;
        struct curl_slist *slist = NULL;
        curl_mime *mime;
        curl_mime *alt;
        curl_mimepart *part;

        curl_easy_setopt( curl, CURLOPT_URL, SMTP_HOST_URL.c_str( ) );
        curl_easy_setopt( curl, CURLOPT_MAIL_FROM, SMTP_HOST_EMAIL.c_str( ) );
        curl_easy_setopt( curl, CURLOPT_USE_SSL, 1L );
        curl_easy_setopt( curl, CURLOPT_USERNAME, SMTP_HOST_EMAIL.c_str( ) );
        curl_easy_setopt( curl, CURLOPT_PASSWORD, SMTP_PASSWORD.c_str( ) );
        curl_easy_setopt( curl, CURLOPT_HTTPAUTH, (long)CURLAUTH_ANY );
        curl_easy_setopt( curl, CURLOPT_VERBOSE, 1L );

        recipients = curl_slist_append( recipients, email.email.c_str( ) );
        curl_easy_setopt( curl, CURLOPT_MAIL_RCPT, recipients );

        std::for_each( headers_text.begin( ), headers_text.end( ),
                       [ & ]( auto h )
                       {
                           if ( h != "" )
                               headers =
                                   curl_slist_append( headers, h.c_str( ) );
                           else
                               headers = curl_slist_append( headers, &null );
                       } );
        curl_easy_setopt( curl, CURLOPT_HTTPHEADER, headers );

        mime = curl_mime_init( curl );
        alt = curl_mime_init( curl );

        /* HTML message. */
        part = curl_mime_addpart( alt );
        curl_mime_data( part,
                        fmt::format( html, email.username, link ).c_str( ),
                        CURL_ZERO_TERMINATED );
        curl_mime_type( part, "text/html" );

        /* Text message. */
        part = curl_mime_addpart( alt );
        curl_mime_data( part,
                        fmt::format( text, email.username, link ).c_str( ),
                        CURL_ZERO_TERMINATED );

        /* Create the inline part. */
        part = curl_mime_addpart( mime );
        curl_mime_subparts( part, alt );
        curl_mime_type( part, "multipart/alternative" );
        slist = curl_slist_append( NULL, "Content-Disposition: inline" );
        curl_mime_headers( part, slist, 1 );

        /* Send the message */
        curl_easy_setopt( curl, CURLOPT_MIMEPOST, mime );
        res = curl_easy_perform( curl );

        /* Check for errors */
        if ( res != CURLE_OK )
            logger->log->error( "curl_easy_perform() failed: {}\n",
                                curl_easy_strerror( res ) );

        /* Free lists. */
        curl_slist_free_all( recipients );
        curl_slist_free_all( headers );
        curl_easy_cleanup( curl );
        curl_mime_free( mime );
    }
}