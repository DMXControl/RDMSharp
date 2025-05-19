using System;
using System.Linq;

namespace RDMSharp.RDM
{
    public readonly struct PDL
    {
        public const uint MAX_LENGTH = 0xE7 * 0xFF;
        public readonly uint? Value { get; }
        public readonly uint? MinLength { get; }
        public readonly uint? MaxLength { get; }

        public PDL()
        {
            Value = 0;
        }
        public PDL(uint value) : this()
        {
            if (value >= MAX_LENGTH)
                throw new ArgumentOutOfRangeException($"The Parameter {nameof(value)} should be in range of 0 - {MAX_LENGTH}");

            Value = value;
        }
        public PDL(uint minLength, uint maxLength) : this()
        {
            if (minLength > MAX_LENGTH)
                throw new ArgumentOutOfRangeException($"The Parameter {nameof(minLength)} should be in range of 0 - {MAX_LENGTH}");
            if (maxLength > MAX_LENGTH)
                throw new ArgumentOutOfRangeException($"The Parameter {nameof(maxLength)} should be in range of 0 - {MAX_LENGTH}");

            if (minLength == maxLength)
                Value = minLength;
            else
            {
                MinLength = Math.Min(minLength, maxLength);
                MaxLength = Math.Max(minLength, maxLength);
                Value = null;
            }
        }
        public PDL(params PDL[] pdls)
        {
            uint value = 0, min = 0, max = 0;

            foreach (PDL pdl in pdls.Where(p => p.Value.HasValue))
                value += pdl.Value.Value;

            foreach (PDL pdl in pdls.Where(p => p.MinLength.HasValue))
                min += pdl.MinLength.Value;
            foreach (PDL pdl in pdls.Where(p => p.MaxLength.HasValue))
                max += pdl.MaxLength.Value;

            if (min == max)
                Value = Math.Min(MAX_LENGTH, value + min);
            else
            {
                MinLength = Math.Min(MAX_LENGTH, value + Math.Min(min, max));
                MaxLength = Math.Min(MAX_LENGTH, value + Math.Max(min, max));
                Value = null;
            }
        }

        public bool IsValid(int length)
        {
            if (Value.HasValue)
                return Value == length;

            return MinLength <= length && length <= MaxLength;
        }
    }
}
