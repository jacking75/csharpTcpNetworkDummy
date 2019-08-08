using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    public class SendPacketInfo
    {
        Random RandDataSize = new Random();
        public Int16 PACKET_HEADER_SIZE = 5;

        public Int16 BufferSize;
        public byte[] BufferData;
        public Int16 BodySize;
        //public byte[] BodyData;

        public void Init(int maxBodySize)
        {
            BufferData = new byte[PACKET_HEADER_SIZE + maxBodySize];
        }

        public void SetData(int minBodySize, int maxBodySize)
        {
            var length = RandDataSize.Next(minBodySize, maxBodySize);
            var bodyData = Encoding.Unicode.GetBytes(Utils.RandomString(length));
            var bodySize = (Int16)bodyData.Length;


            // 패킷 전체 크기(2), 패킷id(2), 패킷타입(1), Body(padding)
            Int16 packetId = 101;

            BufferSize = (Int16)(PACKET_HEADER_SIZE + bodySize);
            BodySize = bodySize;

            Buffer.BlockCopy(BitConverter.GetBytes(BufferSize), 0, BufferData, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(packetId), 0, BufferData, 2, 2);
            Buffer.BlockCopy(bodyData, 0, BufferData, 6, bodySize);
        }

        public string BodyData()
        {
            return System.Text.Encoding.UTF32.GetString(BufferData, PACKET_HEADER_SIZE, BodySize);
        }
    }


    public class RecvPacketInfo
    {
        public Int16 PACKET_HEADER_SIZE = 4;

        public int BufferSize;
        public byte[] Buffer;

        public int BodySize;
        //public byte[] BodyData;

        public void Init(int maxBodySize)
        {
            BufferSize = maxBodySize;
            Buffer = new byte[PACKET_HEADER_SIZE + maxBodySize];
        }

        public void Received(int recvSize)
        {
            Int16 packetSize = BitConverter.ToInt16(Buffer, 0);
            Int16 packetId = BitConverter.ToInt16(Buffer, 2);

            BodySize = packetSize - PACKET_HEADER_SIZE;
        }

        public string BodyData()
        {
            return System.Text.Encoding.UTF32.GetString(Buffer, PACKET_HEADER_SIZE, BodySize);
        }
    }

}
