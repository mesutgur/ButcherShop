using ButcherShop.Business.Abstract;
using ButcherShop.Entity.Entities;
using ButcherShop.WebUI.Helpers;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContactMessageController : Controller
    {
        private readonly IContactMessageService _contactMessageService;

        public ContactMessageController(IContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        // GET: Admin/ContactMessage
        public ActionResult Index(string filter = "all")
        {
            var messages = _contactMessageService.GetAll(m => !m.IsDeleted);

            // Filtreleme
            switch (filter?.ToLower())
            {
                case "unread":
                    messages = messages.Where(m => !m.IsRead).ToList();
                    break;
                case "read":
                    messages = messages.Where(m => m.IsRead).ToList();
                    break;
                default: // all
                    break;
            }

            ViewBag.Filter = filter;
            ViewBag.UnreadCount = _contactMessageService.GetUnreadCount();

            return View(messages);
        }

        // GET: Admin/ContactMessage/Details/5
        public ActionResult Details(int id)
        {
            var message = _contactMessageService.GetById(id);

            if (message == null || message.IsDeleted)
            {
                TempData["Error"] = "Mesaj bulunamadı!";
                return RedirectToAction("Index");
            }

            // Okunmadı ise okundu olarak işaretle
            if (!message.IsRead)
            {
                _contactMessageService.MarkAsRead(id);
            }

            return View(message);
        }

        // POST: Admin/ContactMessage/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int id, string replyMessage, string adminNote)
        {
            try
            {
                var message = _contactMessageService.GetById(id);

                if (message == null || message.IsDeleted)
                {
                    TempData["Error"] = "Mesaj bulunamadı!";
                    return RedirectToAction("Index");
                }

                // Admin notu varsa kaydet
                if (!string.IsNullOrEmpty(adminNote))
                {
                    message.AdminNote = adminNote;
                    message.ModifiedDate = DateTime.Now;
                    _contactMessageService.Update(message);
                }

                // Email yanıtı gönder
                if (!string.IsNullOrEmpty(replyMessage))
                {
                    bool emailSent = SendReplyEmail(message, replyMessage);

                    if (emailSent)
                    {
                        TempData["Success"] = "✅ Yanıtınız başarıyla gönderildi!";
                    }
                    else
                    {
                        TempData["Warning"] = "⚠️ Email gönderilemedi, lütfen tekrar deneyin.";
                    }
                }
                else
                {
                    TempData["Success"] = "✅ Admin notu kaydedildi!";
                }

                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Hata: {ex.Message}";
                return RedirectToAction("Details", new { id = id });
            }
        }

        // POST: Admin/ContactMessage/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _contactMessageService.Delete(id);
                TempData["Success"] = "✅ Mesaj başarıyla silindi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Hata: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // POST: Admin/ContactMessage/MarkAsRead
        [HttpPost]
        public JsonResult MarkAsRead(int id)
        {
            try
            {
                _contactMessageService.MarkAsRead(id);
                return Json(new { success = true, message = "Okundu olarak işaretlendi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/ContactMessage/MarkAsUnread
        [HttpPost]
        public JsonResult MarkAsUnread(int id)
        {
            try
            {
                var message = _contactMessageService.GetById(id);
                if (message != null)
                {
                    message.IsRead = false;
                    message.ReadDate = null;
                    message.ModifiedDate = DateTime.Now;
                    _contactMessageService.Update(message);
                    return Json(new { success = true, message = "Okunmadı olarak işaretlendi" });
                }
                return Json(new { success = false, message = "Mesaj bulunamadı" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #region Helper Methods

        private bool SendReplyEmail(ContactMessage message, string replyMessage)
        {
            try
            {
                var fromAddress = new MailAddress(
                    ConfigurationManager.AppSettings["EmailFrom"] ?? "noreply@kasapdukkan.com",
                    "Kasap Dükkanı"
                );

                var toAddress = new MailAddress(message.Email, message.Name);

                string subject = $"Re: {message.Subject ?? "İletişim Formu Mesajınız"}";
                string body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background: #dc3545; color: white; padding: 20px; text-align: center; }}
                            .content {{ background: #f8f9fa; padding: 20px; margin-top: 20px; }}
                            .reply {{ background: white; padding: 15px; border-left: 4px solid #dc3545; margin: 20px 0; }}
                            .original {{ background: #e9ecef; padding: 15px; margin: 20px 0; }}
                            .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h2>Kasap Dükkanı</h2>
                                <p>Mesajınıza Yanıt</p>
                            </div>
                            <div class='content'>
                                <p>Sayın <strong>{message.Name}</strong>,</p>
                                
                                <div class='reply'>
                                    <h3>Yanıtımız:</h3>
                                    {replyMessage.Replace("\n", "<br>")}
                                </div>

                                <div class='original'>
                                    <h4>Orijinal Mesajınız:</h4>
                                    <p><strong>Konu:</strong> {message.Subject}</p>
                                    <p><strong>Mesaj:</strong><br>{message.Message.Replace("\n", "<br>")}</p>
                                    <p><small><strong>Gönderim Tarihi:</strong> {message.CreatedDate:dd.MM.yyyy HH:mm}</small></p>
                                </div>
                            </div>
                            <div class='footer'>
                                <p>Bu mail otomatik olarak gönderilmiştir.</p>
                                <p>Daha fazla bilgi için: info@kasapdukkan.com</p>
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
                        Subject = subject,
                        Body = body,
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

        #endregion
    }
}