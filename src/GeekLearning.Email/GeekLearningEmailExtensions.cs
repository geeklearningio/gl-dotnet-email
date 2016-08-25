namespace GeekLearning.Email
{
    using Microsoft.Extensions.DependencyInjection;
    using Providers.InMemory;
    using Providers.SendGrid;
    using System;

    public static class GeekLearningEmailExtensions
    {
        public static IServiceCollection AddEmail(this IServiceCollection services, EmailProvider emailProvider)
        {
            switch (emailProvider)
            {
                case EmailProvider.InMemory:
                    services.AddSingleton<IEmailSender, InMemoryEmailSender>();
                    break;
                case EmailProvider.SendGrid:
                    services.AddSingleton<IEmailSender, SendGridEmailSender>();
                    break;
                default:
                    throw new NotSupportedException();
            }

            return services;
        }
    }
}
