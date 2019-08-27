using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    public abstract class Singleton<T> where T : class, new()
    {
        protected static  T _instance;

        // 사용 전 중복 접근 방지를 위해 반드시 초기화
        public abstract void Init(object param);

        public static T GetInstance
        {
            get
            {
                if (null == _instance)
                    _instance = new T();
                
                return _instance;
            }
        }
    }
}
