namespace GeekLearning.Email.Internal
{
    public class EmailAddress : IEmailAddress
    {
        public EmailAddress()
        {
        }

        public EmailAddress(string email, string displayName)
        {
            this.Email = email;
            this.DisplayName = displayName;
        }

        public string Email { get; set; }

        public string DisplayName { get; set; }
    }
}
