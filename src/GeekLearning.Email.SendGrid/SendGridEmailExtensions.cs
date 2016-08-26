namespace GeekLearning.Email.SendGrid
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class SendGridEmailExtensions
    {
        public static IServiceCollection AddSendGridEmail(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IEmailProviderType, SendGridEmailProviderType>());
            return services;
        }
    }
}
