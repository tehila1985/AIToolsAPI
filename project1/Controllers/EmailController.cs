using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IConfiguration configuration, ILogger<EmailController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("contact")]
        public async Task<IActionResult> SendContactEmail([FromBody] ContactFormDto contactForm)
        {
            try
            {
                // Email configuration - Add these to appsettings.json
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var adminEmail = "rivka7905@gmail.com";

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPassword);

                    // Email to admin
                    var adminMessage = new MailMessage
                    {
                        From = new MailAddress(smtpUser),
                        Subject = $"פנייה חדשה מ-{contactForm.Name}",
                        Body = $@"
                            <div dir='rtl' style='font-family: Arial, sans-serif;'>
                                <h2>פנייה חדשה מאתר CozyGen</h2>
                                <p><strong>שם:</strong> {contactForm.Name}</p>
                                <p><strong>אימייל:</strong> {contactForm.Email}</p>
                                <p><strong>טלפון:</strong> {contactForm.Phone}</p>
                                <p><strong>הודעה:</strong></p>
                                <p>{contactForm.Message}</p>
                            </div>
                        ",
                        IsBodyHtml = true
                    };
                    adminMessage.To.Add(adminEmail);

                    await client.SendMailAsync(adminMessage);

                    // Confirmation email to customer
                    var customerMessage = new MailMessage
                    {
                        From = new MailAddress(smtpUser),
                        Subject = "פנייתך התקבלה בהצלחה - CozyGen",
                        Body = $@"
                            <div dir='rtl' style='font-family: Arial, sans-serif;'>
                                <h2>שלום {contactForm.Name},</h2>
                                <p>תודה שפנית אלינו!</p>
                                <p>פנייתך התקבלה בהצלחה ונחזור אליך בהקדם האפשרי.</p>
                                <br>
                                <p><strong>פרטי הפנייה:</strong></p>
                                <p>{contactForm.Message}</p>
                                <br>
                                <p>בברכה,<br>צוות CozyGen</p>
                            </div>
                        ",
                        IsBodyHtml = true
                    };
                    customerMessage.To.Add(contactForm.Email);

                    await client.SendMailAsync(customerMessage);
                }

                _logger.LogInformation($"Contact email sent from {contactForm.Email}");
                return Ok(new { message = "Emails sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                return StatusCode(500, new { message = "Failed to send email" });
            }
        }
    }

    public class ContactFormDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
    }
}
