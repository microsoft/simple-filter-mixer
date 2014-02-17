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
            return GetEnumerator();
        }

        public FilterEnum GetEnumerator()
        {
            return new FilterEnum(_filters);
        }
    }

    public class FilterEnum : IEnumerator
    {
        public FilterListObject[] Filters;

        // Enumerators are positioned before the first element 
        // until the first MoveNext() call. 
        int _position = -1;

        public FilterEnum(FilterListObject[] list)
        {
            Filters = list;
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < Filters.Length);
        }

        public void Reset()
        {
            _position = -1;
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
                    return Filters[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
