using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SSMPUtils.Utils
{
    internal class NoRepeat<T> : IEnumerable
    {
        private List<T> _unused = new();
        private List<T> _used = new();
        private readonly Random random = new Random();

        public void Add(T item)
        {
            _unused.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _unused.GetEnumerator();
        }

        public T GetRandom()
        {
            if (_used.Count == 0 && _unused.Count <= 1)
            {
                if (_unused.Count == 0)
                {
                    throw new Exception("NoRepeat was empty, could not return a proper value.");
                }
                return _unused[0];
            }
            int i = random.Next(_unused.Count);
            var element = _unused[i];
            _unused.RemoveAt(i);
            if (_unused.Count == 0)
            {
                _unused.AddRange(_used);
                _used = new List<T>();
                //Log.LogInfo("Resetting NoRepeat");
            }
            _used.Add(element);

            return element;
        }
    }
}
