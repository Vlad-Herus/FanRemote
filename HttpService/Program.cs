using System.Security.Cryptography.X509Certificates;
using FanRemote.Interfaces;
using FanRemote.Model;
using FanRemote.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Serilog;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseWindowsService();
        builder.Host.UseSerilog((HostBuilderContext context, LoggerConfiguration loggerConfig) =>
          loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Filter.ByExcluding(c =>
                {
                    if (!c.Properties.ContainsKey("RequestPath"))
                        return false;
                    bool data = c.Properties["RequestPath"].ToString().Trim("\"").StartsWith("/data");
                    if (data)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                })
        );

        builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = @"D:\Projects\FanRemote\ClientApp\dist\"; });
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddTransient<IGpuTempSensor, GpuTempSensor>();
        builder.Services.AddTransient<ITempDataCalculator, TempDataCalculator>();
        builder.Services.AddSingleton<ISpeedControl, SpeedControl>();
        builder.Services.AddTransient<IETagService, ETagService>();
        builder.Services.AddSingleton<ITempHistoryStore, TempHistoryStore>();
        builder.Services.AddHostedService<GpuMonitoringHostedService>();

        builder.Services.AddOptions<FanControlOptions>()
            .Bind(builder.Configuration.GetSection(nameof(FanControlOptions)));
        builder.Services.AddOptions<NvidiaSmiOptions>()
            .Bind(builder.Configuration.GetSection(nameof(NvidiaSmiOptions)));
        builder.Services.AddOptions<FanControlOptions>()
            .Bind(builder.Configuration.GetSection(nameof(FanControlOptions)));

        builder.Services.AddSingleton<FanControlConfiguration>();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseSerilogRequestLogging();
        app.UseSpaStaticFiles();
        app.UseHttpsRedirection();

        app.MapGet("/tempState", (ITempHistoryStore store) =>
        {
            var current = store
            .GetTemps()
            .OrderByDescending(pid => pid.Timestamp)
            .FirstOrDefault();

            return FanResult(current?.Speed ?? 0);
        })
        .WithName("tempState");

        app.MapGet("/data", async (
            ITempHistoryStore TempHistoryStore,
            IETagService ETagService,
            [FromHeader(Name = "ETag")] string? eTag,
            HttpResponse response) =>
        {
            var temps = TempHistoryStore.GetTemps();
            var filtered = ETagService.Filter(temps, eTag);
            var newEtag = ETagService.GetETag(filtered);
            if (newEtag is not null)
                response.Headers.Append(HeaderNames.ETag, newEtag);
            else
                response.Headers.Append(HeaderNames.ETag, eTag);

            return filtered;
        })
        .WithName("data");

        app.MapPost("/speed", async (
            [FromBody] SpeedRequest speedRequest,
            FanControlConfiguration fanControlConfiguration) =>
        {
            fanControlConfiguration.ForcedSpeed = speedRequest.ForcedSpeed;
        })
        .WithName("speed");

        app.Run();
    }

    private static IResult FanResult(int speed)
    {
        return Results.Content(
            speed.ToString(),
            System.Net.Mime.MediaTypeNames.Text.Plain,
            null,
            429);
    }
}
