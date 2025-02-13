﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSerialize
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UInt32DataFieldAttribute : BaseDataFieldAttribute
    {
        public override uint DataFieldIndex { get; }
        public override uint DataFieldSize { get; }
        public override byte[] ConvertToBytes<T>(T dataFieldValue)
        {
            var dataFieldInt32 = Convert.ToInt32(dataFieldValue);

            var bytes = BitConverter.GetBytes(dataFieldInt32);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            var packageBytes = new byte[DataFieldSize];

            for (var i = 0; i < bytes.Length; i++)
            {
                if (bytes.Length > i)
                {
                    packageBytes[i] = bytes[i];
                }
            }

            return packageBytes;
        }

        public override T ConvertToObject<T>(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return (T)(object)BitConverter.ToUInt32(bytes, 0);
        }

        public override bool IsCollection { get; }
        public override uint SingleDataSize { get; }
        public override string CollectionSizePropertyName { get; }
        public override bool IsComplexObject { get; }

        public UInt32DataFieldAttribute(uint dataFieldIndex, uint dataFieldSize)
        {
            DataFieldIndex = dataFieldIndex;
            DataFieldSize = dataFieldSize;

            IsCollection = false;
            IsComplexObject = false;
        }
    }
}
