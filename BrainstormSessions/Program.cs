using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.Email;

namespace BrainstormSessions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration= new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.File(new JsonFormatter(),@"c:\temp\logs.log",shared: true)
                .WriteTo.Email(new EmailConnectionInfo
                {
                    FromEmail = "from",
                    ToEmail = "to",
                    MailServer = "smtp.mail.ru",
                    NetworkCredentials = new NetworkCredential
                    {
                        UserName = "UserName@mail.ru",
                        Password = "password"
                    },
                    EnableSsl = true,
                    Port = 465,
                    EmailSubject = "Error in app"
                }, restrictedToMinimumLevel: LogEventLevel.Debug, batchPostingLimit: 1)
                .CreateLogger();

            try
            {
                Log.Information("Starting web application");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }

            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
