using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    public interface IObjectPoolOwner<T>
    {
        // 이걸 T로 반환하면 ObjectPoolComponentBase.cs에서 Back(this)할 때 문제가 되서 번거롭지만 상위 클래스로 받게 함
        void Back(ObjectComponentBase<T> obj);
    }
}
