using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Zicore.Collections.Generic
{
    public class LockFreeQueue<T>
    {
        public LockFreeQueue()
        {
            first = new Node();
            last = first;
        }

        int count = 0;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        Node first;
        Node last;

        public void Enqueue(T item)
        {
            Node newNode = new Node();
            newNode.item = item;
            Node old = Interlocked.Exchange(ref first, newNode);
            old.next = newNode;
            count++;
        }

        public bool Dequeue(out T item)
        {
            Node current;
            do
            {
                current = last;
                if (current.next == null)
                {
                    item = default(T);
                    return false;
                }
            }
            while (current != Interlocked.CompareExchange(ref last, current.next, current));
            item = current.next.item;
            current.next.item = default(T);
            count--;
            return true;
        }

        class Node
        {
            public T item;
            public Node next;
        }
    }
}
