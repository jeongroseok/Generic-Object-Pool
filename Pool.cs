using System;
using System.Collections.Generic;

namespace Pooling
{
    public class Pool<T>
        where T : class
    {
        public interface IObjectHandler
        {
            T Create();
            void Destory(T poolObject);
            void Initialize(T poolObject);
            void Deinitialize(T poolObject);
        }

        public class PoolEventArgs : System.EventArgs
        {
            public T Value { get; private set; }
            public PoolEventArgs(T value)
            {
                this.Value = value;
            }
        }

        private IObjectHandler objectHandler;
        private Queue<T> queue = new Queue<T>();
        //private List<T> managed = new List<T>();

        public event EventHandler<PoolEventArgs> Created;
        public event EventHandler<PoolEventArgs> Initialized;
        public event EventHandler<PoolEventArgs> Deinitialized;

        public Pool(IObjectHandler objectHandler, int reserve = 10)
        {
            this.objectHandler = objectHandler;
            Reserve(reserve);
        }

        public void Reserve(int reserve)
        {
            if (queue.Count >= reserve)
            {
                return;
            }

            int d = reserve - queue.Count;
            for (int i = 0; i < d; i++)
            {
                Create();
            }
        }

        public T Pull()
        {
            if (queue.Count <= 0)
            {
                Create();
            }
            var obj = queue.Dequeue();
            objectHandler.Initialize(obj);
            if (Initialized != null)
            {
                Initialized(this, new PoolEventArgs(obj));
            }
            return obj;
        }

        public void Push(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (queue.Contains(instance))
            {
                throw new ArgumentException("duplicated", "instance");
            }
            objectHandler.Deinitialize(instance);
            if (Deinitialized != null)
            {
                Deinitialized(this, new PoolEventArgs(instance));
            }
            queue.Enqueue(instance);
        }

        public void Clear()
        {
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                objectHandler.Destory(current);
            }
        }

        private void Create()
        {
            var o = objectHandler.Create();
            queue.Enqueue(o);
            if (Created != null)
            {
                Created(this, new PoolEventArgs(o));
            }
            //managed.Add(o);
        }
    }
}