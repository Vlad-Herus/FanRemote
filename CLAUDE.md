# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FanRemote is a GPU fan control system with three components:

- **`HttpService/`** — ASP.NET Core (.NET 10) backend that reads GPU temp via `nvidia-smi` and computes fan speed
- **`ClientApp/`** — Vue 3 + Vite frontend dashboard for monitoring and manual speed override
- **`FanControl/`** — ESP32 Arduino firmware (PlatformIO) that polls the server and drives a PWM fan

## Commands

### HttpService (C#)
```bash
cd HttpService
dotnet build
dotnet run
```

### ClientApp (Vue/Vite)
```bash
cd ClientApp
npm install
npm run dev      # dev server
npm run build    # output to dist/
npm run lint     # oxlint + eslint
npm run format   # prettier
```

### FanControl (ESP32 / PlatformIO)
```bash
cd FanControl
pio run                        # build
pio run --target upload        # flash to device
pio device monitor             # serial monitor (115200 baud)
```

## Architecture & Data Flow

```
nvidia-smi → GpuTempSensor → TempDataCalculator → GpuMonitoringHostedService
                                                          ↓
                                                   SpeedControl (ramp algorithm)
                                                          ↓
                                                   TempHistoryStore (last 60 readings)
                                                          ↓
                          ┌───────────────────────────────┤
                          ↓                               ↓
                   GET /tempState                   GET /data (ETag)
                          ↓                               ↓
                      ESP32 → PWM fan             Vue LineChart + SpeedOverride
```

**`GpuMonitoringHostedService`** runs a background loop on `StepIntervalSeconds` (default 10s), calling `TempDataCalculator.Calculate()` → `SpeedControl.GetSpeed()` → `TempHistoryStore.LogTemp()`.

**`SpeedControl`** implements a ramp algorithm (0–255 range):
- Below `TempFloor` (55°C): speed = 0
- Above `TempCeiling` (85°C): speed = 255
- Between: max allowed speed scales linearly; current speed ramps up by `StepPercentage` per interval
- `FanControlConfiguration.ForcedSpeed` overrides the algorithm (set via `POST /speed`)

**ETag incremental polling**: `/data` returns only readings newer than the client's last-seen ETag. The Vue client sends its stored ETag as a request header on each poll (every 1s).

**`/tempState` uses HTTP 429**: This is intentional — the ESP32 firmware treats 429 as success and parses the body as the PWM speed value (0–255).

## Key Configuration (appsettings.json)

```json
"FanControlOptions": { "TempFloor": 55, "TempCeiling": 85, "StepPercentage": 2, "StepIntervalSeconds": 10 },
"NvidiaSmiOptions": { "MvidiaSmiExeLocation": "<path to nvidia-smi.exe>" },
"Kestrel": { "EndPoints": { "Http": { "Url": "http://*:6969" } } }
```

SPA static files root is hardcoded in `Program.cs`:
```csharp
configuration.RootPath = @"D:\Projects\FanRemote\ClientApp\dist\";
```
Run `npm run build` in `ClientApp/` before starting the backend to serve the frontend.

## ESP32 Firmware Setup

Before flashing `FanControl/src/main.cpp`, fill in:
- `ssid` / `password` — 2.4 GHz WiFi credentials
- `serverUrl` — e.g., `http://192.168.x.x:6969/tempState`

PWM output is on GPIO 13 at 25 kHz, 8-bit resolution.

## Service Deployment

The HttpService is configured to run as a Windows Service via `builder.Host.UseWindowsService()`. The `/data` request path is excluded from Serilog request logging to reduce noise.
