//****************************************************************************
// File: ByteWriter.cs
// Author: Li Nan
// Date: 2024-01-07 12:00
// Version: 1.0
//****************************************************************************
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vena
{
    public readonly struct ByteWriter
    {
        public const int MaxStringLength = 1024 * 32;
        static readonly byte[] stringBuffer = new byte[MaxStringLength];
        
        private readonly IBufferWriter<byte> bufferWriter;
        
        public ByteWriter(in IBufferWriter<byte> bufferWriter )
        {
            this.bufferWriter = bufferWriter;
        }

        public void WriteByte(byte value)
        {
            var span = bufferWriter.GetSpan( 1 );
            span[0] = value;
            bufferWriter.Advance( 1 );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value) => WriteByte((byte)value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value)
        {
            WriteByte((byte)value);
            WriteByte((byte)(value >> 8));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(short value) => WriteUInt16((ushort)value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteChar(char value) => WriteUInt16(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(bool value) => WriteByte((byte)(value ? 1 : 0));
        
        public void WriteUInt32(uint value)
        {
            var span = bufferWriter.GetSpan( 4 );
            BinaryPrimitives.WriteUInt32LittleEndian( span, value );
            bufferWriter.Advance( 4 );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value) => WriteUInt32((uint)value);

        public void WriteUInt64(ulong value)
        {
            var span = bufferWriter.GetSpan( 8 );
            BinaryPrimitives.WriteUInt64LittleEndian(span, value);
            bufferWriter.Advance( 8 );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value) => WriteUInt64((ulong)value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSingle(float value)
        {
            var converter = new UIntFloat { floatValue = value };
            WriteUInt32(converter.intValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value)
        {
            var converter = new UIntDouble { doubleValue = value };
            WriteUInt64(converter.longValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDecimal(decimal value)
        {
            var converter = new UIntDecimal { decimalValue = value };
            WriteUInt64(converter.longValue1);
            WriteUInt64(converter.longValue2);
        }

        public void WriteString(string value)
        {
            if (value == null)
            {
                WriteUInt16(0);
                return;
            }

            // convert to byte[]
            int size = Encoding.UTF8.GetBytes(value, 0, value.Length, stringBuffer, 0);

            // check if within max size
            if (size >= MaxStringLength)
            {
                throw new IndexOutOfRangeException("ByteWrite.WriteString(string) too long: " + size + ". Limit: " +
                                                   MaxStringLength);
            }
            
            // write size and bytes
            WriteUInt16(checked((ushort)(size + 1)));
            WriteBytes(stringBuffer, 0, size);
        }
        
                
        public void WriteSpan(in ReadOnlySpan<byte> span )
        {
            var toSpan = bufferWriter.GetSpan( span.Length );
            span.CopyTo( toSpan );
            bufferWriter.Advance( span.Length );
        }
        
        public void WriteBytes(byte[] bytes, int offset, int count)
        {
            var fromSpan = bytes.AsSpan( offset, count );
            var toSpan = bufferWriter.GetSpan( count );
            fromSpan.CopyTo( toSpan );
            bufferWriter.Advance(count);
        }
        
        public void WriteBytesWithSize(byte[] buffer)
        {
            // buffer might be null, so we can't use .Length in that case
            WriteBytesWithSize(buffer, 0, buffer?.Length ?? 0);
        }
        
        public void WriteBytesWithSize(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                WriteUInt32(0u);
                return;
            }
            
            WriteUInt32(checked((uint)count) + 1u);
            WriteBytes(bytes, offset, count);
        }
        
        public void WriteSequence(in ReadOnlySequence<byte> sequence)
        {
            foreach( var readOnly in sequence )
            {
                var fromSpan = readOnly.Span;
                var toSpan = bufferWriter.GetSpan( fromSpan.Length );
                fromSpan.CopyTo( toSpan );
                bufferWriter.Advance( fromSpan.Length );
            }
        }
        
        public void WriteSequenceWithSize(ReadOnlySequence<byte> sequence)
        {
            if (sequence.IsEmpty)
            {
                WriteUInt32(0u);
                return;
            }

            WriteUInt32(checked((uint)sequence.Length) + 1u);
            WriteSequence(sequence);
        }

        public void WriteSegmentWithSize(ArraySegment<byte> segment)
        {
            WriteBytesWithSize(segment.Array, segment.Offset, segment.Count);
        }
    }
}