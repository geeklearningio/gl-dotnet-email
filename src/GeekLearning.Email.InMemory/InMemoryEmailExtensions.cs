namespace GeekLearning.Email.InMemory
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class InMemoryEmailExtensions
    {
        public static IServiceCollection AddInMemoryEmail(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IEmailProviderType, InMemoryEmailProviderType>());
            return services;
        }
    }
}
