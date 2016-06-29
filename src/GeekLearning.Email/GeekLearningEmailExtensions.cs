namespace GeekLearning.Email
{
    using Microsoft.Extensions.DependencyInjection;
    using Templating;

    public static class GeekLearningEmailExtensions
    {
        public static IServiceCollection AddEmail(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSender, Internal.EmailSender>();

            return services;
        }
    }
}
