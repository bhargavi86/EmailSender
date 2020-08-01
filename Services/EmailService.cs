using EmailSender.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailSender.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IOptions<EmailSettings> _settings;

        public EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> settings) 
        {
            _logger = logger;
            _settings = settings;
        }

        public bool SendEmail(string body, string subject, List<string> toAddresses, List<string> attachmentFilePaths = null, bool deleteFilesAfterMail = false)
        {
            var isEmailSent = true;
            var smtpClient = _settings.Value.SMTPClient;
            var fromAddress = _settings.Value.FromAddress;
            var port = _settings.Value.SMTPPort;
            var password = _settings.Value.Password;

            try
            {
                switch (smtpClient)
                {
                    case "smtp.sendgrid.net":
                        List<EmailAddress> addressList = new List<EmailAddress>();
                        List<SendGrid.Helpers.Mail.Attachment> attachmentList = new List<SendGrid.Helpers.Mail.Attachment>();

                        var apiKey = password;
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(fromAddress);

                        foreach (string s in toAddresses)
                        {
                            addressList.Add(new EmailAddress(s));
                        }

                        var htmlBody = body;

                        if (attachmentFilePaths != null)
                        {
                            foreach (string path in attachmentFilePaths)
                            {
                                if (File.Exists(path))
                                {
                                    SendGrid.Helpers.Mail.Attachment attachment = new SendGrid.Helpers.Mail.Attachment();
                                    attachment.Filename = Path.GetFileName(path);
                                    var extension = Path.GetExtension(path);

                                    switch (extension)
                                    {
                                        case ".xlsx":
                                            attachment.Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                            break;
                                        case ".xls":
                                            attachment.Type = "application/vnd.ms-excel";
                                            break;
                                        case ".pdf":
                                            attachment.Type = "application/pdf";
                                            break;
                                    }
                                    attachment.Disposition = "attachment";
                                    attachment.Content = Convert.ToBase64String(File.ReadAllBytes(path));
                                    attachmentList.Add(attachment);
                                }
                            }
                        }

                        var message = MailHelper.CreateSingleEmailToMultipleRecipients(from, addressList, subject, "", htmlBody);
                        if (attachmentList.Count > 0)
                        {
                            message.AddAttachments(attachmentList);
                        }

                        Task.Run(() => client.SendEmailAsync(message)).Wait();

                        if (deleteFilesAfterMail)
                        {
                            foreach (string path in attachmentFilePaths)
                            {
                                File.Delete(path);
                            }
                        }
                        break;
                    case "smtp.office365.com":
                    case "smtp.ionos.com":
                    case "smtp.gmail.com":
                    default:
                        MailMessage mail = new MailMessage();                       
                        mail.From = new MailAddress(fromAddress);

                        foreach (string s in toAddresses)
                        {
                            mail.To.Add(s);
                        }

                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;

                        if (attachmentFilePaths != null)
                        {
                            foreach (string filePath in attachmentFilePaths)
                            {
                                System.Net.Mail.Attachment attachment;
                                attachment = new System.Net.Mail.Attachment(filePath);
                                mail.Attachments.Add(attachment);
                            }
                        }

                        SmtpClient mailClient = new SmtpClient(smtpClient);
                        mailClient.Port = Convert.ToInt32(port);
                        mailClient.Credentials = new NetworkCredential(fromAddress, password);
                        mailClient.EnableSsl = true;

                        mailClient.Send(mail);

                        if (deleteFilesAfterMail)
                        {
                            mail.Attachments.Dispose();
                            mailClient.Dispose();

                            foreach (string filePath in attachmentFilePaths)
                            {
                                File.Delete(filePath);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email of type: {smtpClient}", smtpClient);
                isEmailSent = false;
            }

            return isEmailSent;
        }

        public async Task SendEmailAsync(string body, string subject, List<string> toAddresses, List<string> attachmentFiles = null)
        {
            var smtpClient = _settings.Value.SMTPClient;
            var fromAddress = _settings.Value.FromAddress;
            var port = _settings.Value.SMTPPort;
            var password = _settings.Value.Password;

            try
            {
                switch (smtpClient)
                {
                    case "smtp.sendgrid.net":
                        List<EmailAddress> addressList = new List<EmailAddress>();
                        List<SendGrid.Helpers.Mail.Attachment> attachmentList = new List<SendGrid.Helpers.Mail.Attachment>();

                        var apiKey = password;
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(fromAddress);

                        foreach (string s in toAddresses)
                        {
                            addressList.Add(new EmailAddress(s));
                        }

                        var htmlBody = body;

                        if (attachmentFiles != null)
                        {
                            foreach (string path in attachmentFiles)
                            {
                                if (File.Exists(path))
                                {
                                    SendGrid.Helpers.Mail.Attachment attachment = new SendGrid.Helpers.Mail.Attachment();
                                    attachment.Filename = Path.GetFileName(path);
                                    var extension = Path.GetExtension(path);

                                    switch (extension)
                                    {
                                        case ".xlsx":
                                            attachment.Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                            break;
                                        case ".xls":
                                            attachment.Type = "application/vnd.ms-excel";
                                            break;
                                        case ".pdf":
                                            attachment.Type = "application/pdf";
                                            break;
                                    }
                                    attachment.Disposition = "attachment";
                                    attachment.Content = Convert.ToBase64String(File.ReadAllBytes(path));
                                    attachmentList.Add(attachment);
                                }
                            }
                        }

                        var message = MailHelper.CreateSingleEmailToMultipleRecipients(from, addressList, subject, "", htmlBody);

                        if (attachmentList.Count > 0)
                        {
                            message.AddAttachments(attachmentList);
                        }

                        await client.SendEmailAsync(message);

                        break;
                    case "smtp.office365.com":
                    case "smtp.ionos.com":
                    case "smtp.gmail.com":
                    default:
                        MailMessage mail = new MailMessage();
                        SmtpClient mailClient = new SmtpClient(smtpClient);
                        mail.From = new MailAddress(fromAddress);

                        foreach (string s in toAddresses)
                        {
                            mail.To.Add(s);
                        }

                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;

                        if (attachmentFiles != null)
                        {
                            foreach (string filePath in attachmentFiles)
                            {
                                System.Net.Mail.Attachment attachment;
                                attachment = new System.Net.Mail.Attachment(filePath);
                                mail.Attachments.Add(attachment);
                            }
                        }

                        mailClient.Port = Convert.ToInt32(port);
                        mailClient.Credentials = new NetworkCredential(fromAddress, password);
                        mailClient.EnableSsl = true;

                        await mailClient.SendMailAsync(mail);

                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email of type: {smtpClient}", smtpClient);
            }
        }
    }
}
