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
using CiPlatformWeb.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
//using System.Security.Policy;

namespace CiPlatformWeb.Repositories.Repository
{
    public class EmailGeneration : IEmailGeneration
    {
        private readonly ApplicationDbContext _db;
       

        public EmailGeneration (ApplicationDbContext db)
        {
            _db = db;
        }  

        User IEmailGeneration.CheckUser (ForgotPasswordValidation obj)
        {
            return _db.Users.Where(e => e.Email == obj.Email).FirstOrDefault();

        }
        void IEmailGeneration.GenerateEmail (ForgotPasswordValidation obj)
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

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "localhost";
            uriBuilder.Port = 44395;
            uriBuilder.Path = "Home/ResetPassword";
            uriBuilder.Query = "token=" + token;

            var PasswordResetLink = uriBuilder.ToString();

            var ResetPasswordInfo = new PasswordReset()
            {
                Email = obj.Email,
                Token = token
            };
            _db.Add(ResetPasswordInfo);
            _db.SaveChanges();

            var fromEmail = new MailAddress("hemal04121@gmail.com");
            var toEmail = new MailAddress(obj.Email);
            var fromEmailPassword = "orwgqohhtojovpvr";
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
