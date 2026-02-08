using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfVideoPlayer.Services
{
    public class SwitchController
    {
            public const string SwitchMqttName = "SwitchMqttName";
            private const string SwitchCommandTopic = "bearcome/switch/command";
            private const string SwitchReportTopic = "bearcome/switch/report";

            private readonly IMqttClient _mqttClient;
            private readonly SwitchRule _switchRule;
            private readonly IOptions<SwitchConfig> _switchConfig;

            public SwitchController([FromKeyedServices(SwitchMqttName)] IMqttClient mqttClient, 
                  SwitchRule switchRule,
                  IOptions<SwitchConfig> switchConfig)
            { 
                  _mqttClient = mqttClient;
                  _switchRule = switchRule;
                  _switchConfig = switchConfig;
            }


            public async Task StartAsync()
            {
                  var options = new MqttClientOptionsBuilder()
                                                .WithTcpServer("localhost", 1883)
                                                .WithClientId($"{SwitchMqttName}Client")
                                                .Build();
                  await _mqttClient.ConnectAsync(options);
            }

            public async Task RedButOnAsync()
            {
                  var commandBytes = _switchRule.SwitchOn(_switchConfig.Value.RedButOnFlag);
                  var message = new MqttApplicationMessageBuilder()
                        .WithTopic(SwitchCommandTopic)
                        .WithPayload(commandBytes)
                        .WithRetainFlag(false)
                        .Build();
                  await _mqttClient.PublishAsync(message);
            }
            public async Task RedButOffAsync()
            {
                  var commandBytes = _switchRule.SwitchOff(_switchConfig.Value.RedButOffFlag);
                  var message = new MqttApplicationMessageBuilder()
                        .WithTopic(SwitchCommandTopic)
                        .WithPayload(commandBytes)
                        .WithRetainFlag(false)
                        .Build();
                  await _mqttClient.PublishAsync(message);
            }

            public async Task BlueButOnAsync()
            {
                  var commandBytes = _switchRule.SwitchOn(_switchConfig.Value.BlueButOnFlag);
                  var message = new MqttApplicationMessageBuilder()
                        .WithTopic(SwitchCommandTopic)
                        .WithPayload(commandBytes)
                        .WithRetainFlag(false)
                        .Build();
                  await _mqttClient.PublishAsync(message);
            }
            public async Task BlueButOffAsync()
            {
                  var commandBytes = _switchRule.SwitchOff(_switchConfig.Value.BlueButOffFlag);
                  var message = new MqttApplicationMessageBuilder()
                        .WithTopic(SwitchCommandTopic)
                        .WithPayload(commandBytes)
                        .WithRetainFlag(false)
                        .Build();
                  await _mqttClient.PublishAsync(message);
            }
      }

      public class SwitchConfig
      { 
            public byte RedButOnFlag { get; set; } =0x05;
            public byte RedButOffFlag { get; set; } = 0x05;
            public byte BlueButOnFlag { get; set; } = 0x0a;
            public byte BlueButOffFlag { get; set; } = 0x0a;
      }
}
