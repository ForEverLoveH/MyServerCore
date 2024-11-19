using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.CRC
{
    public class CRCService
    {
        private static readonly ushort[] Table = new ushort[256];

        static CRCService()
        {
            const ushort polynomial = 0x1021;
            for (ushort i = 0; i < Table.Length; ++i)
            {
                ushort value = 0;
                var temp = i;
                for (var j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                Table[i] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ushort ComputeChecksum(byte[] bytes)
        {
            var crc = 0;
            foreach (var t in bytes)
            {
                var index = (byte)(crc ^ t);
                crc = (crc >> 8) ^ Table[index];
            }
            return (ushort)crc;
        }
    }
}
