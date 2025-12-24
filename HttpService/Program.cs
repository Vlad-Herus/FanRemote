using FanRemote.Services;
using Microsoft.Extensions.Options;
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
        builder.Services.AddScoped<ISpeedControl, SpeedControl>();
        builder.Services.AddSingleton<IGpuTempHistoryStore, GpuTempHistoryStore>();
        builder.Services.AddHostedService<GpuMonitoringHostedService>();

        builder.Services.AddOptions<FanControlOptions>()
            .Bind(builder.Configuration.GetSection(nameof(FanControlOptions)));
        builder.Services.AddOptions<NvidiaSmiOptions>()
            .Bind(builder.Configuration.GetSection(nameof(NvidiaSmiOptions)));


        var app = builder.Build();
        
        

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
            var speed = speedControl.GetSpeed();
            return FanResult(speed);
        })
        .WithName("tempState");

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
