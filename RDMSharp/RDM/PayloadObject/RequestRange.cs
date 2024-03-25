using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class RequestRange<T>: IRequestRange, IRequestRange<T>
    {

        object IRequestRange.Start => _start;

        object IRequestRange.End => _end;

        T IRequestRange<T>.Start => (T)Convert.ChangeType(_start, typeof(T));

        T IRequestRange<T>.End => (T)Convert.ChangeType(_end, typeof(T));

        private ulong _start;
        private ulong _end;

        public RequestRange(T start, T end)
        {
            _start = Convert.ToUInt64(start);
            _end = Convert.ToUInt64(end);
        }

        IEnumerable<T> IRequestRange<T>.ToEnumerator()
        {
            for (ulong i = _start; i <= _end; i++)
                yield return (T)Convert.ChangeType(i, typeof(T));
        }
        IEnumerable<object> IRequestRange.ToEnumerator()
        {
            for (ulong i = _start; i <= _end; i++)
                yield return (T)Convert.ChangeType(i, typeof(T));
        }
    }
    public interface IRequestRange<T>
    {
        T Start { get; }
        T End { get; }
        public IEnumerable<T> ToEnumerator();
    }
    public interface IRequestRange
    {
        object Start { get; }
        object End { get; }

        public IEnumerable<object> ToEnumerator();
    }
}