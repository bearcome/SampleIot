using System;
using System.Collections.Generic;
using System.Text;

namespace WpfVideoPlayer
{
      public static class Crc16
      {
            private static readonly ushort[] Table = new ushort[256];

            static Crc16()
            {
                  const ushort poly = 0xA001; // 0x8005 反转后的多项式
                  for (ushort i = 0; i < 256; i++)
                  {
                        ushort temp = i;
                        for (int j = 0; j < 8; j++)
                              temp = (temp & 1) == 1 ? (ushort)((temp >> 1) ^ poly) : (ushort)(temp >> 1);
                        Table[i] = temp;
                  }
            }

            public static byte[] Compute(byte[] data)
            {
                  ushort crc = 0xFFFF;
                  foreach (byte b in data)
                        crc = (ushort)((crc >> 8) ^ Table[(crc ^ b) & 0xFF]);
                  return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) }; // 小端序
            }
      }
}
