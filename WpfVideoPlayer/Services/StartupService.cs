using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace WpfVideoPlayer.Services
{
      public class StartupService : BackgroundService
      {

            private readonly MqttServer _mqttServer;
            private readonly BtnListener _btnListener;
            private readonly Cl330Controller _cl330Controller;
            private readonly ILogger<StartupService> _log;
            private readonly SwitchController _switchController;
            private readonly ISerialService _serialService;

            public StartupService(MqttServer mqttServer, 
                  BtnListener btnListener,
                  ISerialService serialService,
                  Cl330Controller cl330Controller,
                  SwitchController switchController,
                  ILogger<StartupService> log)
            {
                  _cl330Controller = cl330Controller;
                  _mqttServer = mqttServer;
                  _btnListener = btnListener;
                  _switchController = switchController;
                  _serialService = serialService;
                  _log = log;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                  _mqttServer.ClientConnectedAsync += _mqttServer_ClientConnectedAsync;
                  _mqttServer.InterceptingPublishAsync += _mqttServer_InterceptingPublishAsync;

                  await _mqttServer.StartAsync();
                  await _btnListener.StartAsync();
                  await _cl330Controller.StartAsync();
                  await _switchController.StartAsync();

                  _serialService.RegistButOnTask(_switchController.RedButOnAsync, _switchController.BlueButOnAsync);
                  _serialService.RegistButOnTask(_cl330Controller.SetRedButProject0Async, _cl330Controller.SetBlueButProject1Async);
                  _serialService.RegistButOffTask(_switchController.RedButOffAsync, _switchController.BlueButOffAsync);
                  _serialService.RegistButOffTask(_cl330Controller.SetButOffProject2Async, _cl330Controller.SetButOffProject2Async);

#warning Test code, remove it later

                  //await Task.Delay(20000);
                  for (int i = 0; i < 1000; i++)
                  {
                        //await Task.Delay(10000);
                        //await _cl330Controller.SetRedButProject0Async();
                        //await Task.Delay(10000);
                        //await _cl330Controller.SetBlueButProject1Async();
                        //await Task.Delay(10000);
                        //await _cl330Controller.SetButOffProject2Async();
                        //await _serialService.Test();

                  }


            }



            private Task _mqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
                  _log.LogInformation(@$"ClientId:{arg.ClientId}  Published A  Message 
                        Topic :{arg.ApplicationMessage.Topic}
                        Message:{string.Concat(arg.ApplicationMessage.Payload.ToArray().Select(b => b.ToString("X2")))}");
                  return Task.CompletedTask;
            }

        private Task _mqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
            {
                  _log.LogInformation($"ClientId:{arg.ClientId} Connected");
                  return Task.CompletedTask;
            }
      }
}
