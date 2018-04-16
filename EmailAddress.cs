namespace GeekLearning.Email.Internal
{
    public class EmailAddress : IEmailAddress
    {
        public EmailAddress()
        {
        }

        public EmailAddress(string email, string displayName, AddressTarget addressAs)
        {
            this.Email = email;
            this.DisplayName = displayName;
            this.AddressAs = addressAs;
        }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public AddressTarget AddressAs { get; set; }
    }
}
