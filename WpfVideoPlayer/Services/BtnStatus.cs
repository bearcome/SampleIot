using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace WpfVideoPlayer.Services
{
      public class BtnStatus
      {
            // Message01020200017878  1通道按下
            // Message0102020000B9B8  1通道抬起
                        //010202010179E801820300A1
                        //011008970001B245
                        //0102020001787801820300A1
            /// <summary>
            /// 解析报文
            /// 上报单通道状态报文 
            /// </summary>
            /// <param name="data"></param>
            /// <returns>通道，开关状态</returns>
            public KeyValuePair<byte, bool>? ReadBtnStatus(byte[] data)
            {

#warning CRC 校验暂时dui
                  //var crcRes = Crc16.Compute(data.Take(data.Length - 2).ToArray());
                  //if (crcRes[0] != data[data.Length - 2] || crcRes[1] != data[data.Length - 1])
                  //      return null;

                  if (data[0] == 0x01 && data[1] == 0x02 && data[2] == 0x02)
                  {
                        return new KeyValuePair<byte, bool>(data[3], data[4] == 0x01);
                  }
                  return null;
            }
      }
}
