using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    public class PacketUtil
    {
        public const Int16 PACKET_HEADER_SIZE = 5;

        public static byte[] StringToBytes(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            return data;
        }

        public static string FromPacketBodyData(byte[] packetData, Int32 bodySize)
        {
            return System.Text.Encoding.UTF8.GetString(packetData, PACKET_HEADER_SIZE, bodySize);
        }
    }

    public class SendPacketInfo
    {
        Random RandDataSize = new Random();
    
        public Int16 BufferSize;
        public byte[] BufferData;
        public Int16 BodySize;
        
        public void Init(int maxBodySize)
        {
            BufferData = new byte[PacketUtil.PACKET_HEADER_SIZE + maxBodySize];
        }

        public void SetData(int minBodySize, int maxBodySize)
        {
            try
            {
                var length = RandDataSize.Next(minBodySize, maxBodySize);
                var bodyData = PacketUtil.StringToBytes(Utils.RandomString(length));
                var bodySize = (Int16)bodyData.Length;


                // 패킷 전체 크기(2), 패킷id(2), 패킷타입(1), Body(padding)
                Int16 packetId = 101;

                BufferSize = (Int16)(PacketUtil.PACKET_HEADER_SIZE + bodySize);
                BodySize = bodySize;

                Buffer.BlockCopy(BitConverter.GetBytes(BufferSize), 0, BufferData, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(packetId), 0, BufferData, 2, 2);
                Buffer.BlockCopy(bodyData, 0, BufferData, 5, bodySize);
            }
            catch(Exception ex)
            {
                Utils.Logger.Error(ex.Message);
            }
        }

        public string BodyData()
        {
            return PacketUtil.FromPacketBodyData(BufferData, BodySize);
        }
    }


    public class RecvPacketInfo
    {
        public int BufferSize;
        public byte[] Buffer;

        public int BodySize;
        

        public void Init(int maxBodySize)
        {
            BufferSize = PacketUtil.PACKET_HEADER_SIZE + maxBodySize;
            Buffer = new byte[BufferSize];
        }

        public void Received(int recvSize)
        {
            Int16 packetSize = BitConverter.ToInt16(Buffer, 0);
            Int16 packetId = BitConverter.ToInt16(Buffer, 2);

            BodySize = packetSize - PacketUtil.PACKET_HEADER_SIZE;
        }

        public string BodyData()
        {
            return PacketUtil.FromPacketBodyData(Buffer, BodySize);
        }
    }

}
