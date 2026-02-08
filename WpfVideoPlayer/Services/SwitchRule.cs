using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WpfVideoPlayer.Services
{
    public class SwitchRule
    {
            //例子通道12开
            public byte[] DemoOn = { 0xB2, 0xFF, 0xA1, 0x03, 0x00, 0x01, 0xA4, 0x2B };
            //例子通道1关                
            public byte[] DemoOff = { 0xB2, 0xFF, 0xA1, 0x01, 0x00, 0x00, 0xA1, 0x2B };

            
            //开启 1-8 一个bit 对应一个开关 可同时开多个
            public byte[] SwitchOn(byte flag)
            {
                  var order = new byte[DemoOn.Length];
                  DemoOn.CopyTo(order, 0);

                  order[3] = flag;
                  order[6] = (byte)(order[1] + order[2] + order[3] + order[4] + order[5]);
                  return order;
            }
            public byte[] SwitchOff(byte flag)
            {
                  var order = new byte[DemoOff.Length];
                  DemoOff.CopyTo(order, 0);

                  order[3] = flag;
                  order[6] = (byte)(order[1] + order[2] + order[3] + order[4] + order[5]);
                  return order;
            }
    }
}
