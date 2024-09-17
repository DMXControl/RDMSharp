﻿using System;
using System.Text.Json.Serialization;
using RDMSharp.Metadata.JSON.Converter;

namespace RDMSharp.Metadata.OneOfTypes
{
    [JsonConverter(typeof(OneOfTypesConverter))]
    public readonly struct OneOfTypes
    {
        public readonly BitType? BitType { get; }
        public readonly BitFieldType? BitFieldType { get; }
        public readonly BytesType? BytesType { get; }
        public readonly BooleanType? BooleanType { get; }
        public readonly IntegerType<byte>? IntegerType_UInt8 { get; }
        public readonly IntegerType<sbyte>? IntegerType_Int8 { get; }
        public readonly IntegerType<UInt16>? IntegerType_UInt16 { get; }
        public readonly IntegerType<Int16>? IntegerType_Int16 { get; }
        public readonly IntegerType<UInt32>? IntegerType_UInt32 { get; }
        public readonly IntegerType<Int32>? IntegerType_Int32 { get; }
        public readonly IntegerType<UInt64>? IntegerType_UInt64 { get; }
        public readonly IntegerType<Int64>? IntegerType_Int64 { get; }
#if NET7_0_OR_GREATER
        public readonly IntegerType<UInt128>? IntegerType_UInt128 { get; }
        public readonly IntegerType<Int128>? IntegerType_Int128 { get; }
#endif
        public readonly ReferenceType? ReferenceType { get; }
        public readonly ListType? ListType { get; }
        public readonly CompoundType? CompoundType { get; }
        public readonly StringType? StringType { get; }
        public readonly PD_EnvelopeType? PD_EnvelopeType { get; }

        public OneOfTypes(BitType bitType)
        {
            BitType = bitType;
        }
        public OneOfTypes(BitFieldType bitFieldType)
        {
            BitFieldType = bitFieldType;
        }
        public OneOfTypes(BytesType bytesType)
        {
            BytesType = bytesType;
        }
        public OneOfTypes(BooleanType booleanType)
        {
            BooleanType = booleanType;
        }
        public OneOfTypes(IntegerType<byte> integerType_UInt8)
        {
            IntegerType_UInt8 = integerType_UInt8;
        }
        public OneOfTypes(IntegerType<sbyte> integerType_Int8)
        {
            IntegerType_Int8 = integerType_Int8;
        }
        public OneOfTypes(IntegerType<UInt16> integerType_UInt16)
        {
            IntegerType_UInt16 = integerType_UInt16;
        }
        public OneOfTypes(IntegerType<Int16> integerType_Int16)
        {
            IntegerType_Int16 = integerType_Int16;
        }
        public OneOfTypes(IntegerType<UInt32> integerType_UInt32)
        {
            IntegerType_UInt32 = integerType_UInt32;
        }
        public OneOfTypes(IntegerType<Int32> integerType_Int32)
        {
            IntegerType_Int32 = integerType_Int32;
        }
        public OneOfTypes(IntegerType<UInt64> integerType_UInt64)
        {
            IntegerType_UInt64 = integerType_UInt64;
        }
        public OneOfTypes(IntegerType<Int64> integerType_Int64)
        {
            IntegerType_Int64 = integerType_Int64;
        }
#if NET7_0_OR_GREATER
        public OneOfTypes(IntegerType<UInt128> integerType_UInt128)
        {
            IntegerType_UInt128 = integerType_UInt128;
        }
        public OneOfTypes(IntegerType<Int128> integerType_Int128)
        {
            IntegerType_Int128 = integerType_Int128;
        }
#endif
        public OneOfTypes(ReferenceType referenceType)
        {
            ReferenceType = referenceType;
        }
        public OneOfTypes(StringType stringType)
        {
            StringType = stringType;
        }
        public OneOfTypes(ListType listType)
        {
            ListType = listType;
        }
        public OneOfTypes(CompoundType compoundType)
        {
            CompoundType = compoundType;
        }
        public OneOfTypes(PD_EnvelopeType pdEnvelopeType)
        {
            PD_EnvelopeType = pdEnvelopeType;
        }

        public override string ToString()
        {
            if (BitType.HasValue)
                return BitType.Value.ToString();

            if (BitFieldType.HasValue)
                return BitFieldType.Value.ToString();

            if (BytesType.HasValue)
                return BytesType.Value.ToString();

            if (BooleanType.HasValue)
                return BooleanType.Value.ToString();

            if (IntegerType_UInt8.HasValue)
                return IntegerType_UInt8.Value.ToString();
            if (IntegerType_UInt8.HasValue)
                return IntegerType_UInt8.Value.ToString();

            if (IntegerType_UInt16.HasValue)
                return IntegerType_UInt16.Value.ToString();
            if (IntegerType_UInt16.HasValue)
                return IntegerType_UInt16.Value.ToString();

            if (IntegerType_UInt32.HasValue)
                return IntegerType_UInt32.Value.ToString();
            if (IntegerType_UInt32.HasValue)
                return IntegerType_UInt32.Value.ToString();

            if (IntegerType_UInt64.HasValue)
                return IntegerType_UInt64.Value.ToString();
            if (IntegerType_UInt64.HasValue)
                return IntegerType_UInt64.Value.ToString();
#if NET7_0_OR_GREATER
            if (IntegerType_UInt128.HasValue)
                return IntegerType_UInt128.Value.ToString();
            if (IntegerType_UInt128.HasValue)
                return IntegerType_UInt128.Value.ToString();
#endif

            if (ReferenceType.HasValue)
                return ReferenceType.Value.ToString();

            if (StringType.HasValue)
                return StringType.Value.ToString();

            if (ListType != null)
                return ListType.ToString();

            if (CompoundType != null)
                return CompoundType.ToString();

            if (PD_EnvelopeType.HasValue)
                return PD_EnvelopeType.Value.ToString();

            return base.ToString();
        }
    }
}