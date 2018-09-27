using System.Net.Mime;

namespace GeekLearning.Email.Internal
{
    public class EmailAttachment : IEmailAttachment
    {
        public EmailAttachment()
        {
        }

        public EmailAttachment(string fileName, byte[] data, string mediaType, string mediaSubtype)
        {
            FileName = fileName;
            Data = data;
            MediaType = mediaType;
            MediaSubtype = mediaSubtype;
        }

        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string MediaType { get; set; }
        public string MediaSubtype { get; set; }
        public string ContentType => string.Join("/", MediaType, MediaSubtype);
    }
}
