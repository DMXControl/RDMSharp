using System.Text.Json.Serialization;
using RDMSharp.Metadata.JSON.Converter;

namespace RDMSharp.Metadata.OneOfTypes
{
    [JsonConverter(typeof(OneOfTypesConverter))]
    public readonly struct OneOfTypes
    {
        public readonly BitType? BitType { get; }
        public readonly BitFieldType? BitFieldType { get; }
        public readonly BooleanType? BooleanType { get; }
        public readonly IntegerType? IntegerType { get; }
        public readonly ReferenceType? ReferenceType { get; }
        public readonly StringType? StringType { get; }

        public OneOfTypes(BitType bitType)
        {
            BitType = bitType;
        }

        public OneOfTypes(BitFieldType bitFieldType)
        {
            BitFieldType = bitFieldType;
        }

        public OneOfTypes(BooleanType booleanType)
        {
            BooleanType = booleanType;
        }
        public OneOfTypes(IntegerType integerType)
        {
            IntegerType = integerType;
        }

        public OneOfTypes(ReferenceType referenceType)
        {
            ReferenceType = referenceType;
        }

        public OneOfTypes(StringType stringType)
        {
            StringType = stringType;
        }

        public override string ToString()
        {
            if (BitType.HasValue)
                return BitType.Value.ToString();

            if (BitFieldType.HasValue)
                return BitFieldType.Value.ToString();

            if (BooleanType.HasValue)
                return BooleanType.Value.ToString();

            if (IntegerType.HasValue)
                return IntegerType.Value.ToString();

            if (ReferenceType.HasValue)
                return ReferenceType.Value.ToString();

            if (StringType.HasValue)
                return StringType.Value.ToString();

            return base.ToString();
        }
    }
}