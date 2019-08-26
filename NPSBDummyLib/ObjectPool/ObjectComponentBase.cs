using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    public class ObjectComponentBase<T> : IDisposable
    {
        protected IObjectPoolOwner<T> Owner;

        public void Init(IObjectPoolOwner<T> owner)
        {
            Owner = owner;
        }

        public void Back()
        {
            Owner.Back(this);
        }

        public virtual void Dispose()
        {
        }
    }
}
