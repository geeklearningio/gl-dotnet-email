namespace GeekLearning.Email
{
    using Microsoft.Extensions.DependencyInjection;
    using Providers.InMemory;
    using Providers.SendGrid;

    public static class GeekLearningEmailExtensions
    {
        public static IServiceCollection AddEmailSendGrid(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSender, SendGridEmailSender>();

            return services;
        }

        public static IServiceCollection AddEmailInMemory(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSender, InMemoryEmailSender>();

            return services;
        }
    }
}
