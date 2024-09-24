﻿using RDMSharp.Metadata.JSON.Converter;
using RDMSharp.RDM;
using System;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    [JsonConverter(typeof(OneOfTypesConverter))]
    public readonly struct OneOfTypes
    {
        public readonly CommonPropertiesForNamed ObjectType;
        public readonly BitFieldType BitFieldType { get; }
        public readonly BytesType BytesType { get; }
        public readonly BooleanType BooleanType { get; }
        public readonly IntegerType<byte> IntegerType_UInt8 { get; }
        public readonly IntegerType<sbyte> IntegerType_Int8 { get; }
        public readonly IntegerType<ushort> IntegerType_UInt16 { get; }
        public readonly IntegerType<short> IntegerType_Int16 { get; }
        public readonly IntegerType<uint> IntegerType_UInt32 { get; }
        public readonly IntegerType<int> IntegerType_Int32 { get; }
        public readonly IntegerType<ulong> IntegerType_UInt64 { get; }
        public readonly IntegerType<long> IntegerType_Int64 { get; }
#if NET7_0_OR_GREATER
        public readonly IntegerType<UInt128> IntegerType_UInt128 { get; }
        public readonly IntegerType<Int128> IntegerType_Int128 { get; }
#endif
        public readonly ReferenceType? ReferenceType { get; }
        public readonly ListType ListType { get; }
        public readonly CompoundType CompoundType { get; }
        public readonly StringType StringType { get; }
        public readonly PD_EnvelopeType PD_EnvelopeType { get; }

        public OneOfTypes(BitFieldType bitFieldType)
        {
            BitFieldType = bitFieldType;
            ObjectType = bitFieldType;
        }
        public OneOfTypes(BytesType bytesType)
        {
            BytesType = bytesType;
            ObjectType = bytesType;
        }
        public OneOfTypes(BooleanType booleanType)
        {
            BooleanType = booleanType;
            ObjectType = booleanType;
        }
        public OneOfTypes(IntegerType<byte> integerType_UInt8)
        {
            IntegerType_UInt8 = integerType_UInt8;
            ObjectType = integerType_UInt8;
        }
        public OneOfTypes(IntegerType<sbyte> integerType_Int8)
        {
            IntegerType_Int8 = integerType_Int8;
            ObjectType = integerType_Int8;
        }
        public OneOfTypes(IntegerType<ushort> integerType_UInt16)
        {
            IntegerType_UInt16 = integerType_UInt16;
            ObjectType = integerType_UInt16;
        }
        public OneOfTypes(IntegerType<short> integerType_Int16)
        {
            IntegerType_Int16 = integerType_Int16;
            ObjectType = integerType_Int16;
        }
        public OneOfTypes(IntegerType<uint> integerType_UInt32)
        {
            IntegerType_UInt32 = integerType_UInt32;
            ObjectType = integerType_UInt32;
        }
        public OneOfTypes(IntegerType<int> integerType_Int32)
        {
            IntegerType_Int32 = integerType_Int32;
            ObjectType = integerType_Int32;
        }
        public OneOfTypes(IntegerType<ulong> integerType_UInt64)
        {
            IntegerType_UInt64 = integerType_UInt64;
            ObjectType = integerType_UInt64;
        }
        public OneOfTypes(IntegerType<long> integerType_Int64)
        {
            IntegerType_Int64 = integerType_Int64;
            ObjectType = integerType_Int64;
        }
#if NET7_0_OR_GREATER
        public OneOfTypes(IntegerType<UInt128> integerType_UInt128)
        {
            IntegerType_UInt128 = integerType_UInt128;
            ObjectType = integerType_UInt128;
        }
        public OneOfTypes(IntegerType<Int128> integerType_Int128)
        {
            IntegerType_Int128 = integerType_Int128;
            ObjectType = integerType_Int128;
        }
#endif
        public OneOfTypes(ReferenceType referenceType)
        {
            ReferenceType = referenceType;
        }
        public OneOfTypes(StringType stringType)
        {
            StringType = stringType;
            ObjectType = stringType;
        }
        public OneOfTypes(ListType listType)
        {
            ListType = listType;
            ObjectType = listType;
        }
        public OneOfTypes(CompoundType compoundType)
        {
            CompoundType = compoundType;
            ObjectType = compoundType;
        }
        public OneOfTypes(PD_EnvelopeType pdEnvelopeType)
        {
            PD_EnvelopeType = pdEnvelopeType;
            ObjectType = pdEnvelopeType;
        }

        public bool IsEmpty()
        {
            return ObjectType == null && ReferenceType == null;
        }
        public PDL GetDataLength()
        {
            return ObjectType?.GetDataLength() ?? ReferenceType?.GetDataLength() ?? new PDL();
        }
        public byte[] ParsePayloadToData(DataTree dataTree)
        {
            CommonPropertiesForNamed objectType = ObjectType ?? ReferenceType?.ReferencedObject;
            var data = objectType.ParsePayloadToData(dataTree);

            if (GetDataLength().IsValid(data.Length))
                throw new ArithmeticException($"Parsed Data.Lenght not fits Calculated DataLength");

            return data;
        }
        public DataTree ParseDataToPayload(ref byte[] data)
        {
            CommonPropertiesForNamed objectType = ObjectType ?? ReferenceType?.ReferencedObject;

            if (GetDataLength().IsValid(data.Length))
                throw new ArithmeticException($"Parsed Data.Lenght not fits Calculated DataLength");

            return objectType.ParseDataToPayload(ref data);
        }

        public override string ToString()
        {
            return ObjectType?.ToString() ?? ReferenceType.ToString();
        }
    }
}