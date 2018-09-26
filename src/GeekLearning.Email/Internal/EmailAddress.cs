namespace GeekLearning.Email.Internal
{
    public enum AddressTarget { Cc, Bcc, ReplyTo }

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

    public class EmailAddressExt : IEmailAddress
    {
        private IEmailAddress _emailAddress {get; set;}

        public EmailAddressExt(IEmailAddress emailAddress) : base() {
            this._emailAddress = emailAddress;
        }

        public string Email {
            get { return this._emailAddress.Email; }
        }

        public string DisplayName {
            get { return this._emailAddress.DisplayName; }
        }

        public AddressTarget AddressAs { get; set; }
    }

    public static class EmailAddressExtensions
    {

        public static EmailAddressExt ToCc(this IEmailAddress emailAddress) 
        {
            return new EmailAddressExt(emailAddress) { AddressAs = AddressTarget.Cc };
        }

        public static EmailAddressExt ToBcc(this IEmailAddress emailAddress)
        {
            return new EmailAddressExt(emailAddress) { AddressAs = AddressTarget.Bcc };
        }

        public static EmailAddressExt ToReplyTo(this IEmailAddress emailAddress)
        {
            return new EmailAddressExt(emailAddress) { AddressAs = AddressTarget.ReplyTo };
        }
    }
}
