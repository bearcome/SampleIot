using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Text;
using MQTTnet.Extensions.ManagedClient;
using Microsoft.Extensions.Options;

namespace WpfVideoPlayer.Services
{
      public class Cl330Controller
      {
            public const string Cl330CommandTopic = "bearcome/switch/command";
            public const string Cl330ReportTopic = "bearcome/cl330/report"; 
            public const string Cl330MqttName = "Cl330MqttName";
            private readonly IMqttClient _mqttClient;
            private readonly CL330CommandBuilder _cL330CommandBuilder;
            public Cl330Controller([FromKeyedServices(Cl330MqttName)]IMqttClient mqttClient,
                  CL330CommandBuilder cL330CommandBuilder)
            {
                  _mqttClient = mqttClient;
                  _cL330CommandBuilder = cL330CommandBuilder;
            }

            public async Task StartAsync()
            {
                  var options = new MqttClientOptionsBuilder()
                                                .WithTcpServer("localhost", 1883) 
                                                .WithClientId($"{Cl330MqttName}Client")
                                                .Build();
                  await _mqttClient.ConnectAsync(options);
            }

            public async Task SetRedButProject0Async()
            {
                  var commandBytes = _cL330CommandBuilder.SetProject0();
                  await SendCl330Command(commandBytes);
            }
            public async Task SetBlueButProject1Async()
            {
                  var commandBytes = _cL330CommandBuilder.SetProject1();
                  await SendCl330Command(commandBytes);
            }

            public async Task SetButOffProject2Async()
            {
                  var commandBytes = _cL330CommandBuilder.SetProject2();
                  await SendCl330Command(commandBytes);
            }

            private async Task SendCl330Command(byte[] command)
            {
                  await Task.Delay(1000);

                  var message = new MqttApplicationMessageBuilder()
                        .WithTopic(Cl330CommandTopic)
                        .WithPayload(command)
                        .WithRetainFlag(false)
                        .Build();
                 
                  await _mqttClient.PublishAsync(message);
            }
      }

      public class LightConfig
      { 
            public ushort MenuStackLocation { get; set; }=2200-1;
            public ushort Menu0Value { get; set; }=2024;
            public ushort Menu1Value { get; set; }=2025;
            public ushort Menu2Value { get; set; } = 2026;
      }
      public class CL330CommandBuilder
      {
            //2200地址写入 2024 命令如下
            //01 10 08 97 00 01 02 07 E8 31 09 
            private const byte SlaveId = 0x01;
            private const byte FunctionCode = 0x10;
            private readonly IOptions<LightConfig> _lightConfig;

            public CL330CommandBuilder(IOptions<LightConfig> lightConfig)
            {
                  _lightConfig = lightConfig;
            }

            public byte[] SetProject0()
            {
                  //return new byte[] { 0x01, 0x10, 0x08, 0x97, 0x00, 0x01, 0x02, 0x07, 0xE8, 0x31, 0x09 };
                  return GererateData(_lightConfig.Value.MenuStackLocation, _lightConfig.Value.Menu0Value); 
            }
            public byte[] SetProject1()
            {
                  //return new byte[] { 0x01, 0x10, 0x08, 0x97, 0x00, 0x01, 0x02, 0x07, 0xE9, 0xF0, 0xC9  };
                  return GererateData(_lightConfig.Value.MenuStackLocation, _lightConfig.Value.Menu1Value);
            }
            public byte[] SetProject2()
            {
                  return GererateData(_lightConfig.Value.MenuStackLocation, _lightConfig.Value.Menu2Value);
            }
            public byte[] TurnOn()
            {
                  return new byte[] {  }; 
            }
            private byte[] GererateData(ushort stackLocation, ushort value)
            {
                  var order = new byte[11] { SlaveId, FunctionCode,
                        (byte)(stackLocation>>8),
                        (byte)(stackLocation & 0x00FF),
                        0x00,0x01,0x02,
                        (byte)(value >> 8),
                        (byte)(value & 0x00FF),
                        0x00, 0x00, };

                  var crc = Crc16.Compute(order.Take(order.Length-2).ToArray());
                  order[9] = crc[0];
                  order[10] = crc[1];
                  return order;
            }
      }
}
