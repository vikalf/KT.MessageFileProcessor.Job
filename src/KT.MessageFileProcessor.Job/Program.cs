using AutoMapper;
using Azure.Storage.Blobs;
using Grpc.Core;
using KT.Framework.Common.EventBus.Service;
using KT.Framework.Common.Infrastructure.Services;
using KT.MessageFileProcessor.Business.Components.Definition;
using KT.MessageFileProcessor.Business.Components.Implementation;
using KT.MessageFileProcessor.Business.Managers.Definition;
using KT.MessageFileProcessor.Business.Managers.Implementation;
using KT.MessageFileProcessor.Data.Repositories.Definition;
using KT.MessageFileProcessor.Data.Repositories.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Linq;

namespace KT.MessageFileProcessor.Job
{
    static class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static readonly string appName = "KT.MessageFileProcessor.Job";
        public static IServiceCollection ServiceProvider { get; set; }

        static void Main(string[] args)
        {
            try
            {
                SetupJob();
                bool isValidArgument = false;

                if (args.Contains("message-file-processor-student-events-xml"))
                {
                    isValidArgument = true;
                    Log.Information("Starting Message File Processor for Student Events..");


                    using var provider = ServiceProvider.BuildServiceProvider();
                    var manager = provider.GetRequiredService<IMessageFileProcessorManager>();

                    manager.ProcessStudentEventsXmlFiles().GetAwaiter().GetResult();

                    Log.Information("Student Events Xml Files Job Completed..");
                }

                if (!isValidArgument)
                {
                    Log.Error("No valid argument was provided");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Internal Error: " + ex.Message);
            }
        }

        private static void SetupJob()
        {
            SetConfiguration();
            CreateLogger();

            ServiceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(Configuration)
            .AddScoped<IMessageFileProcessorManager, MessageFileProcessorManager>()
            .AddScoped<IMessageFileProcessorComponent, MessageFileProcessorComponent>()
            .AddScoped<IXmlFileProcessor, XmlFileProcessor>()
            .AddScoped<IActivityStreamRepository, ActivityStreamRepository>()
            .AddScoped<IJobsRepository, JobsRepository>()
            .AddScoped<IIntegrationEventComponent, IntegrationEventComponent>()
            .AddSingleton(GetBlobServiceClient())
            .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
            .AddAutoMapper(typeof(AutoMapper.MappingProfile));



            ServiceProvider.AddEventBus(option =>
            {
                option.ConnectionString = Environment.GetEnvironmentVariable("SB_CONNECTION_STRING");
            });


            ServiceProvider.AddEnvironmentService();
            SetupGrpcClients(ServiceProvider);
        }

        private static void SetupGrpcClients(IServiceCollection services)
        {
            var grpc_proxy = Environment.GetEnvironmentVariable("GRPC_Proxy");
            var grpcChannel = new Channel(grpc_proxy, ChannelCredentials.Insecure);


            services.AddSingleton(typeof(KT.Jobs.Service.Definition.Jobs.JobsClient),
                new KT.Jobs.Service.Definition.Jobs.JobsClient(grpcChannel));

            services.AddSingleton(typeof(KT.ActivityStream.Service.Definition.ActivityStream.ActivityStreamClient),
                new KT.ActivityStream.Service.Definition.ActivityStream.ActivityStreamClient(grpcChannel));

        }

        private static BlobServiceClient GetBlobServiceClient()
        {

            string connectionString = Environment.GetEnvironmentVariable("SA_CONNECTION_STRING");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            return blobServiceClient;

        }

        private static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console(new CompactJsonFormatter())
              .ReadFrom.Configuration(Configuration)
              .CreateLogger();
        }

        private static void SetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
               .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
               .AddJsonFile($"sharedsettings.json", optional: true)
               .AddJsonFile($"/app/settings/sharedsettings.json", optional: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();
        }


    }
}
