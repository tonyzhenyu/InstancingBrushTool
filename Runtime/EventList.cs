using System.Collections.Generic;
using System;
using System.Collections;

namespace Instances
{
    public class EventList<T> : IList<T>
    {
        public T this[int i]
        {
            get
            {
                return ts[i];
            }
            set
            {
                ts[i] = value;
            }
        }
        public int Count { get { return ts.Count; } }

        public bool IsReadOnly => throw new NotImplementedException();

        public enum ArgsType { Add, Remove };
        public Action<T, ArgsType> onDataChanged;
        private List<T> ts;

        public EventList(List<T> ts)
        {
            this.ts = ts;
        }
        public void Add(T item)
        {
            ts.Add(item);
            onDataChanged?.Invoke(item, ArgsType.Add);
            ClearAllAction();
        }
        public bool Remove(T item)
        {
            ts.Remove(item);
            onDataChanged?.Invoke(item, ArgsType.Remove);
            ClearAllAction();
            return true;
        }
        private void ClearAllAction()
        {
            if (onDataChanged == null)
            {
                return;
            }
            Delegate[] delegates = onDataChanged.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++)
            {
                onDataChanged -= delegates[i] as Action<T, ArgsType>;
            }

            if (onDataChanged != null)
            {
                //onDataChanged.Invoke(T);
            }
            else
            {
                //Debug.Log("Clear Delegates");
            }
        }
        public T[] ToArray()
        {
            return ts.ToArray();
        }

        public int IndexOf(T item)
        {
            return ts.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ts.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ts.RemoveAt(index);
        }
        public void Clear()
        {
            ts.Clear();
        }

        public bool Contains(T item)
        {
            return ts.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ts.GetEnumerator();
        }
    }
}