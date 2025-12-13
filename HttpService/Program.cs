using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public partial class Program
{
    private static bool Triggered = false;
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseWindowsService();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddTransient<IGpuTempService, GpuTempService>();

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

        app.MapGet("/tempState", async (IGpuTempService gpuTempService, IOptionsSnapshot<FanControlOptions> fanControlOptions) =>
        {
            var temp = await gpuTempService.GetGpuTempInC();

            if (temp >= fanControlOptions.Value.GpuTempCeiling)
            {
                Triggered = true;
                return FanOn(fanControlOptions);
            }
            else if (Triggered && temp > fanControlOptions.Value.GpuTempRecoveryThreshold)
                return FanOn(fanControlOptions);
            else
            {
                Triggered = false;
                return Results.StatusCode(200); 
            }
        })
        .WithName("tempState");

        app.Run();
    }

    private static IResult FanOn(IOptionsSnapshot<FanControlOptions> fanControlOptions)
    {
        return Results.Content(
            fanControlOptions.Value.FanSpeed.ToString(),
            System.Net.Mime.MediaTypeNames.Text.Plain,
            null,
            429);
    }
}
