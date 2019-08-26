using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    class ObjectComponentAction : ObjectComponentBase<ObjectComponentAction>
    {

        public Byte[] BodyBytes { get; set; }

        public ObjectComponentAction()
        {
            BodyBytes = new byte[DummyManager.GetDummyInfo.PacketSizeMax];
        }

        public override void Dispose()
        {
            Owner.Back(this);
        }
    }
}
