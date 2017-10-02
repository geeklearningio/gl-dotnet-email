namespace GeekLearning.Email.Integration.Test
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Storage;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Templating;

    public class StoresFixture : IDisposable
    {
        private CloudStorageAccount cloudStorageAccount;
        private CloudBlobContainer container;

        public StoresFixture()
        {
            this.BasePath = PlatformServices.Default.Application.ApplicationBasePath;

            var builder = new ConfigurationBuilder()
                .SetBasePath(BasePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.development.json", optional: true).
                AddInMemoryCollection(new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("Storage:Stores:azure:FolderName", Guid.NewGuid().ToString("N").ToLower())
                });

            this.Configuration = builder.Build();

            this.SendGridUser = Configuration["SendGrid:User"];
            this.SendGridKey = Configuration["SendGrid:Key"];

            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddOptions();

            services.AddStorage(Configuration)
                .AddAzureStorage()
                .AddFileSystemStorage(BasePath);
            services.AddTemplating()
                .AddHandlebars();

            this.Services = services.BuildServiceProvider();

            ResetStores();
        }

        public string SendGridKey { get; set; }

        public string SendGridUser { get; set; }

        private void ResetStores()
        {
            ResetAzureStore();
            ResetFileSystemStore();
        }

        private void ResetFileSystemStore()
        {
            var directoryName = Configuration["Storage:Stores:filesystem:FolderName"];
            var process = Process.Start(new ProcessStartInfo("robocopy.exe")
            {
                Arguments = $"\"{System.IO.Path.Combine(BasePath, "SampleDirectory")}\" \"{System.IO.Path.Combine(BasePath, directoryName)}\" /MIR"
            });

            if (!process.WaitForExit(30000))
            {
                throw new TimeoutException("File system store was not reset properly");
            }
        }

        private void ResetAzureStore()
        {
            var azCopy = System.IO.Path.Combine(
                Environment.ExpandEnvironmentVariables(Configuration["AzCopyPath"]),
                "AzCopy.exe");

            cloudStorageAccount = CloudStorageAccount.Parse(Configuration["Storage:Stores:azure:ConnectionString"]);
            var key = cloudStorageAccount.Credentials.ExportBase64EncodedKey();
            var containerName = Configuration["Storage:Stores:azure:FolderName"];
            var dest = cloudStorageAccount.BlobStorageUri.PrimaryUri.ToString() + containerName;

            var client = cloudStorageAccount.CreateCloudBlobClient();

            container = client.GetContainerReference(containerName);
            container.CreateAsync().Wait();

            var process = Process.Start(new ProcessStartInfo(azCopy)
            {
                Arguments = $"/Source:\"{System.IO.Path.Combine(BasePath, "SampleDirectory")}\" /Dest:\"{dest}\" /DestKey:{key} /S"
            });

            if (!process.WaitForExit(30000))
            {
                throw new TimeoutException("Azure store was not reset properly");
            }
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider Services { get; }

        public string BasePath { get; }

        public void Dispose()
        {
            container.DeleteIfExistsAsync().Wait();
        }
    }
}
