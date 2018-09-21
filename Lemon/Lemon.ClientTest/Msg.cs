using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LampMonitor
{
    /// <summary>
    /// 68 0A 94 29 01 17 00 00 00 20 09 00 00 00 00 00 00 00 00 00 70 16 开灯
    /// 68 0A 94 29 01 17 00 00 00 A0 09 00 00 00 00 00 00 00 00 00 F0 16 开灯回数据
    /// 68 0A 94 29 01 17 00 00 00 20 09 01 00 00 00 00 00 00 00 00 71 16 关灯
    /// 68 0A 94 29 01 17 00 00 00 A0 09 01 01 00 00 00 00 00 00 00 F2 16 关灯回数据
    /// </summary>
    public class Msg : IMessage
    {
        public byte Begin { get; set; }
        public byte Type { get; set; }
        public byte[] Address { get; set; }
        public byte ControllerCode { get; set; }
        public byte DataLength { get; set; }
        public byte[] Data { get; set; }
        public byte CheckCode { get; set; }
        public byte End { get; set; }

        public Msg()
        {
        }

        public static Msg GetOpenLampMsg(byte[] address)
        {
            byte[] data = new byte[9];
            Msg msg = new Msg()
            {
                Begin = 0x68,
                Type = 0x0A,
                Address = address,
                ControllerCode = 0x20,
                Data = data,
                End = 0x16
            };
            return msg;
        }

        public static Msg GetCloseLampMsg(byte[] address)
        {
            byte[] data = new byte[9];
            data[0] = 1;
            Msg msg = new Msg()
            {
                Begin = 0x68,
                Type = 0x0A,
                Address = address,
                ControllerCode = 0x20,
                Data = data,
                End = 0x16
            };
            return msg;
        }

        public static Msg GetOpenLampBackMsg(byte[] address)
        {
            byte[] data = new byte[9];
            Msg msg = new Msg()
            {
                Begin = 0x68,
                Type = 0x0A,
                Address = address,
                ControllerCode = 0xA0,
                Data = data,
                End = 0x16
            };
            return msg;
        }

        public static Msg GetStateLampMsg(byte[] address)
        {
            byte[] data = new byte[9];
            data[0] = 5;
            Msg msg = new Msg()
            {
                Begin = 0x68,
                Type = 0x0A,
                Address = address,
                ControllerCode = 0x20,
                Data = data,
                End = 0x16
            };
            return msg;
        }

        public int FromBytes(List<byte> bytes)
        {
            if (bytes.Count < 13)
                return 0;
            Begin = bytes[0];
            Type = bytes[1];
            Address = new byte[7];
            for (int i = 0; i < 7; i++)
            {
                Address[i] = bytes[i + 2];
            }
            ControllerCode = bytes[9];
            DataLength = bytes[10];

            int totalLength = DataLength + 13;
            if (bytes.Count < totalLength)
                return 0;

            Data = new byte[DataLength];
            for (int i = 0; i < DataLength; i++)
            {
                Data[i] = bytes[i + 11];
            }
            CheckCode = bytes[totalLength - 2];
            End = bytes[totalLength - 1];
            return totalLength;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(Begin);
            bytes.Add(Type);

            for (int i = 0; i < 7; i++)
            {
                bytes.Add(Address[i]);
            }

            bytes.Add(ControllerCode);

            DataLength = (byte)Data.Length;
            bytes.Add(DataLength);
            for (int i = 0; i < Data.Length; i++)
            {
                bytes.Add(Data[i]);
            }

            int totalLength = DataLength + 13;

            int checkCodeInt = 0;
            for (int i = 0; i < bytes.Count; i++)
            {
                checkCodeInt += bytes[i];
            }

            string jz = Convert.ToString(checkCodeInt, 2);
            string newjz = jz.Substring(jz.Length - 8);
            bytes.Add((byte)Convert.ToInt32(newjz, 2));
            bytes.Add(End);
            return bytes.ToArray();
        }
    }
}
