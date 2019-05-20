# AzureIoTSolution
A simple solution for demonstrating the Azure IoT functionalities

Azure Function EventHub Trigger is a quick solution to your question.
Originally IoT Hub and Event Hub are more client concentric and machine to machine concentric respectively. Once you want to connect thousands of device in scale you have to use the IoTHub and provide the event processing loads to the back-end's Event-Hub layer.
[Here are the features of IoTHub and EventHub][1]

  [1]: https://i.stack.imgur.com/NNGTt.png

So the simplest way to gain mqtt messages from device to EventHub is to leverage IoTHub and employ an Azure Function comprises of Event-Hub Trigger to get the EventData. Finally Injest that EventData to Azure SQL Server.

You can access the IoTHub MQTT directly using sdk or azure iothub API,
otherwise using an MQTT Broker as a Bridge to IoTHub.

As first step try to use curl command to directly access Azure IoTHub API.

# Publish Example
```
mosquitto_pub -d -q 1 --capath /etc/ssl/certs/ -V mqttv311 -p 8883 \
  -h iothub007.azure-devices.net \
  -i device0001 \
  -u "iothub007.azure-devices.net/device0001/api-version=2016-11-14" \
  -P "SharedAccessSignature sr=xxxx&skn=xxxx&sig=xxxx&se=xxxx" \
  -t "devices/device0001/messages/events/"
  -m '{"message":"howdy"}'
```

# Subscribe Example

```
mosquitto_sub -d -q 1 --capath /etc/ssl/certs/ -V mqttv311 -p 8883 \
  -h iothub0007.azure-devices.net \
  -i device0001 \
  -u "iothub0007.azure-devices.net/device0001/api-version=2016-11-14" \
  -P "SharedAccessSignature sr=xxxx&skn=xxxx&sig=xxxx&se=xxxx" \
  -t "devices/device0001/messages/devicebound/#"
```

In Second Phase

You can try to use Azure IoT SDK to do the same as above.

If the second step seems hard then try to use MQTT broker as a bridge to the Azure IoT Hub(*).
I would recommend to use VerneMQ or Mosquitto.

Currently MS Azure IoT SDK do not support MQTT directly(AMQP is supported from SDK) , that is why its be quick to use an MQTT broker MQTT bridge.
