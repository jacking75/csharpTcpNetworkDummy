using CSBaseLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    public class ObjectComponentPacket : ObjectComponentBase<ObjectComponentPacket>
    {
        public EResultCode ResultCode { get; set; }
        public PACKETID PacketId { get; set; }
        
        public Byte[] BodyBytes { get; set; }

        public ObjectComponentPacket()
        {
            BodyBytes = new byte[DummyManager.GetDummyInfo.PacketSizeMax];
        }

        public override void Dispose()
        {
            Owner.Back(this);
        }
    }
}
