namespace GeekLearning.Email.Smtp
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class SmtpEmailExtensions
    {
        public static IServiceCollection AddSmtpEmail(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IEmailProviderType, SmtpEmailProviderType>());
            return services;
        }
    }
}
