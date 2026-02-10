using System;
using System.Buffers;

namespace Vena
{
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

        #endregion
    }

    public struct Serializer : ISerializer
    {
        private static readonly ArrayPool<byte> _bufferPool = ArrayPool<byte>.Create(1024, 1024 * 1024);

        private byte[] _buffer;

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
            throw new NotImplementedException();
        }

        public short ReadShort()
        {
            throw new NotImplementedException();
        }

        public int ReadInt32()
        {
            throw new System.NotImplementedException();
        }

        public uint ReadUInt32()
        {
            throw new System.NotImplementedException();
        }

        public long ReadInt64()
        {
            throw new System.NotImplementedException();
        }

        public ulong ReadUInt64()
        {
            throw new System.NotImplementedException();
        }

        public bool ReadBoolean()
        {
            throw new System.NotImplementedException();
        }

        public float ReadSingle()
        {
            throw new System.NotImplementedException();
        }

        public string ReadString()
        {
            throw new NotImplementedException();
        }

        public void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }

        public void WriteShort(short value)
        {
            throw new NotImplementedException();
        }

        public void WriteInt32(int value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteUInt32(uint value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteInt64(long value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteUInt64(ulong value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteBoolean(bool value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteSingle(float value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteString(string value)
        {
            throw new NotImplementedException();
        }
    }
}