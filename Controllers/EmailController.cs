using fotofolioAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace fotofolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmailController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            try
            {
                var smtpClient = new SmtpClient(_config["Smtp:Host"])
                {
                    Port = int.Parse(_config["Smtp:Port"]),
                    Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["Smtp:From"]),
                    Subject = $"New message from {request.FullName}",
                    Body = $"From: {request.FullName} ({request.Email})\n\nMessage:\n{request.Message}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(_config["Smtp:To"]);

                await smtpClient.SendMailAsync(mailMessage);

                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }
    }

}
