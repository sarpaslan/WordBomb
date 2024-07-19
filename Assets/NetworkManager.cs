using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
using JamesFrowen.SimpleWeb;
using UnityEngine;

namespace WordBomb
{
    public class Client
    {
        public int Id;
    }

    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private int m_port = 7778;
        [SerializeField] private int m_maxMessageSize = 32000;
        [SerializeField] private int m_maxHandShakeSize = 5000;

        [SerializeField] private bool m_noDelay = true;
        [SerializeField] private int m_sendTimeout = 5000;
        [SerializeField] private int m_receiveTimeout = 5000;

        [SerializeField] private int m_maxMessagePerTick = 5000;

        [Header("Ssl Settings")]
        [SerializeField] private bool sslEnabled;
        [Tooltip("See .cert.example.Json for example")]
        [SerializeField] private string sslCertJson = "./cert.json";
        [SerializeField] private SslProtocols sslProtocols = SslProtocols.Tls12;
        private TcpConfig m_config;
        private SimpleWebServer m_server;
        public Dictionary<int, Client> Connections = new Dictionary<int, Client>();
        public Dictionary<MessageType, Action<int, NetworkMemoryStream>> MessageListeners
        = new();

        public NetworkMemoryStream Stream = new();

        private void Awake()
        {
            m_config = new TcpConfig(m_noDelay, m_sendTimeout, m_receiveTimeout);
            var sslConfig = SslConfigLoader.Load(sslEnabled, sslCertJson, sslProtocols);
            m_server = new SimpleWebServer(m_maxMessagePerTick, m_config, m_maxMessageSize, m_maxHandShakeSize, sslConfig);
            m_server.Start(GameConfig.PORT);
            m_server.onConnect += OnConnect;
            m_server.onDisconnect += OnDisconnect;
            m_server.onData += OnData;
            m_server.onError += OnError;
            MessageListeners.Add(MessageType.LoginRequest, OnLoginRequest);
        }

        private void Update()
        {
            m_server.ProcessMessageQueue();
        }

        private void OnData(int connectionId, ArraySegment<byte> segment)
        {
            var type = (MessageType)segment[0];
            var messageSegment = new ArraySegment<byte>(segment.Array, segment.Offset, segment.Count);
            Stream.Set(messageSegment);
            if (MessageListeners.TryGetValue(type, out var action))
            {
                action?.Invoke(connectionId, Stream);
            }
        }

        private void OnLoginRequest(int id, NetworkMemoryStream stream)
        {
            var lgReq = new LoginRequest();
            lgReq.ReadFrom(stream.Reader);
            Debug.Log($"Login Request : {lgReq.Name} {lgReq.Password}");
        }

        private void OnConnect(int id)
        {
            var client = new Client { Id = id };
            Connections.Add(id, client);
            Debug.Log($"{id} is connected");
        }

        private void OnDisconnect(int id)
        {
            if (Connections.Remove(id))
            {
                Debug.Log($"{id} is disconnected");
            }
            else
            {
                OnError(id, new InvalidOperationException("Disconnecting an unknown user id"));
            }
        }

        private void OnError(int id, Exception exception)
        {
            Debug.LogError($"Error with connection {id}: {exception}");
        }

        private void OnDestroy()
        {
            m_server.Stop();
            MessageListeners.Clear();
            Connections.Clear();
            m_server.onConnect -= OnConnect;
            m_server.onDisconnect -= OnDisconnect;
            m_server.onData -= OnData;
            m_server.onError -= OnError;
        }
    }
}

