using System;
using System.Collections.Concurrent;


namespace NPSBDummyLib
{
    public class ObjectPool<T> : Singleton<ObjectPool<T>>, IObjectPoolOwner<T>
        where T : ObjectComponentBase<T>, new()
    {
        private ConcurrentStack<T> ObjList;
        private Int32 IntervalSize;


        public ObjectPool()
        {
            ObjList = new ConcurrentStack<T>();
            Init(1000);
        }

        public override void Init(object param)
        {
            Clear();

            int size = (int)param;
            IntervalSize = (size / 2) + 1;
            Reserve(size);
        }

        public void Reserve(Int32 size)
        {
            Int32 cnt = 0;

            for (; cnt < size; ++cnt)
            {
                var obj = new T();
                if (obj == null)
                {
                    break;
                }

                obj.Init(this);
                ObjList.Push(obj);
            }

            if (cnt != size)
            {
                throw new Exception("Memory size is full!!");
            }
        }

        public T Get()
        {
            if (ObjList.Count == 0)
            {
                Reserve(IntervalSize);
            }

            T obj;
            if (!ObjList.TryPop(out obj))
            {
                return null;
            }

            return obj;
        }

        public void Back(ObjectComponentBase<T> obj)
        {
            if (obj != null)
            {
                ObjList.Push((T)obj);
            }
        }

        public void Clear()
        {
            ObjList.Clear();
        }
    }
}
