using System.Collections.Generic;
using System;
using System.Collections;

namespace Instances
{
    public class EventList<T> : List<T>
    {
        public new T this[int i]
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
        public new int Count { get { return ts.Count; } }

        

        public enum ArgsType { Add, Remove };
        public Action<T, ArgsType> onDataChanged;
        private List<T> ts;

        public EventList(List<T> ts)
        {
            this.ts = ts;
        }
        public new void Add(T item)
        {
            //base.Add(item);

            onDataChanged?.Invoke(item, ArgsType.Add);
            ClearAllAction();
        }
        public new bool Remove(T item)
        {
            onDataChanged?.Invoke(item, ArgsType.Remove);
            bool a = base.Remove(item);
            ClearAllAction();
            return a;
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

        
    }
}