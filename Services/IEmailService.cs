using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailSender.Services
{
    public interface IEmailService
    {
        public bool SendEmail(string body, string subject, List<string> toAddresses, List<string> attachmentFilePaths = null, bool deleteFilesAfterMail = false);

        public Task SendEmailAsync(string body, string subject, List<string> toAddresses, List<string> attachmentFilePaths = null);
    }
}
