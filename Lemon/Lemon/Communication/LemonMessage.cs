using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Lemon.Communication
{
    public class LemonMessage : IMessage
    {
        public int HeaderLength { get; set; }
        public int BodyLength { get; set; }
        public int StateCode { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }

        public LemonMessage()
        {
            Header = "";
            Body = "";
        }

        public int FromBytes(List<byte> bytes)
        {
            if (bytes.Count < 12)
                return 0;

            byte[] bytesNew = bytes.ToArray();
            HeaderLength = BitConverter.ToInt32(bytesNew, 0);
            BodyLength = BitConverter.ToInt32(bytesNew, 4);
            StateCode = BitConverter.ToInt32(bytesNew, 8);

            int totalLength = HeaderLength + BodyLength + 12;
            if (bytes.Count < totalLength)
                return 0;
            Header = System.Text.Encoding.UTF8.GetString(bytesNew, 12, HeaderLength);
            Body = System.Text.Encoding.UTF8.GetString(bytesNew, 12 + HeaderLength, BodyLength);

            return totalLength;
        }

        public byte[] ToBytes()
        {
            List<byte> headerBytes = System.Text.Encoding.UTF8.GetBytes(Header).ToList();
            List<byte> bodyBytes = System.Text.Encoding.UTF8.GetBytes(Body).ToList();

            HeaderLength = headerBytes.Count;
            BodyLength = bodyBytes.Count;

            int totalLength = HeaderLength + BodyLength + 12;

            byte[] bytesNew = new byte[totalLength];
            byte[] headerLenthByte = BitConverter.GetBytes(HeaderLength);
            byte[] bodyLenthByte = BitConverter.GetBytes(BodyLength);
            byte[] codeByte = BitConverter.GetBytes(StateCode);
            Array.Copy(headerLenthByte, 0, bytesNew, 0, 4);
            Array.Copy(bodyLenthByte, 0, bytesNew, 4, 4);
            Array.Copy(codeByte, 0, bytesNew, 8, 4);

            Array.Copy(headerBytes.ToArray(), 0, bytesNew, 12, HeaderLength);
            Array.Copy(bodyBytes.ToArray(), 0, bytesNew, HeaderLength + 12, BodyLength);
            return bytesNew;
        }
    }
}
