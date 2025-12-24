using FanRemote.Services;
using Microsoft.Extensions.Options;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseWindowsService();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddTransient<IGpuTempSensor, GpuTempSensor>();
        builder.Services.AddTransient<ISpeedControl, SpeedControl>();
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
