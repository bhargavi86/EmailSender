namespace EmailSender.Models
{
    public class EmailSettings
    {
        public string SMTPClient { get; set; }

        public string FromAddress { get; set; }

        public string Password { get; set; }

        public string SMTPPort { get; set; }
    }
}
