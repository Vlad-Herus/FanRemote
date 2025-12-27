using FanRemote.Interfaces;
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
        );

        builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = @"D:\Projects\FanRemote\ClientApp\dist\"; });
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddTransient<IGpuTempSensor, GpuTempSensor>();
        builder.Services.AddTransient<IPidCalculator, PidCalculator>();
        builder.Services.AddTransient<ISpeedControl, SpeedControl>();
        builder.Services.AddTransient<IPidCacheService, PidCacheService>();
        builder.Services.AddSingleton<IPidHistoryStore, PidHistoryStore>();
        builder.Services.AddHostedService<GpuMonitoringHostedService>();

        builder.Services.AddOptions<FanControlOptions>()
            .Bind(builder.Configuration.GetSection(nameof(FanControlOptions)));
        builder.Services.AddOptions<NvidiaSmiOptions>()
            .Bind(builder.Configuration.GetSection(nameof(NvidiaSmiOptions)));
        builder.Services.AddOptions<PidOptions>()
            .Bind(builder.Configuration.GetSection(nameof(PidOptions)));

        builder.Services.AddSingleton<PidConfiguration>();


        var app = builder.Build();

        var pidOptions = app.Services.GetRequiredService<IOptions<PidOptions>>();
        var pidConfiguration = app.Services.GetRequiredService<PidConfiguration>();
        pidConfiguration.Proportional = pidOptions.Value.Proportional;
        pidConfiguration.Integral = pidOptions.Value.Integral;
        pidConfiguration.Derivative = pidOptions.Value.Derivative;

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseSerilogRequestLogging();
        app.UseSpaStaticFiles();
        app.UseHttpsRedirection();

        app.MapGet("/tempState", async (ISpeedControl speedControl) =>
        {
            return 0;
        })
        .WithName("tempState");

        app.MapGet("/data", async (
            IPidHistoryStore pidHistoryStore,
            IPidCacheService pidCacheService,
            [FromHeader(Name = "ETag")] string? eTag,
            HttpResponse response) =>
        {
            var temps = pidHistoryStore.GetTemps();
            var filtered = pidCacheService.Filter(temps, eTag);
            var newEtag = pidCacheService.GetETag(filtered);
            if (newEtag is not null)
                response.Headers.Append(HeaderNames.ETag, newEtag);

            return filtered;
        })
        .WithName("data");

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
