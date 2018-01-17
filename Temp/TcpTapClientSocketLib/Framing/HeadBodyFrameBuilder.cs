using System;

namespace TcpTapClientSocketLib
{
    // 헤더 4바이트, 이중 2바이트는 Body 크기
    // Body Data

    public sealed class HeadBodyFrameBuilderBuilder : FrameBuilder
    {
        public HeadBodyFrameBuilderBuilder(bool isMasked = false)
            : this(new HeadBodyFrameEncoder(), new HeadBodyFrameDecoder())
        {
        }

        public HeadBodyFrameBuilderBuilder(HeadBodyFrameEncoder encoder, HeadBodyFrameDecoder decoder)
            : base(encoder, decoder)
        {
        }
    }

    public sealed class HeadBodyFrameEncoder : IFrameEncoder
    {
        public HeadBodyFrameEncoder()
        {
        }
                
        public void EncodeFrame(byte[] payload, int offset, int count, out byte[] frameBuffer, out int frameBufferOffset, out int frameBufferLength)
        {
            var buffer = Encode(payload, offset, count);

            frameBuffer = buffer;
            frameBufferOffset = 0;
            frameBufferLength = buffer.Length;
        }

        private static byte[] Encode(byte[] payload, int offset, int count)
        {
            byte[] fragment = null;
            
            if (count > 0)
            {
                fragment = new byte[count];
                Array.Copy(payload, offset, fragment, 0, count);
            }

            return fragment;
        }
    }

    public sealed class HeadBodyFrameDecoder : IFrameDecoder
    {
        public HeadBodyFrameDecoder()
        {
        }
        
        public bool TryDecodeFrame(byte[] buffer, int offset, int count, out int frameLength, out byte[] payload, out int payloadOffset, out int payloadCount)
        {
            frameLength = 0;
            payload = null;
            payloadOffset = 0;
            payloadCount = 0;

            var frameHeader = DecodeHeader(buffer, offset, count);
            if (frameHeader != null && (frameHeader.Length + Header.Size) <= count)
            {
                payload = buffer;
                payloadOffset = offset;
                payloadCount = Header.Size + frameHeader.Length;
                
                frameLength = frameHeader.Length + Header.Size;

                return true;
            }

            return false;
        }

        internal sealed class Header
        {
            public short PacketId { get; set; }
            public short Length { get; set; }

            public override string ToString()
            {
                return $"PacketId[{PacketId}], Length[{Length}]";
            }

            public static int Size = sizeof(short) + sizeof(short);
        }

        private static Header DecodeHeader(byte[] buffer, int offset, int count)
        {
            if (count < Header.Size)
                return null;

            var header = new Header();
            int offsetPos = offset;


            header.PacketId = BitConverter.ToInt16(buffer, offsetPos);
            offsetPos += sizeof(short);

            header.Length = BitConverter.ToInt16(buffer, offsetPos);
            offsetPos += sizeof(ushort);
            
            return header;
        }
                
    }
}
