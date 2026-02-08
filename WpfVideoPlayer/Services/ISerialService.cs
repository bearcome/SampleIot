using System;
using System.Collections.Generic;
using System.Text;

namespace WpfVideoPlayer.Services
{
      /// <summary>
      /// 串口服务器  间接控制灯光、视频播放器、灯效、风扇等设备
      /// </summary>
      public interface ISerialService
      {
            void RegistButOnTask(Func<Task> redButTask, Func<Task> blueButTask);
            void RegistButOffTask(Func<Task> redButOffFunc, Func<Task> blueButOffFunc);

            Task ButDown(byte btnNO);
            Task ButUp(byte btnNO);

            Task Test();
      }

      public class SerialService : ISerialService
      {

            private Func<Task>? redButOnClicked = null;
            private Func<Task>? blueButOnClicked = null;

            private Func<Task>? redButOffClicked = null;
            private Func<Task>? blueButOffClicked = null;

            public SerialService()
            {
                  
            }

            public void RegistButOnTask(Func<Task> redButFunc, Func<Task> blueButFunc)
            {
                  redButOnClicked -= redButFunc;
                  redButOnClicked += redButFunc;
                  blueButOnClicked -= blueButFunc;
                  blueButOnClicked += blueButFunc;
            }
            public void RegistButOffTask(Func<Task> redButOffFunc, Func<Task> blueButOffFunc)
            {
                  redButOffClicked -= redButOffFunc;
                  redButOffClicked += redButOffFunc;
                  blueButOffClicked -= blueButOffFunc;
                  blueButOffClicked += blueButOffFunc;
            }

            /// <summary>
            /// 0x00 对应 红
            /// 0x01 对应 蓝
            /// </summary>
            /// <param name="btnNO"></param>
            public async Task ButDown(byte btnNO) {
                  if (btnNO == 0x00 )
                  {
                        await redButOnClicked?.Invoke();
                  }
                  else if (btnNO == 0x01)
                  {
                        await blueButOnClicked?.Invoke();
                  }
            }

            public async Task ButUp(byte btnNO)
            {
                  if (btnNO == 0x00)
                  {
                        await redButOffClicked?.Invoke();
                  }
                  else if (btnNO == 0x01)
                  {
                        await blueButOffClicked?.Invoke();
                  }
            }

            //test
            public async Task Test()
            {
                  await Task.Delay(10000);
                  await redButOnClicked?.Invoke();
                  await Task.Delay(10000);
                  await redButOffClicked?.Invoke();

                  await Task.Delay(10000);
                  await blueButOnClicked?.Invoke();
                  await Task.Delay(10000);
                  await blueButOffClicked?.Invoke();
            }
      }     
}
