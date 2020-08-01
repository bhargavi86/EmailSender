using EmailSender.Models;
using EmailSender.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmailSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]   
        [Route("SendEmail")]
        public bool SendEmail([FromBody] Email mailInput)
        {
            return _emailService.SendEmail(mailInput.body, mailInput.subject, mailInput.toAddresses, mailInput.attachmentFilePaths, mailInput.deleteFiles);
        }

        [HttpPost]
        [Route("SendEmailAsync")]
        public async void SendEmailAsync([FromBody] Email mailInput)
        {
            await _emailService.SendEmailAsync(mailInput.body, mailInput.subject, mailInput.toAddresses, mailInput.attachmentFilePaths);
        }
    }
}
