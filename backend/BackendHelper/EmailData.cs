using System.Net;
using System.Net.Mail;

namespace BackendHelper;

public class EmailData
{
    public string Username;
    public UInt64 Puid;
    public string Code;
    public string Email;

    public EmailData(string username, UInt64 puid, string code, string email)
    {
        this.Username = username;
        this.Puid = puid;
        this.Code = code;
        this.Email = email;
    }
    
    public static void SendMail(EmailData data)
    {
        string link = $"{(Convert.ToBoolean(Environment.GetEnvironmentVariable("USE_SSL")) ? "https://" : "http://")}{Environment.GetEnvironmentVariable("ZWOO_DOMAIN")}/auth/verify?id={data.Puid}&code={data.Code}";
        string text = $"\r\nHello {data.Username},\r\nplease click the link to verify your zwoo account.\r\n{link}\r\n\r\nThe confirmation expires with the end of this day\r\n(UTC + 01:00).\r\n\r\nIf you've got this E-Mail by accident or don't want to\r\nregister, please ignore it.\r\n\r\nⒸ ZWOO 2022\r\n";
        string html =
            "<!DOCTYPE html>" +
            "<html lang=\"en\" " +
            "xmlns:o=\"urn:schemas-microsoft-com:office:office\" " +
            "xmlns:v=\"urn:schemas-microsoft-com:vml\"><head><title></" +
            "title><meta content=\"text/html; charset=utf-8\" " +
            "http-equiv=\"Content-Type\"/><meta content=\"width=device-width, " +
            "initial-scale=1.0\" name=\"viewport\"/><!--[if " +
            "mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</" +
            "o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></" +
            "xml><![endif]-->" +
            "<style>*{box-sizing:border-box;}body{margin:0;padding:0;}a[x-" +
            "apple-data-detectors]{color:inherit!important;text-decoration:" +
            "inherit!important;}#MessageViewBody " +
            "a{color:inherit;text-decoration:none;}p{line-height:inherit}." +
            "desktop_hide,.desktop_hide " + 
            "table{mso-hide:all;display:none;max-height:0px;overflow:hidden;}@" +
            "media(max-width:520px){.desktop_hide " +
            "table.icons-inner{display:inline-block!important;}.icons-inner{" +
            "text-align:center;}.icons-inner td{margin:0 " +
            "auto;}.row-content{width:100%!important;}.column " +
            ".border,.mobile_hide{display:none;}table{table-layout:fixed!" +
            "important;}.stack " +
            ".column{width:100%;display:block;}.mobile_hide{min-height:0;max-" +
            "height:0;max-width:0;overflow:hidden;font-size:0px;}.desktop_hide," +
            ".desktop_hide table " +
            "{display:table!important;max-height:none!important;}}</style>" +
            "</head><body " +
            "style=\"background-color:#2d2e3b;margin:0;padding:0;-webkit-text-" +
            "size-adjust:none;text-size-adjust:none;\"><table border=\"0\" " +
            "cellpadding=\"0\" cellspacing=\"0\" class=\"nl-container\" " +
            "role=\"presentation\" style=\"mso-table-lspace: 0pt; " +
            "mso-table-rspace: 0pt; background-color: #2d2e3b;\" " +
            "width=\"100%\"><tbody><tr><td><table align=\"center\" " +
            "border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"row " +
            "row-1\" role=\"presentation\" " +
            "style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;\" "+
            "width=\"100%\"><tbody><tr><td><table align=\"center\" "+
            "border=\"0\" cellpadding=\"0\" cellspacing=\"0\" "+
            "class=\"row-content stack\" role=\"presentation\" "+
            "style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;color:#000000;"+
            "width:500px;\" width=\"500\">"+
            "<tbody><tr><td class=\"column column-1\" "+
            "style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;font-weight:400;"+
            "text-align:left;vertical-align:top;padding-top:5px;padding-bottom:"+
            "5px;border-top:0px;border-right:0px;border-bottom:0px;border-left:"+
            "0px;\" width=\"100%\"><table border=\"0\" cellpadding=\"0\" "+
            "cellspacing=\"0\" class=\"heading_block\" role=\"presentation\" "+
            "style=\"mso-table-lspace: 0pt;mso-table-rspace:0pt;\" "+
            "width=\"100%\"><tr><td "+
            "style=\"width:100%;text-align:center;\"><h1 "+
            "style=\"margin:0;color:#ebebeb;font-size:30px;font-family:Arial,"+
            "Helvetica "+
            "Neue,Helvetica,sans-serif;line-height:120%;text-align:center;"+
            "direction:ltr;font-weight:700;letter-spacing:normal;margin-top:0;"+
            "margin-bottom:0;\"><span "+
            "class=\"tinyMce-placeholder\">ZWOO</span></h1>"+
            "</td></tr></table><table border=\"0\" cellpadding=\"0\" "+
            "cellspacing=\"0\" class=\"icons_block\" role=\"presentation\" "+
            "style=\"mso-table-lspace: 0pt;mso-table-rspace:0pt;\" "+
            "width=\"100%\"><tr><td style=\"vertical-align: middle; color: "+
            "#000000; text-align: center; font-family: inherit; font-size: "+
            "14px;\"><table align=\"center\" cellpadding=\"0\" "+
            "cellspacing=\"0\" role=\"presentation\" "+
            "style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;\"><tr><td "+
            "style=\"vertical-align:middle;text-align:center;padding-top:15px;"+
            "padding-bottom:15px;padding-left:15px;padding-right:15px;\"><img "+
            "align=\"center\" alt=\"\" class=\"icon\" height=\"64\" src=\""+
            "https://zwoo-ui.web.app/img/logo/zwoo_logo_none.svg"+
            "\" style=\"display:block;height:auto;margin:0 auto;border:0;\" "+
            "width=\"64\"/></td>"+
            "</tr></table></td></tr></table><table border=\"0\" "+
            "cellpadding=\"0\" cellspacing=\"0\" class=\"heading_block\" "+
            "role=\"presentation\" "+
            "style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;\" "+
            "width=\"100%\"><tr><td "+
            "style=\"width:100%;text-align:center;\"><h2 "+
            "style=\"margin:0;color:#ebebeb;font-size:24px;font-family:Arial, "+
            "Helvetica "+
            "Neue,Helvetica,sans-serif;line-height:120%;text-align:center;"+
            "direction:ltr;font-weight:700;letter-spacing:normal;margin-top:0;"+
            "margin-bottom:0;\"><span class=\"tinyMce-placeholder\">the second "+
            "challenge</span></h2></td></tr></table><table border=\"0\" "+
            "cellpadding=\"0\" cellspacing=\"0\" class=\"heading_block\" "+
            "role=\"presentation\" "+
            "style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;\" "+
            "width=\"100%\"><tr><td "+
            "style=\"width:100%;text-align:center;padding-top:20px;\"><h3 "+
            "style=\"margin:0;color:#ebebeb;font-size:16px;font-family:Arial,"+
            "Helvetica "+
            "Neue,Helvetica,sans-serif;line-height:120%;text-align:left;"+
            "direction:ltr;font-weight:700;letter-spacing:normal;margin-top:0;"+
            "margin-bottom:0;\">Hello " +
            data.Username +
            ",</h3></td></tr></table><table border=\"0\" cellpadding=\"10\" "+
            "cellspacing=\"0\" class=\"paragraph_block\" role=\"presentation\" "+
            "style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;word-break:"+
            "break-word;\" width=\"100%\"><tr><td><div "+
            "style=\"color:#ebebeb;font-size:14px;font-family:Arial, Helvetica "+
            "Neue, Helvetica, "+
            "sans-serif;font-weight:400;line-height:120%;text-align:left;"+
            "direction:ltr;letter-spacing:0px;\"><p style=\"margin: "+
            "0;\">please verify your account via the <strong>button</strong> "+
            "<br/>or press <strong>the "+
            "link.</strong></p></div></td></tr></table><table border=\"0\" "+
            "cellpadding=\"10\" cellspacing=\"0\" class=\"button_block\" "+
            "role=\"presentation\" style=\"mso-table-lspace: 0pt; "+
            "mso-table-rspace: 0pt;\" width=\"100%\"><tr><td><div "+
            "align=\"center\">"+
            "<!--[if mso]><v:roundrect "+
            "xmlns:v=\"urn:schemas-microsoft-com:vml\" "+
            "xmlns:w=\"urn:schemas-microsoft-com:office:word\" href=\"" +
            link +
            " style=\"height:42px;width:78px;v-text-anchor:middle;\" arcsize=\"10%\" stroke=\"false\" fillcolor=\"#3AAEE0\"><w:anchorlock/><v:textbox inset=\"0px,0px,0px,0px\"><center style=\"color:#ffffff; font-family:Arial, sans-serif; font-size:16px\"><![endif]--><a href=\"" +
            link +
            "\" " +
            "style=\"text-decoration:none;display:inline-block;color:#ffffff;" +
            "background-color:#3AAEE0;border-radius:4px;width:auto;border-top:" +
            "1px solid #3AAEE0;font-weight:400;border-right:1px solid " +
            "#3AAEE0;border-bottom:1px solid #3AAEE0;border-left:1px solid " +
            "#3AAEE0;padding-top:5px;padding-bottom:5px;font-family:Arial, " +
            "Helvetica Neue, Helvetica, " +
            "sans-serif;text-align:center;mso-border-alt:none;word-break:keep-" +
            "all;\" target=\"_blank\"><span "+
            "style=\"padding-left:20px;padding-right:20px;font-size:16px;"+
            "display:inline-block;letter-spacing:normal;\"><span "+
            "style=\"font-size:16px;line-height:2;word-break:break-word;mso-"+
            "line-height-alt:32px;\">verify</span></span></a><!--[if "+
            "mso]></center></v:textbox></v:roundrect><![endif]-->"+
            "</div></td></tr></table><table border=\"0\" cellpadding=\"10\" "+
            "cellspacing=\"0\" class=\"paragraph_block\" role=\"presentation\" "+
            "style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;word-break:"+
            "break-word;\" width=\"100%\"><tr><td><div "+
            "style=\"color:#ebebeb;font-size:14px;font-family:Arial, Helvetica "+
            "Neue, Helvetica, "+
            "sans-serif;font-weight:400;line-height:120%;text-align:left;"+
            "direction:ltr;letter-spacing:0px;\"><p "+
            "style=\"margin:0;margin-bottom:16px;\">" +
            link +
            "</p><p style=\"margin:0;margin-bottom:16px;\">The confirmation "+
            "expires with the end of this day (UTC + 01:00).<br/>If you've got "+
            "this E-Mail by accident or don't want to register, please ignore "+
            "it.</p><p style=\"margin:0;margin-bottom:16px;\"></p><p "+
            "style=\"margin:0;\">Ⓒ ZWOO "+
            "2022</p></div></td></tr></table></td></tr></tbody></table></td></"+
            "tr></tbody></table></td></tr></tbody></table><!-- End "+
            "--></body></html>\r\n";

        var mail = new MailMessage();
        mail.From = new MailAddress(Environment.GetEnvironmentVariable("SMTP_HOST_EMAIL"));
        mail.To.Add(new MailAddress(data.Email));

        mail.Subject = "Verify your ZWOO Account";

        var plain = AlternateView.CreateAlternateViewFromString(text, null, "text/plain");
        var htmlview = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
        mail.AlternateViews.Add(plain);
        mail.AlternateViews.Add(htmlview);

        var smtp = new SmtpClient(Environment.GetEnvironmentVariable("SMTP_HOST_URL"), Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_HOST_PORT")));
        smtp.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("SMTP_USERNAME"), Environment.GetEnvironmentVariable("SMTP_PASSWORD"));
        smtp.Send(mail);
    }
}