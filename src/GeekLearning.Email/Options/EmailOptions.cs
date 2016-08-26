namespace GeekLearning.Email
{
    using Internal;

    public class EmailOptions 
    {
        public EmailProviderOptions Provider { get; set; }

        public EmailAddress DefaultSender { get; set; }

        public string TemplateStorage { get; set; }

        public MockupOptions Mockup { get; set; } = new MockupOptions();
    }
}
