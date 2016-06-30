using System;

namespace GeekLearning.Email.Samples
{
    public class User : IEmailAddress
    {
        public string Email { get; set; }

        public string DisplayName { get; set; }
    }
}
