namespace GeekLearning.Email
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using SendGrid;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSendGridEmail(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IEmailProviderType, SendGridEmailProviderType>());
            return services;
        }
    }
}
