//****************************************************************************
// File: ByteReader.cs
// Author: Li Nan
// Date: 2024-01-07 12:00
// Version: 1.0
//****************************************************************************

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vena
{
    public struct ByteReader
    {
        private ReadOnlySequence<byte> sequence;
        
        public long Available => sequence.Length;
        public bool IsEmpty => sequence.IsEmpty;
        
        public ByteReader(ReadOnlySequence<byte> sequence)
        {
            this.sequence = sequence;
        }

        public byte ReadByte()
        {
            var data = sequence.First.Span[0];
            sequence = sequence.Slice(sequence.GetPosition(1));
            return data;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }
        
        public ushort ReadUInt16()
        {
            Span<byte> span = stackalloc byte[2];
            ReadToSpan(span, 2);
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar() => (char)ReadUInt16();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBoolean() => ReadByte() != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16() => (short)ReadUInt16();
        
        public uint ReadUInt32()
        {
            Span<byte> span = stackalloc byte[4];
            ReadToSpan(span, 4);
            return BinaryPrimitives.ReadUInt32LittleEndian( span );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32() => (int)ReadUInt32();
        
        public ulong ReadUInt64()
        {
            Span<byte> span = stackalloc byte[8];
            ReadToSpan(span, 8);
            return BinaryPrimitives.ReadUInt64LittleEndian( span );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64() => (long)ReadUInt64();

        
        public float ReadSingle()
        {
            var converter = new UIntFloat {
                intValue = ReadUInt32()
            };
            return converter.floatValue;
        }

        public double ReadDouble()
        {
            var converter = new UIntDouble {
                longValue = ReadUInt64()
            };
            return converter.doubleValue;
        }

        public decimal ReadDecimal()
        {
            var converter = new UIntDecimal
            {
                longValue1 = ReadUInt64(),
                longValue2 = ReadUInt64()
            };
            return converter.decimalValue;
        }

        public void ReadToSpan(Span<byte> bytes, int count)
        {
            var readOnlySequence = ReadSequence(count);
            readOnlySequence.CopyTo(bytes);
        }

        // read bytes into the passed buffer
        public byte[] ReadBytes(byte[] bytes, int count)
        {
            // check if passed byte array is big enough
            if (count > bytes.Length)
                throw new EndOfStreamException("ReadBytes can't read " + count +
                                               " + bytes because the passed byte[] only has length " + bytes.Length);
            var readOnlySequence = ReadSequence(count);
            readOnlySequence.CopyTo(bytes);
            return bytes;
        }

        // useful to parse payloads etc. without allocating
        public ReadOnlySequence<byte> ReadSequence(int count)
        {
            var result = sequence.Slice(0, count);
            sequence = sequence.Slice(count);
            return result;
        }
        
        public override string ToString()
        {
            return $"BinaryReader sequence: {sequence.ToString()}";
        }
        
        public string ReadString()
        {
            var size = ReadUInt16();
            if (size == 0)
                return null;

            var realSize = size - 1;

            if (realSize >= ByteWriter.MaxStringLength)
                throw new EndOfStreamException("ReadString too long: " + realSize + ". Limit is: " +
                                               ByteWriter.MaxStringLength);

            var unreadSpan = sequence.First.Span;
            if (unreadSpan.Length >= realSize)
            {
                var value = Encoding.UTF8.GetString(unreadSpan.Slice(0, realSize));
                sequence = sequence.Slice(realSize);
                return value;
            }

            return ReadStringSlow(realSize);
        }

        private string ReadStringSlow(int byteLength)
        {
            // We need to decode bytes incrementally across multiple spans.
            var maxCharLength = Encoding.UTF8.GetMaxCharCount(byteLength);
            var charArray = ArrayPool<char>.Shared.Rent(maxCharLength);
            try
            {
                var decoder = Encoding.UTF8.GetDecoder();

                var remainingByteLength = byteLength;
                var initializedChars = 0;
                while (remainingByteLength > 0)
                {
                    var unreadSpan = sequence.First.Span;
                    var bytesRead = System.Math.Min(remainingByteLength, unreadSpan.Length);
                    remainingByteLength -= bytesRead;
                    var flush = remainingByteLength == 0;
#if NETCOREAPP
                    initializedChars +=
                    decoder.GetChars(unreadSpan.Slice(0, bytesRead), charArray.AsSpan(initializedChars), flush);
#else
                    unsafe
                    {
                        fixed (byte* pUnreadSpan = unreadSpan)
                        fixed (char* pCharArray = &charArray[initializedChars])
                        {
                            initializedChars += decoder.GetChars(pUnreadSpan, bytesRead, pCharArray,
                                charArray.Length - initializedChars, flush);
                        }
                    }
#endif
                    sequence = sequence.Slice(bytesRead);
                }
                
                var value = new string(charArray, 0, initializedChars);
                return value;
            }
            catch
            {
                throw;
            }
            finally
            {
                ArrayPool<char>.Shared.Return(charArray);
            }
        }

        public byte[] ReadBytesAndSize()
        {
            var count = ReadUInt32();
            return count == 0 ? null : ReadBytes(checked((int)(count - 1u)));
        }

        public ReadOnlySequence<byte> ReadSizeAndSequence()
        {
            var count = ReadUInt32();
            return count == 0 ? default : ReadSequence(checked((int)(count - 1u)));
        }
        
        public byte[] ReadBytes(int count)
        {
            var bytes = new byte[count];
            ReadBytes(bytes, count);
            return bytes;
        }
    }

}