using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pooling
{
    class Program
    {
        static void Main(string[] args)
        {
            Pool<Fruit> pool = new Pool<Fruit>(new FruitHandler(), 3);
            pool.Created += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.Value + "가 생성되었다.");
            };
            pool.Initialized += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.Value + "가 초기화 되었다.");
            };
            pool.Deinitialized += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.Value + "가 해제되었다.");
            };

            var fruit = pool.Pull();
            pool.Push(fruit);

            Queue<Fruit> q = new Queue<Fruit>();
            for (int i = 0; i < 5; i++)
            {
                q.Enqueue(pool.Pull());
            }

            while (q.Count > 0)
            {
                pool.Push(q.Dequeue());
            }

            pool.Clear();
        }
    }

    class FruitHandler : Pool<Fruit>.IObjectHandler
    {
        public Fruit Create()
        {
            var obj = new Fruit("사과");
            Console.WriteLine(obj.ToString() + " 생성");
            return obj;
        }

        public void Initialize(Fruit poolObject)
        {
            Console.WriteLine(poolObject.ToString() + " 초기화");
        }

        public void Deinitialize(Fruit poolObject)
        {
            Console.WriteLine(poolObject.ToString() + " 해제");
        }

        public void Destory(Fruit poolObject)
        {
            Console.WriteLine(poolObject.ToString() + " 파괴");
        }
    }

    class Fruit
    {
        private static int idCount = 0;
        public int ID { get; private set; }
        public string Name { get; private set; }

        public Fruit(string name)
        {
            this.ID = idCount++;
            this.Name = name;
        }

        public override string ToString()
        {
            return string.Format("[{0}번 {1}]", ID, Name);
        }
    }
}
