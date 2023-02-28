using Azure.Core;
using CiPlatformWeb.Entities.DataModels;
using CiPlatformWeb.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CiPlatformWeb.Repositories.Repository
{
    public class EmailGeneration : IEmailGeneration
    {
        private readonly ApplicationDbContext _db;
       

        public EmailGeneration (ApplicationDbContext db)
        {
            _db = db;
        }

        string IEmailGeneration.GenerateToken ()
        {
            Random random = new Random();

            int capitalCharCode = random.Next(65, 91);
            char randomCapitalChar = (char) capitalCharCode;


            int randomint = random.Next();


            int SmallcharCode = random.Next(97, 123);
            char randomChar = (char) SmallcharCode;

            String token = "";
            token += randomCapitalChar.ToString();
            token += randomint.ToString();
            token += randomChar.ToString();

            return token;
        }

        string IEmailGeneration.GenerateLink (User obj, string token)
        {
            var PasswordResetLink = Url.Action("ResetPassword", "Home", new { Email = obj.Email, Token = token }, Request.Scheme);
            return PasswordResetLink;
        }


        void IEmailGeneration.ResetPasswordAdd (User obj, string token)
        {
            var ResetPasswordInfo = new PasswordReset()
            {
                Email = obj.Email,
                Token = token
            };
            _db.Add(ResetPasswordInfo);
            _db.SaveChanges();
        }

        void IEmailGeneration.GenerateEmail (User obj, string token, string PasswordResetLink)
        {
            var fromEmail = new MailAddress("hemal04121@gmail.com");
            var toEmail = new MailAddress(obj.Email);
            var fromEmailPassword = "Pratham@2211";
            string subject = "Reset Password";
            string body = PasswordResetLink;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            MailMessage message = new MailMessage(fromEmail, toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            smtp.Send(message);
        }

  
    }
}
