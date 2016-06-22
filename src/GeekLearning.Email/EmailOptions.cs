namespace GeekLearning.Email
{
    public class EmailOptions
    {
        public string Key { get; set; }

        public string User { get; set; }

        public Implementation.EmailAddress DefaultSender { get; set; }

        public string TemplateStorage { get; set; }
    }
}
