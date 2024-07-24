using System;
using System.IO;

namespace WordBomb.Network
{
    [Serializable]
    struct JoinedLobbyResponse
    {
        public NetworkLobbyState LobbyState;

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)MessageType.JoinedLobbyResponse);
            LobbyState.Write(writer);
        }
        public void ReadFrom(BinaryReader reader)
        {
            reader.ReadByte();
            LobbyState = new NetworkLobbyState();
            LobbyState.Read(reader);
        }
    }
}