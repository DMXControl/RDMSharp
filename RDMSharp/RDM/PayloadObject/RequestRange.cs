using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class RequestRange<T>
    {
        public readonly T Start;
        public readonly T End;

        public RequestRange(T start, T end)
        {
            this.Start = start;
            this.End = end;
        }

        public IEnumerable<T> ToEnumerator()
        {
            ulong start = Convert.ToUInt64(this.Start);
            ulong end = Convert.ToUInt64(this.End);
            for (ulong i = start; i <= end; i++)
                yield return (T)Convert.ChangeType(i, typeof(T));
        }
    }
}