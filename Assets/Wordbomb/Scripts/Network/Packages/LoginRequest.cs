using System;
using System.IO;

namespace WordBomb.Network
{
    [Serializable]
    struct LoginRequest
    {
        public string Name;
        public string Password;

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)MessageType.LoginRequest);
            writer.Write(Name);
            writer.Write(Password);
        }
        public void ReadFrom(BinaryReader reader)
        {
            reader.ReadByte();
            Name = reader.ReadString();
            Password = reader.ReadString();
        }
    }
}