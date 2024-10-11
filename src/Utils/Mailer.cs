using System.Net;
using System.Net.Mail;
using DotNetEnv;

namespace BlogApp.Utils
{
    public class Mailer
    {
        private readonly string _mailHost = Env.GetString("MAIL_HOST");
        private readonly string _mailUser = Env.GetString("MAIL_USER");
        private readonly string _mailPass = Env.GetString("MAIL_PASS");

        public async Task SendEmailAsync(string toEmail, string otp)
        {
            using var smtpClient = new SmtpClient(_mailHost)
            {
                Port = 587, // Use port 587 for TLS
                Credentials = new NetworkCredential(_mailUser, _mailPass),
                EnableSsl = true, // Enable SSL
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_mailUser),
                Subject = "OTP Verification Blog App",
                Body = MailBody(otp),
                IsBodyHtml = true, // Set to true if body contains HTML
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public static string MailBody(string otp)
        {
            return @"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""UTF-8"">
                    <title>OTP Verification Email</title>
                    <style>
                        body {
                            background-color: #ffffff;
                            font-family: Arial, sans-serif;
                            font-size: 16px;
                            line-height: 1.4;
                            color: #333333;
                            margin: 0;
                            padding: 0;
                        }
                        .container {
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            text-align: center;
                        }
                        .logo {
                            margin-bottom: 20px;
                            color: white;
                            background-image: linear-gradient(to right, #68217A , #7A2176);
                            max-width: max-content;
                            margin-left: auto;
                            margin-right: auto;
                            border-radius: 10px;
                            padding: 10px;
                        }
                        .message {
                            font-size: 18px;
                            font-weight: bold;
                            margin-bottom: 20px;
                        }
                        .body {
                            font-size: 16px;
                            margin-bottom: 20px;
                        }
                        .highlight {
                            font-weight: bold;
                            font-size: 24px; /* Optional: Adjust font size as needed */
                        }
                        .support {
                            font-size: 14px;
                            color: #999999;
                            margin-top: 20px;
                        }
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <h1 class=""logo"">Blog-App</h1>
                        <div class=""message"">OTP Verification Email</div>
                        <div class=""body"">
                            <p>Dear User,</p>
                            <p>Thank you for registering with Blog App. To complete your registration, please use the following OTP (One-Time Password) to verify your account:</p>
                            <h2 class=""highlight"">"
                + otp
                + @"</h2>
                            <p>This OTP is valid for 5 minutes. If you did not request this verification, please disregard this email. Once your account is verified, you will have access to our platform and its features.</p>
                        </div>
                        <div class=""support"">If you have any questions or need assistance, please feel free to reach out to us at Blog-App.com. We are here to help!</div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}
