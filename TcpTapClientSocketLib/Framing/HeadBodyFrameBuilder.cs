﻿using System;

namespace TcpTapClientSocketLib
{
    // 헤더 4바이트, 이중 2바이트는 Body 크기
    // Body Data

    public sealed class HeadBodyFrameBuilderBuilder : FrameBuilder
    {
        public HeadBodyFrameBuilderBuilder(bool isMasked = false)
            : this(new HeadBodyFrameEncoder(isMasked), new HeadBodyFrameDecoder(isMasked))
        {
        }

        public HeadBodyFrameBuilderBuilder(HeadBodyFrameEncoder encoder, HeadBodyFrameDecoder decoder)
            : base(encoder, decoder)
        {
        }
    }

    // Body 데이터 길이 데이터 선택적 크기 + Body 데이터 마스킹 가능
    public sealed class HeadBodyFrameEncoder : IFrameEncoder
    {
        private static readonly Random _rng = new Random(DateTime.UtcNow.Millisecond);
        private static readonly int MaskingKeyLength = 4;

        public HeadBodyFrameEncoder(bool isMasked = false)
        {
            IsMasked = isMasked;
        }

        public bool IsMasked { get; private set; }

        public void EncodeFrame(byte[] payload, int offset, int count, out byte[] frameBuffer, out int frameBufferOffset, out int frameBufferLength)
        {
            var buffer = Encode(payload, offset, count, IsMasked);

            frameBuffer = buffer;
            frameBufferOffset = 0;
            frameBufferLength = buffer.Length;
        }

        private static byte[] Encode(byte[] payload, int offset, int count, bool isMasked = false)
        {
            byte[] fragment;

            // Payload length:  7 bits, 7+16 bits, or 7+64 bits.
            // The length of the "Payload data", in bytes: 
            // if 0-125, that is the payload length.  
            // If 126, the following 2 bytes interpreted as a 16-bit unsigned integer are the payload length.  
            // If 127, the following 8 bytes interpreted as a 64-bit unsigned integer are the payload length.
            if (count < 126)
            {
                fragment = new byte[1 + (isMasked ? MaskingKeyLength : 0) + count];
                fragment[0] = (byte)count;
            }
            else if (count < 65536)
            {
                fragment = new byte[1 + 2 + (isMasked ? MaskingKeyLength : 0) + count];
                fragment[0] = (byte)126;
                fragment[1] = (byte)(count / 256);
                fragment[2] = (byte)(count % 256);
            }
            else
            {
                fragment = new byte[1 + 8 + (isMasked ? MaskingKeyLength : 0) + count];
                fragment[0] = (byte)127;

                int left = count;
                for (int i = 8; i > 0; i--)
                {
                    fragment[i] = (byte)(left % 256);
                    left = left / 256;

                    if (left == 0)
                        break;
                }
            }

            // Mask:  1 bit
            // Defines whether the "Payload data" is masked.
            if (isMasked)
                fragment[0] = (byte)(fragment[0] | 0x80);

            // Masking-key:  0 or 4 bytes
            // The masking key is a 32-bit value chosen at random by the client.
            if (isMasked)
            {
                int maskingKeyIndex = fragment.Length - (MaskingKeyLength + count);
                for (var i = maskingKeyIndex; i < maskingKeyIndex + MaskingKeyLength; i++)
                {
                    fragment[i] = (byte)_rng.Next(0, 255);
                }
                if (count > 0)
                {
                    int payloadIndex = fragment.Length - count;
                    for (var i = 0; i < count; i++)
                    {
                        fragment[payloadIndex + i] = (byte)(payload[offset + i] ^ fragment[maskingKeyIndex + i % MaskingKeyLength]);
                    }
                }
            }
            else
            {
                if (count > 0)
                {
                    int payloadIndex = fragment.Length - count;
                    Array.Copy(payload, offset, fragment, payloadIndex, count);
                }
            }

            return fragment;
        }
    }

    public sealed class HeadBodyFrameDecoder : IFrameDecoder
    {
        private static readonly int MaskingKeyLength = 4;

        public HeadBodyFrameDecoder(bool isMasked = false)
        {
            IsMasked = isMasked;
        }

        public bool IsMasked { get; private set; }

        public bool TryDecodeFrame(byte[] buffer, int offset, int count, out int frameLength, out byte[] payload, out int payloadOffset, out int payloadCount)
        {
            frameLength = 0;
            payload = null;
            payloadOffset = 0;
            payloadCount = 0;

            var frameHeader = DecodeHeader(buffer, offset, count);
            if (frameHeader != null && frameHeader.Length + frameHeader.PayloadLength <= count)
            {
                if (IsMasked)
                {
                    payload = DecodeMaskedPayload(buffer, offset, frameHeader.MaskingKeyOffset, frameHeader.Length, frameHeader.PayloadLength);
                    payloadOffset = 0;
                    payloadCount = payload.Length;
                }
                else
                {
                    payload = buffer;
                    payloadOffset = offset + frameHeader.Length;
                    payloadCount = frameHeader.PayloadLength;
                }

                frameLength = frameHeader.Length + frameHeader.PayloadLength;

                return true;
            }

            return false;
        }

        internal sealed class Header
        {
            public bool IsMasked { get; set; }
            public int PayloadLength { get; set; }
            public int MaskingKeyOffset { get; set; }
            public int Length { get; set; }

            public override string ToString()
            {
                return string.Format("IsMasked[{0}], PayloadLength[{1}], MaskingKeyOffset[{2}], Length[{3}]",
                    IsMasked, PayloadLength, MaskingKeyOffset, Length);
            }
        }

        private static Header DecodeHeader(byte[] buffer, int offset, int count)
        {
            if (count < 1)
                return null;

            // parse fixed header
            var header = new Header()
            {
                IsMasked = ((buffer[offset + 0] & 0x80) == 0x80),
                PayloadLength = (buffer[offset + 0] & 0x7f),
                Length = 1,
            };

            // parse extended payload length
            if (header.PayloadLength >= 126)
            {
                if (header.PayloadLength == 126)
                    header.Length += 2;
                else
                    header.Length += 8;

                if (count < header.Length)
                    return null;

                if (header.PayloadLength == 126)
                {
                    header.PayloadLength = buffer[offset + 1] * 256 + buffer[offset + 2];
                }
                else
                {
                    int totalLength = 0;
                    int level = 1;

                    for (int i = 7; i >= 0; i--)
                    {
                        totalLength += buffer[offset + i + 1] * level;
                        level *= 256;
                    }

                    header.PayloadLength = totalLength;
                }
            }

            // parse masking key
            if (header.IsMasked)
            {
                if (count < header.Length + MaskingKeyLength)
                    return null;

                header.MaskingKeyOffset = header.Length;
                header.Length += MaskingKeyLength;
            }

            return header;
        }

        private static byte[] DecodeMaskedPayload(byte[] buffer, int offset, int maskingKeyOffset, int payloadOffset, int payloadCount)
        {
            var payload = new byte[payloadCount];

            for (var i = 0; i < payloadCount; i++)
            {
                payload[i] = (byte)(buffer[offset + payloadOffset + i] ^ buffer[offset + maskingKeyOffset + i % MaskingKeyLength]);
            }

            return payload;
        }
    }
}