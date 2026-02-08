using Microsoft.Extensions.Options;
using System.Windows;
using WpfVideoPlayer.Services;


namespace WpfVideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
      public partial class MainWindow : Window
      {
            private ISerialService _serialService;
            private IOptions<VideoOpt> _videoOpt;

            public MainWindow(ISerialService serialService, IOptions<VideoOpt> videoOpt)
            {
                  _serialService = serialService;
                  _videoOpt = videoOpt;


                  InitializeComponent();


                  _serialService.RegistButOnTask(RedBut_Click, BlueBut_Click);

                  if (_videoOpt != null)
                  {
                        SetResource(_videoOpt.Value.Source0);
                  }
            }

            private Task RedBut_Click()
            {
                  SetResource( _videoOpt.Value.Source0);
                  return Task.CompletedTask;
            }

            private Task BlueBut_Click()
            {
                  SetResource( _videoOpt.Value.Source1);
                  return Task.CompletedTask;
            }


            private void SetResource(string src)
            {

                  this.Dispatcher.Invoke(() =>
                  {
                        // 设置播放资源（本地路径）
                        mediaEle.Source = new Uri(src);
                        // 自动开始播放（可选）
                        mediaEle.Play();
                  });
                 
            }

            /// <summary>
            /// 循环播放
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mediaEle_MediaEnded(object sender, RoutedEventArgs e)
            {
                  // 重置位置到开头
                  mediaEle.Position = TimeSpan.Zero;
                  // 重新播放
                  mediaEle.Play();
            }
      }
}