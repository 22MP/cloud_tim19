using SmartApiary.Simulator.NsDevice;
using SmartApiary.Simulator.Simulator;
using System.Text;
using System.Text.Json;


const string HANDSHAKE_URL = "http://localhost:7139/api/device/handshake";

Device device = new Device();
string? accessToken = null;

Console.WriteLine("SmartApiary Device Simulator");
Console.WriteLine($"Device UUID: {device.DeviceUuid}");
Console.WriteLine();

Console.Write("Enter Serial Number: (SA-YYYY-XXXXX): ");
var serialNumber = Console.ReadLine() ?? "";

accessToken = await DoHandshake(device.DeviceUuid, serialNumber);
if (accessToken is null)
{
    Console.WriteLine("Handshake failed.");
    return Exit();
}
try { device.SetAccessToken(accessToken); } catch { Console.WriteLine("Error in setting acces token. Exiting..."); return Exit(); }

Simulator simulator = new Simulator(device);
await simulator.Start();

Console.ReadLine();
return 0;

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

static int Exit()
{
    Console.WriteLine("Exiting...");
    Console.ReadLine();
    return -1;
}