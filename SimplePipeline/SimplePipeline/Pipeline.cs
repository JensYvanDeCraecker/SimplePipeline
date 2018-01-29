using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    public class Pipeline<T> : IPipeline<T, T>, ICollection<IFilter<T, T>>
    {
        private readonly IList<IFilter<T, T>> filters = new List<IFilter<T, T>>();

        public Pipeline(IEnumerable<IFilter<T, T>> filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));
            foreach (IFilter<T, T> filter in filters)
                this.filters.Add(filter);
        }

        public Pipeline() { }

        public void Add(IFilter<T, T> filter)
        {
            filters.Add(filter);
        }

        public void Clear()
        {
            filters.Clear();
        }

        public Boolean Contains(IFilter<T, T> item)
        {
            return filters.Contains(item);
        }

        public void CopyTo(IFilter<T, T>[] array, Int32 arrayIndex)
        {
            filters.CopyTo(array, arrayIndex);
        }

        public Boolean Remove(IFilter<T, T> item)
        {
            return filters.Remove(item);
        }

        public Int32 Count
        {
            get
            {
                return filters.Count;
            }
        }

        Boolean ICollection<IFilter<T, T>>.IsReadOnly
        {
            get
            {
                return filters.IsReadOnly;
            }
        }

        public IEnumerator<IFilter<T, T>> GetEnumerator()
        {
            return filters.GetEnumerator();
        }

        IEnumerator<Object> IEnumerable<Object>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T Output { get; private set; }

        public Exception Exception { get; private set; }

        public Boolean IsBeginState
        {
            get
            {
                return Equals(Output, default(T)) && Equals(Exception, default(Exception));
            }
        }

        public Boolean Execute(T input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            try
            {
                Output = filters.Aggregate(input, (value, filter) => filter.Execute(value));
                return true;
            }
            catch (Exception e)
            {
                Exception = e;
                return false;
            }
        }

        public void Reset()
        {
            if (IsBeginState)
                return;
            Exception = default(Exception);
            Output = default(T);
        }
    }
}