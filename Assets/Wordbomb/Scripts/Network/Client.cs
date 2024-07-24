
using System;
using System.IO;

namespace WordBomb.Network
{
    [Serializable]
    public class Client
    {
        public int Id;
        public uint KeepAlive;
        public string Name;
        public string Adress;
        public byte SearchGameLanguage;
        public string Lobby;
        public bool Bot;
        public byte AiType;
        public byte Health;
        public float Exp;

        internal void Write(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Health);
            writer.Write(Exp);
        }
        public void Read(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Name = reader.ReadString();
            Health = reader.ReadByte();
            Exp = reader.ReadSingle();
        }
    }
}