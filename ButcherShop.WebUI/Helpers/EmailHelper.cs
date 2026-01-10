using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ButcherShop.WebUI.Helpers
{
    public static class EmailHelper
    {
        public static bool SendContactEmail(string name, string email, string phone, string subject, string message)
        {
            try
            {
                var fromAddress = new MailAddress(
                    ConfigurationManager.AppSettings["EmailFrom"] ?? "noreply@kasapdukkan.com",
                    "Kasap Dükkanı Website"
                );

                var toAddress = new MailAddress(
                    ConfigurationManager.AppSettings["EmailTo"] ?? "info@kasapdukkan.com",
                    "Kasap Dükkanı"
                );

                string emailSubject = $"İletişim Formu - {subject}";
                string emailBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background: #dc3545; color: white; padding: 20px; text-align: center; }}
                            .content {{ background: #f8f9fa; padding: 20px; margin-top: 20px; }}
                            .field {{ margin-bottom: 15px; }}
                            .label {{ font-weight: bold; color: #dc3545; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h2>📧 Yeni İletişim Mesajı</h2>
                            </div>
                            <div class='content'>
                                <div class='field'>
                                    <span class='label'>👤 Ad Soyad:</span><br>{name}
                                </div>
                                <div class='field'>
                                    <span class='label'>📧 E-posta:</span><br>
                                    <a href='mailto:{email}'>{email}</a>
                                </div>
                                <div class='field'>
                                    <span class='label'>📞 Telefon:</span><br>
                                    {(string.IsNullOrEmpty(phone) ? "-" : phone)}
                                </div>
                                <div class='field'>
                                    <span class='label'>📋 Konu:</span><br>
                                    {(string.IsNullOrEmpty(subject) ? "-" : subject)}
                                </div>
                                <div class='field'>
                                    <span class='label'>💬 Mesaj:</span><br>
                                    {message.Replace("\n", "<br>")}
                                </div>
                                <hr>
                                <p><small>Bu mesaj {DateTime.Now:dd.MM.yyyy HH:mm} tarihinde gönderildi.</small></p>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(
                        fromAddress.Address,
                        ConfigurationManager.AppSettings["EmailPassword"]
                    );
                    smtp.Timeout = 20000;

                    using (var mailMessage = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = emailSubject,
                        Body = emailBody,
                        IsBodyHtml = true
                    })
                    {
                        smtp.Send(mailMessage);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log error in production with proper logging framework
                return false;
            }
        }
    }
}