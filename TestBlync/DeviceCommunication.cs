using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;


//GitHub Repository: https://github.com/tfeissle/Blync-Pi


namespace TestBlync
{
    public class DeviceCommunication        
    {
        private const string DeviceConnectionString = "HostName=xxxx.azure-devices.net;DeviceId=BlyncLight;SharedAccessKey=xxxxxxxx==;GatewayHostName=ssl://xxxxx:8883";

        private const string DeviceId = "BlyncLight";
        private const string HostName = "xxxxx.azure-devices.net";
        private const string DeviceKey = "xxxxxxxx";
        private const string HubSuffix = "azure-devices.net";
        private const string HubName = "xxxxx";

        private static int MESSAGE_COUNT = 5;

        private readonly ISerialize _serializer;
        private readonly ILogger _logger;
        private readonly IConfigurationProvider _configurationProvider;
        private DeviceClient _deviceClient;
        private bool _disposed = false;

        public static DeviceClient InitializeDeviceCommunication()
        {
            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString,TransportType.Http1);

                if (deviceClient == null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create DeviceClient!");
                }
                else
                {
                    return deviceClient;
                }
                System.Diagnostics.Debug.WriteLine("Exited!\n");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in sample: {0}", ex.Message);
            }
            return null;
        }

        public static async Task SendEvent(DeviceClient deviceClient, string cmd)
        {
            string dataBuffer;
            System.Diagnostics.Debug.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);
            if (deviceClient != null)
            {
                for (int count = 0; count < MESSAGE_COUNT; count++)
                {
                    dataBuffer = cmd + ", " + Guid.NewGuid().ToString();
                    Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
                    System.Diagnostics.Debug.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, dataBuffer);

                    await deviceClient.SendEventAsync(eventMessage);
                }
            }
        }


        //Look here for details: https://azure.microsoft.com/en-us/develop/iot/
        public static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("\nDevice waiting for commands from IoTHub...\n");
                Message receivedMessage;
                string messageData;
                if (deviceClient != null)
                {
                    //while (true)
                    //{
                    receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(5));
                    if (receivedMessage != null)
                    {
                        messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        System.Diagnostics.Debug.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                        await deviceClient.CompleteAsync(receivedMessage);
                        return;
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error from IoTHub: " + ex.Message.ToString());
            }
        }
        public async Task<DeserializableCommand> ReceiveAsync()
        {
            Message message = await AzureRetryHelper.OperationWithBasicRetryAsync(async () =>
            {
                try
                {
                    return await _deviceClient.ReceiveAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }

                return null;
            });

            if (message != null)
            {
                await AzureRetryHelper.OperationWithBasicRetryAsync(async () =>
                {
                    try
                    {
                        await _deviceClient.CompleteAsync(message.LockToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                });
            }

            if (message != null)
            {
                return new DeserializableCommand(message, _serializer);
            }
            return null;
        }
    }
}
