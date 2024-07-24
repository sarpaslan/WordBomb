
using System.IO;

namespace WordBomb.Network
{
    interface INetworkPackage
    {
        void Write(BinaryWriter writer);
        void Read(BinaryReader reader);
    }
}