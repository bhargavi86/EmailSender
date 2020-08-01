using System.Collections.Generic;

namespace EmailSender.Models
{
    public class Email
    {
        public Email()
        {
            toAddresses = new List<string>();
            attachmentFilePaths = new List<string>();
            deleteFiles = false;
        }

        public string body { get; set; }

        public string subject { get; set; }

        public List<string> toAddresses { get; set; }

        public List<string> attachmentFilePaths { get; set; }

        public bool deleteFiles { get; set; }
    }
}
