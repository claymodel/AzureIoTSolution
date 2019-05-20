using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static System.Console;

namespace AMQPConnect
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var iothubName = "vernemq";
            var hostName = $"{iothubName}.azure-devices.net";
            var deviceId = "dev1";
            var sharedAccessKey = "";
            var port = 5671;

            var deviceCertificateBytes = File.ReadAllBytes($"D:\\Certs\\{deviceId}.pfx");
            var deviceCertificatePassword = "1234";
            var deviceCertificate = (X509Certificate2)new X509Certificate2(deviceCertificateBytes, deviceCertificatePassword);

            Console.WriteLine("AMQP Connect!");

            using (var client = new IoTHubAmqpDeviceClient(hostName, deviceId, deviceCertificate, port))
            //using (var client = new IoTHubAmqpDeviceClient(hostName, deviceId, sharedAccessKey, port))
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
                    return new {
                        A = methodName,
                        B = methodArgs
                    };
                });
                await client.Cloud2DeviceMessages((x) =>
                {
                    WriteLine($"MessageReceived: {x.ToText()}");
                });

                await client.UpdateReportedProperties(new
                {
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
