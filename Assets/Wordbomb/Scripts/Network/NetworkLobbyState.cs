using System.IO;
namespace WordBomb.Network
{
    public struct NetworkLobbyState : INetworkPackage
    {
        public Lobby Lobby;
        public void Write(BinaryWriter writer)
        {
            writer.Write(Lobby.Clients.Count);
            for (int i = 0; i < Lobby.Clients.Count; i++)
            {
                var client = Lobby.Clients[i];
                client.Write(writer);
            }
            writer.Write(Lobby.Owner);
            writer.Write(Lobby.CurrentPlayerId);
            writer.Write(Lobby.CurrentTimer);
            writer.Write(Lobby.Language);
            writer.Write(Lobby.Code);
        }

        public void Read(BinaryReader reader)
        {
            Lobby = new Lobby();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var client = new Client();
                client.Id = reader.ReadInt32();
                client.Name = reader.ReadString();
                client.Health = reader.ReadByte();
                client.Exp = reader.ReadSingle();
                Lobby.Clients.Add(client);
            }
            Lobby.Owner = reader.ReadInt32();
            Lobby.CurrentPlayerId = reader.ReadInt32();
            Lobby.CurrentTimer = reader.ReadSingle();
            Lobby.Language = reader.ReadByte();
            Lobby.Code = reader.ReadString();
        }
    }
}