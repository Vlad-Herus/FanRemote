#include "driver/ledc.h"
#include <Arduino.h>
#include <HTTPClient.h>
#include <WiFi.h>

const char *ssid = ""; // 2.4GHz only
const char *password = "";
const char *serverUrl = "http://:6969/tempState";

const int OUT_PIN = 13;

void setup() {

  Serial.begin(115200);

  pinMode(OUT_PIN, OUTPUT);

  if (ledcAttach(13, 25000, 8) == false) {
    Serial.println("Failed to attach 13");
  }

  ledcWrite(OUT_PIN, 0);

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {

    char buffer[50]; // Ensure enough space for the combined string

    sprintf(buffer, "%s%d", "Connecting to WiFi...", WiFi.status());
    Serial.println(buffer);
    delay(1000);
  }

  Serial.println("Connected to WiFi");
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
}

void loop() {
  if (WiFi.status() == WL_CONNECTED) {

    Serial.print("making request: ");

    HTTPClient http;
    http.begin(serverUrl);
    int httpResponseCode = http.GET();

    Serial.print("HTTP Response Code: ");
    Serial.println(httpResponseCode);

    if (httpResponseCode == 429) {
      String payload = http.getString();
      long speed = payload.toInt();
      if (speed >= 0 && speed <= 255) {
        Serial.print("Speed: ");
        Serial.println(payload);

        ledcWrite(OUT_PIN, speed);
      } else {
        Serial.print("Speed not parsed: ");
        Serial.println(payload);
      }
    } else {
      ledcWrite(OUT_PIN, 0);
    }

    http.end();
    delay(10000);
  }
}