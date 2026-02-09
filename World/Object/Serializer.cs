using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XDTGame.Core;

public interface ISerializer : IDisposable
{
    #region Reader

    byte ReadByte();
    
    short ReadShort();
     
    int ReadInt32();
    
    uint ReadUInt32();
    
    long ReadInt64();
    
    ulong ReadUInt64();
    
    bool ReadBoolean();
    
    float ReadSingle();
    
    string ReadString();
    
    int ReadBytes(byte[] bytes, int startIndex);
    
    #endregion
    
    #region Writer
    
    void WriteByte(byte value);
    
    void WriteShort(short value);

    void WriteInt32(int value);
    
    void WriteUInt32(uint value);
    
    void WriteInt64(long value);
    
    void WriteUInt64(ulong value);
    
    void WriteBoolean(bool value);
    
    void WriteSingle(float value);
    
    void WriteString(string value);
    
    void WriteBytes(byte[] bytes, int startIndex, int length);
    
    #endregion
}

public struct Serializer : ISerializer
{
    static readonly ArrayPool<byte> _bufferPool = ArrayPool<byte>.Create(1024, 1024 * 1024);
    
    static byte[] _tmpBuffer = new byte[8];
    
    private byte[] _buffer;
    
    private int _start, _end;
    
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining) ]
        get => _end - _start;
    }
    
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining) ]
        get => null == _buffer || _start == _end;
    }

    public Serializer()
    {
        _buffer = _bufferPool.Rent(512);
        
        _start = 0;
        
        _end = 0;
    }

    public Serializer(byte[] buffer)
    {
        if (null == buffer)
            throw new ArgumentNullException(nameof(buffer), "buffer is null.");
        
        _buffer = null;
        
        _start = _end = 0;
        
        WriteBuffer(buffer, buffer.Length);
    }
    
    public Serializer(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath), "filePath is null or empty.");
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException("file not found.", filePath);
        
        _buffer = null;
        
        _start = _end = 0;
        
        byte[] bytes = File.ReadAllBytes(filePath);
        
        WriteBuffer(bytes, bytes.Length);
    }
    
    public byte[] GetBuffer()
    {
        return _buffer ?? Array.Empty<byte>();
    }
    
    private void WriteBuffer(byte[] bytes, int count)
    {
        if (null == _buffer || _end + count > _buffer.Length)
        {
            int newSize = Math.Max(_buffer?.Length ?? 0, 512);
            
            while (_end + count > newSize)
            {
                newSize *= 2;
            }
            
            byte[] newBuffer = _bufferPool.Rent(newSize);
            
            if(null != _buffer)
                Array.Copy(_buffer, 0, newBuffer, 0, _end);
            
            if(null != _buffer)
                _bufferPool.Return(_buffer);
            
            _buffer = newBuffer;
        }
        
        Array.Copy(bytes, 0, _buffer, _end, count);
        
        _end += count;
    }
    
    private void ReadBuffer(byte[] bytes, int count)
    {
        if(IsEmpty)
            throw new InvalidOperationException( "read buffer is empty.");
        
        if(_start + count > _end)
            throw new InvalidOperationException( "read buffer overflow.");
        
        Array.Copy(_buffer, _start, bytes, 0, count);
        
        _start += count;
    }
    
    public void Dispose()
    {
        // TODO release managed resources here
        if (null != _buffer)
        {
            _bufferPool.Return(_buffer);
            
            _buffer = null;
        }
    }

    public byte ReadByte()
    {
        ReadBuffer(_tmpBuffer, 1);
        
        return _tmpBuffer[0];
    }

    public short ReadShort()
    {
        ReadBuffer(_tmpBuffer, 2);
        
        return new ByteConvert()
        {
            b0 = _tmpBuffer[0],
            b1 = _tmpBuffer[1]
        }.s;
    }

    public int ReadInt32()
    {
        ReadBuffer(_tmpBuffer, 4);
        
        return new ByteConvert()
        {
            b0 = _tmpBuffer[0],
            b1 = _tmpBuffer[1],
            b2 = _tmpBuffer[2],
            b3 = _tmpBuffer[3]
        }.i;
    }
    
    public uint ReadUInt32()
    {
        ReadBuffer(_tmpBuffer, 4);
        
        return new ByteConvert()
        {
            b0 = _tmpBuffer[0],
            b1 = _tmpBuffer[1],
            b2 = _tmpBuffer[2],
            b3 = _tmpBuffer[3]
        }.ui;
    }

    public long ReadInt64()
    {
        ReadBuffer(_tmpBuffer, 8);
        
        return new ByteConvert()
        {
            b0 = _tmpBuffer[0],
            b1 = _tmpBuffer[1],
            b2 = _tmpBuffer[2],
            b3 = _tmpBuffer[3],
            b4 = _tmpBuffer[4],
            b5 = _tmpBuffer[5],
            b6 = _tmpBuffer[6],
            b7 = _tmpBuffer[7]
        }.l;
    }

    public ulong ReadUInt64()
    {
        ReadBuffer(_tmpBuffer, 8);
        
        return new ByteConvert()
        {
            b0 = _tmpBuffer[0],
            b1 = _tmpBuffer[1],
            b2 = _tmpBuffer[2],
            b3 = _tmpBuffer[3],
            b4 = _tmpBuffer[4],
            b5 = _tmpBuffer[5],
            b6 = _tmpBuffer[6],
            b7 = _tmpBuffer[7]
        }.ul;
    }

    public bool ReadBoolean()
    {
        ReadBuffer(_tmpBuffer, 1);
        
        return new ByteConvert()
        {
            b0 = _tmpBuffer[0]
        }.bl;
    }

    public float ReadSingle()
    {
        ReadBuffer(_tmpBuffer, 4);
        
        return new ByteConvert()
        {
            b0 = _tmpBuffer[0],
            b1 = _tmpBuffer[1],
            b2 = _tmpBuffer[2],
            b3 = _tmpBuffer[3]
        }.f;
    }

    public string ReadString()
    {
        short len = ReadShort();
        if (len <= 0)
            return string.Empty;
        
        if (_tmpBuffer.Length < len)
            _tmpBuffer = new byte[len];
        
        ReadBuffer(_tmpBuffer, len);
        
        return System.Text.Encoding.UTF8.GetString(_tmpBuffer, 0, len);
    }

    public int ReadBytes(byte[] bytes, int startIndex)
    {
        short len = ReadShort();
        
        if (len <= 0)
            return 0;
        
        if (null == bytes || startIndex < 0 || startIndex + len > bytes.Length)
            throw new ArgumentOutOfRangeException(nameof(bytes), "buffer is null or out of range.");
        
        ReadBuffer(bytes, len);
        
        return len;
    }

    public void WriteByte(byte value)
    {
        _tmpBuffer[0] = value;
        WriteBuffer(_tmpBuffer, 1);
    }

    public void WriteShort(short value)
    {
        ByteConvert convert = value;
        _tmpBuffer[0] = convert.b0;
        _tmpBuffer[1] = convert.b1;
        WriteBuffer(_tmpBuffer, 2);
    }

    public void WriteInt32(int value)
    {
        ByteConvert convert = value;
        _tmpBuffer[0] = convert.b0;
        _tmpBuffer[1] = convert.b1;
        _tmpBuffer[2] = convert.b2;
        _tmpBuffer[3] = convert.b3;
        WriteBuffer(_tmpBuffer, 4);
    }

    public void WriteUInt32(uint value)
    {
        ByteConvert convert = value;
        _tmpBuffer[0] = convert.b0;
        _tmpBuffer[1] = convert.b1;
        _tmpBuffer[2] = convert.b2;
        _tmpBuffer[3] = convert.b3;
        WriteBuffer(_tmpBuffer, 4);
    }

    public void WriteInt64(long value)
    {
        ByteConvert convert = value;
        _tmpBuffer[0] = convert.b0;
        _tmpBuffer[1] = convert.b1;
        _tmpBuffer[2] = convert.b2;
        _tmpBuffer[3] = convert.b3;
        _tmpBuffer[4] = convert.b4;
        _tmpBuffer[5] = convert.b5;
        _tmpBuffer[6] = convert.b6;
        _tmpBuffer[7] = convert.b7;
        WriteBuffer(_tmpBuffer, 8);
    }

    public void WriteUInt64(ulong value)
    {
        ByteConvert convert = value;
        _tmpBuffer[0] = convert.b0;
        _tmpBuffer[1] = convert.b1;
        _tmpBuffer[2] = convert.b2;
        _tmpBuffer[3] = convert.b3;
        _tmpBuffer[4] = convert.b4;
        _tmpBuffer[5] = convert.b5;
        _tmpBuffer[6] = convert.b6;
        _tmpBuffer[7] = convert.b7;
        WriteBuffer(_tmpBuffer, 8);
    }

    public void WriteBoolean(bool value)
    {
        ByteConvert convert = value;
        _tmpBuffer[0] = convert.b0;
        WriteBuffer(_tmpBuffer, 1);
    }

    public void WriteSingle(float value)
    {
        ByteConvert convert = value;
        _tmpBuffer[0] = convert.b0;
        _tmpBuffer[1] = convert.b1;
        _tmpBuffer[2] = convert.b2;
        _tmpBuffer[3] = convert.b3;
        WriteBuffer(_tmpBuffer, 4);
    }

    public void WriteString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            WriteShort(0);
            return;
        }

        int byteCount = System.Text.Encoding.UTF8.GetByteCount(value);
        if (byteCount > short.MaxValue)
            throw new InvalidOperationException("string is too long.");

        if (_tmpBuffer.Length < byteCount)
            _tmpBuffer = new byte[byteCount];
        
        int len = System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, _tmpBuffer, 0);
        WriteShort((short)len);
        WriteBuffer(_tmpBuffer, len);
    }

    public void WriteBytes(byte[] bytes, int startIndex, int length)
    {
        if (null == bytes || startIndex < 0 || length <= 0 || startIndex + length > bytes.Length)
            throw new ArgumentOutOfRangeException(nameof(bytes), "buffer is null or out of range.");
        
        if (length > short.MaxValue)
            throw new InvalidOperationException("bytes length is too long.");
        
        WriteShort((short)length);
        WriteBuffer(bytes, length);
    }

    [StructLayout( LayoutKind.Explicit, Size = 8)]
    struct ByteConvert 
    {
        [FieldOffset(0)]
        public byte b0;
        [FieldOffset(1)]
        public byte b1;
        [FieldOffset(2)]
        public byte b2;
        [FieldOffset(3)]
        public byte b3;
        [FieldOffset(4)]
        public byte b4;
        [FieldOffset(5)]
        public byte b5;
        [FieldOffset(6)]
        public byte b6;
        [FieldOffset(7)]
        public byte b7;
        
        [FieldOffset(0)]
        public short s;
        [FieldOffset(0)]
        public int i;
        [FieldOffset(0)]
        public long l;
        [FieldOffset(0)]
        public float f;
        [FieldOffset(0)]
        public double d;
        [FieldOffset(0)]
        public ulong ul;
        [FieldOffset(0)]
        public uint ui;
        [FieldOffset(0)]
        public bool bl;
        
        public static implicit operator ByteConvert(short value)
        {
            return new ByteConvert
            {
                s = value
            };
        }
        
        public static implicit operator ByteConvert(int value)
        {
            return new ByteConvert
            {
                i = value
            };
        }
        
        public static implicit operator ByteConvert(long value)
        {
            return new ByteConvert
            {
                l = value
            };
        }
        
        public static implicit operator ByteConvert(float value)
        {
            return new ByteConvert
            {
                f = value
            };
        }
        
        public static implicit operator ByteConvert(double value)
        {
            return new ByteConvert
            {
                d = value
            };
        }
        
        public static implicit operator ByteConvert(ulong value)
        {
            return new ByteConvert
            {
                ul = value
            };
        }
        
        public static implicit operator ByteConvert(uint value)
        {
            return new ByteConvert
            {
                ui = value
            };
        }
        
        public static implicit operator ByteConvert(bool value)
        {
            return new ByteConvert
            {
                bl = value
            };
        }
    }
}

public interface ISerializable
{
    void Serialize(ref ISerializer writer);
    
    void Deserialize(ref ISerializer reader);
}
