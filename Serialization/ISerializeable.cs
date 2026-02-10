//****************************************************************************
// File: ISerializable.cs
// Author: Li Nan
// Date: 2024-03-19 12:00
// Version: 1.0
//****************************************************************************

namespace Vena
{
    public interface ISerializable
    {
        void Serialize(ref ByteWriter writer);
        
        void Deserialize(ref ByteReader reader);
    }
}