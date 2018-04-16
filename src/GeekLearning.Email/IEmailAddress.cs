namespace GeekLearning.Email
{
    public enum AddressTarget {  From, To, Cc, Bcc, ReplyTo}

    public interface IEmailAddress
    {
        string Email { get; }

        string DisplayName { get; }

        AddressTarget AddressAs { get; }
  
    }
}
