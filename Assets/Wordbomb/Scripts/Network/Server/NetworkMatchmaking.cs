using System;
using System.IO;
using UnityEngine;
using WordBomb.Network;

public class NetworkMatchmaking : IDisposable
{
    private NetworkServer m_server;
    public NetworkMatchmaking(NetworkServer server)
    {
        m_server = server;
        server.MessageListeners.Add(MessageType.PlayRequest, HandlePlayRequest);
        server.MessageListeners.Add(MessageType.CancelPlayRequest, HandleCancelRequest);
    }

    private void HandleCancelRequest(int id, NetworkMemoryStream stream)
    {
        Debug.Log($"{id} is not want to play anymore");
    }

    public void HandlePlayRequest(int id, NetworkMemoryStream stream)
    {
        Debug.Log($"{id} wants to play");
    }
    public void Dispose()
    {
        m_server.MessageListeners.Remove(MessageType.PlayRequest);
    }
}