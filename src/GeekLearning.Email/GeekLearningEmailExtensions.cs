namespace GeekLearning.Email
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class GeekLearningEmailExtensions
    {
        public static IServiceCollection AddEmail(this IServiceCollection services)
        {
            services.TryAddTransient<IEmailSender, Internal.EmailSender>();
            return services;
        }
    }
}
