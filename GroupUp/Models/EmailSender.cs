using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
// ReSharper disable UseObjectOrCollectionInitializer

namespace GroupUp.Models
{
    public class EmailSender
    {
        public static bool Send(string email, int verificationCode)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            // setup Smtp authentication
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("noreply.groupup@gmail.com", "groupup123A+");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("noreply.groupup@gmail.com");
            msg.To.Add(new MailAddress(email));

            msg.Subject = "GroupUp - Your verification code";
            msg.IsBodyHtml = true;
            msg.Body = string.Format($"<html><head></head><body><b>Your verification code is: {verificationCode}</b></body>");

            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}