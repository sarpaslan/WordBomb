
using System;

namespace WordBomb.Network
{
    [Serializable]
    public class Client
    {
        public int Id;
        public uint KeepAlive;
        public string Name;
        public string Adress;
    }

}