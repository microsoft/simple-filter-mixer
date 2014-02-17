using System;
using System.Collections;

namespace ImagingSDKSamples
{
    public class FilterListObject : IEnumerable
    {
        public string Name { get; set; }
        public object[] Constructor { get; set; }

        private FilterListObject[] _filters;

        public FilterListObject()
        {

        }

        public FilterListObject(FilterListObject[] pArray)
        {
            _filters = new FilterListObject[pArray.Length];

            for (int i = 0; i < pArray.Length; i++)
            {
                _filters[i] = pArray[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public FilterEnum GetEnumerator()
        {
            return new FilterEnum(_filters);
        }
    }

    public class FilterEnum : IEnumerator
    {
        public FilterListObject[] _filters;

        // Enumerators are positioned before the first element 
        // until the first MoveNext() call. 
        int position = -1;

        public FilterEnum(FilterListObject[] list)
        {
            _filters = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _filters.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public FilterListObject Current
        {
            get
            {
                try
                {
                    return _filters[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
