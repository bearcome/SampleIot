using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace WpfVideoPlayer.Services
{
      public class BtnListener
      {
            public const string BtnListenerMqttName = "BtnListenerMqttName";
            private readonly IMqttClient _mqttClient;
            private readonly BtnStatus _btnStatus;
            private readonly ISerialService _serialService;
            //上行
            private const string UpTopic = "bearcome/tbnup";
            //用不到  下行
            private const string DownTopic = "bearcome/tbndown"; 

            public BtnListener([FromKeyedServices(BtnListener.BtnListenerMqttName)]IMqttClient mqttClient, 
                  BtnStatus btnStatus,
                  ISerialService serialService)
            {
                  _mqttClient = mqttClient;
                  _btnStatus = btnStatus;
                  _serialService = serialService;
            }

            public async Task StartAsync()
            {
                  var options = new MqttClientOptionsBuilder()
                                                .WithTcpServer("localhost", 1883) // 公共测试 Broker
                                                .WithClientId("MyCSharpClient")
                                                .Build();
                  await _mqttClient.ConnectAsync(options);

                  _mqttClient.ApplicationMessageReceivedAsync += _mqttClient_ApplicationMessageReceivedAsync;
                  await _mqttClient.SubscribeAsync(UpTopic);
                 
            }

        private async Task _mqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
                  var data = arg.ApplicationMessage.Payload;

                  var res = _btnStatus.ReadBtnStatus(data.ToArray());
                  if (res.HasValue && res.Value.Value)
                  {
                        await _serialService.ButDown(res.Value.Key);
                  }
                  else if (res.HasValue && !res.Value.Value)
                  {
                        await _serialService.ButUp(res.Value.Key);
                  }
            }

      }
}
