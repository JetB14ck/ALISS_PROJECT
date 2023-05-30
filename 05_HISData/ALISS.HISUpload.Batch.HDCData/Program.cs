using ALISS.HISUpload.Batch.HDCData.DataAcess;
using ALISS.HISUpload.Batch.HDCData.DataRepos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ALISS.HISUpload.Batch.HDCData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Batch Starting...");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<HDCDataContext>(options =>
                        options.UseSqlServer(builder.Build().GetConnectionString("HDCDataContext")));

                    services.AddTransient<IProgramService, ProgramService>();
                    services.AddTransient<ITRSTGHISFileUploadHeaderRepo, TRSTGHISFileUploadHeaderRepo>();
                    services.AddTransient<ITRSTGHISFileUploadDetailRepo, TRSTGHISFileUploadDetailRepo>();
                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.CreateInstance<ProgramService>(host.Services);
            await svc.BATCH_HDCData_RUN();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
