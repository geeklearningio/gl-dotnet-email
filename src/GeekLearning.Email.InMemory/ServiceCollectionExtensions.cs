namespace GeekLearning.Email
{
    using InMemory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryEmail(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IEmailProviderType, InMemoryEmailProviderType>());
            services.AddSingleton<IInMemoryEmailRepository, InMemoryEmailRepository>();
            return services;
        }
    }
}
