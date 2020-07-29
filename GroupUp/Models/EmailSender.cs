using System;
using System.Net.Mail;
using System.Web.Mvc;

// ReSharper disable UseObjectOrCollectionInitializer

namespace GroupUp.Models
{
    public class EmailSender
    {
        public static bool SendVerificationCode(string emailAddress, int verificationCode)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;

                // Set up SMTP authentication
                System.Net.NetworkCredential credentials =
                    new System.Net.NetworkCredential("noreply.groupup@gmail.com", "groupup123A+");
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("noreply.groupup@gmail.com");
                msg.To.Add(new MailAddress(emailAddress));

                msg.Subject = "GroupUp - Your verification code";
                msg.IsBodyHtml = false;
                msg.Body = string.Format($"Your verification code is: {verificationCode}");

                try
                {
                    client.Send(msg);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool SendPasswordResetLink(string emailAddress, string callbackUrl)
        {
            // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
            // await UserManager.SendEmailAsync(user.Id, "Parola Sıfırlama", "Lütfen parolanızı sıfırlamak için <a href=\"" + callbackUrl + "\">buraya tıklayın</a>");
            // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            using (SmtpClient client = new SmtpClient())
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;

                // Set up SMTP authentication
                System.Net.NetworkCredential credentials =
                    new System.Net.NetworkCredential("noreply.groupup@gmail.com", "groupup123A+");
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("noreply.groupup@gmail.com");
                msg.To.Add(new MailAddress(emailAddress));

                UrlHelper urlHelper = new UrlHelper();
                msg.Subject = "GroupUp - Your Password Reset Link";
                msg.IsBodyHtml = true;
                msg.Body = "Please click <a href=\"" + callbackUrl + "\">here</a> to reset your password.";

                try
                {
                    client.Send(msg);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
    
}