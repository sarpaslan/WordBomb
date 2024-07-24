using System;
using System.IO;

namespace WordBomb.Network
{
    [Serializable]
    struct PlayRequest
    {
        public Language Language;
        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)MessageType.LoginRequest);
            writer.Write((byte)Language);
            //Test
        }
        public void ReadFrom(BinaryReader reader)
        {
            reader.ReadByte();
            Language = (Language)reader.ReadByte();
        }
    }
}