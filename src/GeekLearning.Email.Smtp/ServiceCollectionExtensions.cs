namespace GeekLearning.Email
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Smtp;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSmtpEmail(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IEmailProviderType, SmtpEmailProviderType>());
            return services;
        }
    }
}
