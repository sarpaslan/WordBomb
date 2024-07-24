using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using WordBomb.Game;
using WordBomb.Network;

[Serializable]
public class Lobby
{
    public int Id;
    public int Owner;
    public List<Client> Clients = new List<Client>();
    public string Code;
    public int CurrentPlayerId;
    public float CurrentTimer;
    public byte Language;
}
public class NetworkMatchmaking : IDisposable
{
    private char[] m_randomChars = new[]
    {
       'A','B','C','D','X','L','H','Y','M','N','J'
    };
    public List<Lobby> Lobbies = new List<Lobby>();
    private NetworkServer m_server;
    public NetworkMatchmaking(NetworkServer server)
    {
        m_server = server;
        server.MessageListeners.Add(MessageType.QuickPlayRequest, HandleQuickPlay);
        server.MessageListeners.Add(MessageType.ExitLobbyRequest, HandleExitLobby);
    }

    public void HandleQuickPlay(int id, NetworkMemoryStream stream)
    {
        var client = m_server.GetClient(id);
        if (!string.IsNullOrEmpty(client.Lobby))
        {
            m_server.ErrorUser(id, "You are already in a room");
            return;
        }
        var sameLanguageLobbies = Lobbies.
        Where(t => t.Language == client.SearchGameLanguage);

        var lobby = sameLanguageLobbies.OrderByDescending(t => t.Clients.Count)
        .FirstOrDefault(t => t.Clients.Count < GameConfig.MAX_PLAYER_PER_LOBBY);
        if (lobby == null)
        {
            Debug.Log("Creating a lobby");
            lobby = CreateLobby(client);
            Lobbies.Add(lobby);
        }
        client.Lobby = lobby.Code;
        lock (m_server.Stream)
        {
            m_server.Stream.Reset();
            var joinedLobby = new JoinedLobbyResponse
            {
                LobbyState = new NetworkLobbyState()
                {
                    Lobby = lobby,
                }
            };
            joinedLobby.Write(m_server.Stream.Writer);
            m_server.SendOne(id, m_server.Stream.ToArray());
        }
    }
    public Lobby CreateLobby(Client owner)
    {
        owner.Health = 3;
        return new Lobby()
        {
            Id = 0,
            CurrentPlayerId = 0,
            CurrentTimer = 0,
            Language = owner.SearchGameLanguage,
            Owner = owner.Id,
            Clients = new List<Client>(4)
            {
                owner,
            },
            Code = GenerateRandomCode()
        };
    }

    private string GenerateRandomCode()
    {
        return UnityEngine.Random.Range(1000, 9999).ToString()
        + m_randomChars[UnityEngine.Random.Range(0, m_randomChars.Length - 1)];
    }

    private void HandleExitLobby(int id, NetworkMemoryStream stream)
    {
    }

    public void Dispose()
    {
        m_server.MessageListeners.Remove(MessageType.QuickPlayRequest);
    }
}