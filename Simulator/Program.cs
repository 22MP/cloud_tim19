using System.Text;
using System.Text.Json;

const string HANDSHAKE_URL = "http://localhost:7139/api/device/handshake";
const string TELEMETRY_URL = "http://localhost:5062/api/devices/telemetry";
// 7262

string deviceUuid = Guid.NewGuid().ToString();
string? accessToken = null;

Console.WriteLine("SmartApiary Device Simulator");
Console.WriteLine($"Device UUID: {deviceUuid}");
Console.WriteLine();

Console.Write("Enter Serial Number: (SA-YYYY-XXXXX): ");
var serialNumber = Console.ReadLine() ?? "";

accessToken = await DoHandshake(deviceUuid, serialNumber);
if (accessToken is null)
{
    Console.WriteLine("Handshake failed. Exiting...");
    Console.ReadLine();
    return;
}

// Telemetry simulation
Console.WriteLine("Handshake succeded! Starting with telemetry samples...");

var rnd = new Random();
double baseWeight = 30.0 + rnd.NextDouble() * 20;

while (true)
{
    // Sim...
    baseWeight += (rnd.NextDouble() - 0.48) * 0.5; 
    baseWeight = Math.Max(5.0, baseWeight);        

    var measurement = new
    {
        WeightKg = Math.Round(baseWeight, 2),
        Humidity = Math.Round(40.0 + rnd.NextDouble() * 40, 1),
        Temperature = Math.Round(32.0 + rnd.NextDouble() * 8, 1),
        BatteryPercentage = rnd.Next(10, 100),
        RecordedAt = DateTime.UtcNow
    };

    await SendTelemetry(accessToken, measurement);

    // 5s
    await Task.Delay(5000);
}


static async Task<string?> DoHandshake(string uuid, string serialNumber)
{
    Console.WriteLine($"Executing handshake...");

    using HttpClient http = new HttpClient();
    var payload = new { DeviceUuid = uuid, SerialNumber = serialNumber.Trim() };
    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    try
    {
        var response = await http.PostAsync(HANDSHAKE_URL, content);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonSerializer.Deserialize<JsonElement>(json);
            var token = obj.GetProperty("data").GetProperty("accessToken").GetString();
            Console.WriteLine($"Token received: {token}");
            return token;
        }

        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Handshake error: {response.StatusCode}: {error}");
        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Connection failed: {ex.Message}");
        Console.WriteLine("Check wether functions and WebApi are running");
        return null;
    }
}

static async Task SendTelemetry(string token, object measurement)
{
    using var http = new HttpClient();
    http.DefaultRequestHeaders.Add("X-Device-Token", token);

    var content = new StringContent(JsonSerializer.Serialize(measurement), Encoding.UTF8, "application/json");

    try
    {
        var response = await http.PostAsync(TELEMETRY_URL, content);

        if (response.IsSuccessStatusCode)
        {
            var m = (dynamic)measurement;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Sent: " +$"Weight={((JsonElement)JsonSerializer.SerializeToElement(measurement)).GetProperty("WeightKg")}kg");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode}: {error}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error while sending: {ex.Message}");
    }
}