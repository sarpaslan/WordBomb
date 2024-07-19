using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using JamesFrowen.SimpleWeb;
using UnityEngine;
using System.Collections;

namespace WordBomb.Network
{
    public class NetworkServer : MonoBehaviour
    {
        private TcpConfig m_config;
        private SimpleWebServer m_server;
        public Dictionary<int, Client> Connections = new Dictionary<int, Client>();
        private List<int> m_connectionIds = new List<int>(20);
        public Dictionary<MessageType, Action<int, NetworkMemoryStream>> MessageListeners
        = new();
        public NetworkMemoryStream Stream = new();

        private void Awake()
        {
            m_config = NetworkCommon.TcpConfig;
            m_server = new SimpleWebServer(NetworkCommon.MaxMessagePerTick, m_config,
            NetworkCommon.MaxMessageSize, NetworkCommon.MaxHandShakeSize, NetworkCommon.SslConfig);
            m_server.Start(NetworkCommon.PORT);
            m_server.onConnect += OnConnect;
            m_server.onDisconnect += OnDisconnect;
            m_server.onData += OnData;
            m_server.onError += OnError;
            MessageListeners.Add(MessageType.LoginRequest, OnLoginRequest);
            MessageListeners.Add(MessageType.KeepAlive, KeepAlive);
            StartCoroutine(KeepAliveCoroutine());
        }

        public IEnumerator KeepAliveCoroutine()
        {
            var bytes = new byte[] { (byte)MessageType.KeepAlive };
            var segment = new ArraySegment<byte>(bytes);
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                yield return NetworkCommon.YieldKeepAliveForSeconds;
                for (int i = 0; i < m_connectionIds.Count; i++)
                {
                    var id = m_connectionIds[i];
                    m_server.SendOne(id, segment);
                }
            }
        }

        public void KeepAlive(int arg1, NetworkMemoryStream stream)
        {
            Connections[arg1].KeepAlive += 1;
        }

        private void Update()
        {
            m_server.ProcessMessageQueue();
        }

        private void OnData(int connectionId, ArraySegment<byte> segment)
        {
            var type = (MessageType)segment[0];
            Stream.Set(segment);
            MessageListeners[type].Invoke(connectionId, Stream);
        }

        private void OnLoginRequest(int id, NetworkMemoryStream stream)
        {
            var request = new LoginRequest();
            request.ReadFrom(stream.Reader);
            var conn = Connections[id];
            conn.Name = NetworkValidation.EnsureValidName(conn.Name, request.Name);
        }

        private void OnConnect(int id)
        {
            var client = new Client
            {
                Id = id,
                Adress = m_server.GetClientAddress(id),
                KeepAlive = 0,
                Name = "Guest " + Random.Range(0, 255)
            };
            Connections.Add(id, client);
            m_connectionIds.Add(id);
            Debug.Log($"{id} is connected");
        }

        private void OnDisconnect(int id)
        {
            if (Connections.Remove(id))
            {
                m_connectionIds.Remove(id);
                Debug.Log($"{id} is disconnected");
            }
            else
            {
                OnError(id, new InvalidOperationException("Disconnecting an unknown user id"));
            }
        }

        private void OnError(int id, Exception exception)
        {
            Debug.Log($"Error with connection {id}: {exception}");
        }

        private void OnDestroy()
        {
            m_server.Stop();
            MessageListeners.Clear();
            Connections.Clear();
            m_connectionIds.Clear();
            m_server.onConnect -= OnConnect;
            m_server.onDisconnect -= OnDisconnect;
            m_server.onData -= OnData;
            m_server.onError -= OnError;
        }
    }
}

