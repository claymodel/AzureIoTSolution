using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace MQTTConnect
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            // args
            var iothubName = "vernemq";
            var hostName = $"{iothubName}.azure-devices.net";
            var deviceId = "dev1";
            var sharedAccessKey = "";
            var port = 8883;

            var deviceCertificateBytes = File.ReadAllBytes($"D:\\Certs\\{deviceId}.pfx");
            var deviceCertificatePassword = "1234";
            var deviceCertificate = (X509Certificate2)new X509Certificate2(deviceCertificateBytes, deviceCertificatePassword);

            using (var client = new IoTHubMqttDeviceClient(hostName, deviceId, deviceCertificate, port))
            {
                await client.GetTwin((x) =>
                {
                    WriteLine($"Received twin: {x}");
                });
                await client.DeclareDesiredPropertiesHandler((x) =>
                {
                    WriteLine($"Desired Properties: {x}");
                });
                await client.DeclareDirectMethodHandler((methodName, methodArgs) =>
                {
                    return new
                    {
                        A = methodName,
                        B = methodArgs
                    };
                });
                await client.Cloud2DeviceMessages((p, x) =>
                {
                    WriteLine($"MessageReceived: {x}");
                    WriteLine($"PayloadReceived: {p}");
                });

                await client.UpdateReportedProperties(new {
                    r5 = $"{DateTime.Now}",
                    r6 = $"{DateTime.Now}",
                }, (x) =>
                {
                    Console.Write($"UpdateReportedProperties: {x}");
                });

                var random = new Random();

                while (true)
                {
                    var json = JsonConvert.SerializeObject(new
                    {
                        DeviceId = deviceId,
                        Data = random.Next(0, 100),
                        DateTime = DateTimeOffset.Now
                    });

                    await client.SendEventAsString(json);

                    WriteLine(json);

                    await Task.Delay(1000);
                }
            }
        }
    }
}
