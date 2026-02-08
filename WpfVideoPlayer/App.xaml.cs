using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Server;
using System.Configuration;
using System.Data;
using System.Windows;
using WpfVideoPlayer.Services;
using MQTTnet.LowLevelClient;
using MQTTnet.Extensions.ManagedClient;


namespace WpfVideoPlayer
{
      /// <summary>
      /// Interaction logic for App.xaml
      /// </summary>
      public partial class App : Application
      {
            private IHost _host;

            protected override void OnStartup(StartupEventArgs e)
            {

                  var hostBuilder = Host.CreateDefaultBuilder(e.Args)
                        .ConfigureServices((context, services) =>
                        {
                              // 构建配置
                              context.Configuration = new ConfigurationBuilder()
                                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                  .Build();

                              // 注册服务
                              services.AddSingleton<ISerialService, SerialService>();

                              services.AddSingleton<MqttServer>(opt => {
                                    var mqttServerFactory = new MqttServerFactory();

                                    // The port for the default endpoint is 1883.
                                    // The default endpoint is NOT encrypted!
                                    // Use the builder classes where possible.
                                    var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();
                                    
                                    // The port can be changed using the following API (not used in this example).
                                    // new MqttServerOptionsBuilder()
                                    //     .WithDefaultEndpoint()
                                    //     .WithDefaultEndpointPort(1234)
                                    //     .Build();

                                    var mqttServer = mqttServerFactory.CreateMqttServer(mqttServerOptions);
                                 
                                    return mqttServer;
                              });

                              services.AddKeyedSingleton<IMqttClient>(BtnListener.BtnListenerMqttName, (opt, obj) => {
                                    var mqttClientFactory = new MqttClientFactory( );
                                    var mqttClient = mqttClientFactory.CreateMqttClient();
                                    return mqttClient;
                              });
                              services.AddKeyedSingleton<IMqttClient>(Cl330Controller.Cl330MqttName, (opt, obj) => {
                                    var mqttClientFactory = new MqttClientFactory();
                                    var mqttClient = mqttClientFactory.CreateMqttClient();
                                    return mqttClient;
                              });
                              services.AddKeyedSingleton<IMqttClient>(SwitchController.SwitchMqttName, (opt, obj) => {
                                    var mqttClientFactory = new MqttClientFactory();
                                    var mqttClient = mqttClientFactory.CreateMqttClient();
                                    return mqttClient;
                              });

                              services.AddSingleton<SwitchRule>();
                              services.AddSingleton<SwitchController>();
                              services.AddOptions<SwitchConfig>()
                                  .Bind(context.Configuration.GetSection(nameof(SwitchConfig)));


                              services.AddSingleton<BtnListener>();
                              services.AddSingleton<BtnStatus>();
                              services.AddHostedService< StartupService>();
                              services.AddOptions<VideoOpt>()
                                  .Bind(context.Configuration.GetSection("VideoOpt"));

                              services.AddSingleton<Cl330Controller>();
                              services.AddSingleton<CL330CommandBuilder>();
                              services.AddOptions<LightConfig>()
                                  .Bind(context.Configuration.GetSection(nameof(LightConfig)));

                              // 注册窗口（作为瞬态或单例，根据需求）
                              services.AddTransient<MainWindow>();
                              services.AddLogging(log=>log.AddConsole());

                        });

                  _host = hostBuilder.Build();
                
                  _ = _host.RunAsync();

                  // 手动创建主窗口并通过 DI 解析
                  var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                  mainWindow.Show();

                  base.OnStartup(e);
            }



        protected override void OnExit(ExitEventArgs e)
            {
                  _host?.Dispose();
                  base.OnExit(e);
            }
      }
     
}
