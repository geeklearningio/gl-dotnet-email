namespace GeekLearning.Email
{
    public interface IEmailAttachment
    {
        string FileName { get; set; }
        byte[] Data { get; set; }
        string MediaType { get; set; }
        string MediaSubtype { get; set; }
        string ContentType { get; }
    }
}
