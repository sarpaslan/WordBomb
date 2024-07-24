using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JamesFrowen.SimpleWeb;
using UnityEngine;
using UnityEngine.Events;

namespace WordBomb.Network
{
    public class NetworkClient : MonoBehaviour
    {
        public string Name;
        private SimpleWebClient m_client;
        public bool Connected;
        public UnityEvent OnStart = new();
        public UnityEvent ConnectedToServer = new();
        public UnityEvent DisconnectedFromServer = new();
        public Dictionary<MessageType, Action<NetworkMemoryStream>> MessageListeners
        = new();
        public NetworkMemoryStream Stream = new();

        public void Start()
        {
            m_client = SimpleWebClient.Create(NetworkCommon.MaxMessageSize, NetworkCommon.MaxMessagePerTick, NetworkCommon.TcpConfig);
            var builder = new UriBuilder
            {
                Scheme = "ws",
                Host = NetworkCommon.HOST,
                Port = NetworkCommon.PORT
            };
            m_client.Connect(builder.Uri);
            m_client.onConnect += OnConnected;
            m_client.onDisconnect += OnDisconnected;
            m_client.onData += OnData;
            OnStart?.Invoke();
        }

        private void KeepAlive(NetworkMemoryStream stream)
        {
        }

        private void OnData(ArraySegment<byte> segment)
        {
            var type = (MessageType)segment[0];
            Stream.Set(segment);
            MessageListeners[type].Invoke(Stream);
        }

        private void OnConnected()
        {
            StartCoroutine(KeepAliveCoroutine());
            MessageListeners.Add(MessageType.KeepAlive, KeepAlive);
            Connected = true;
            ConnectedToServer?.Invoke();
        }

        private void OnDisconnected()
        {
            MessageListeners.Remove(MessageType.KeepAlive);
            Connected = false;
            DisconnectedFromServer?.Invoke();
        }

        public void Send(ArraySegment<byte> segment)
        {
            m_client.Send(segment);
        }

        public IEnumerator KeepAliveCoroutine()
        {
            var bytes = new byte[] { (byte)MessageType.KeepAlive };
            var segment = new ArraySegment<byte>(bytes);
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                yield return NetworkCommon.YieldKeepAliveForSeconds;
                m_client.Send(segment);
            }
        }

        private void Update()
        {
            m_client.ProcessMessageQueue();
            if (Input.GetKeyDown(KeyCode.A))
            {
                var login = new LoginRequest();
                login.Password = "5505";
                login.Name = Name;
                var memoryStream = new MemoryStream();
                var writer = new BinaryWriter(memoryStream);
                login.Write(writer);
                m_client.Send(new ArraySegment<byte>(memoryStream.ToArray()));
            }
        }

        private void OnDestroy()
        {
            m_client.Send(new ArraySegment<byte>(MessageType.Exit.ToByteArray()));
            m_client.Disconnect();
            m_client.onConnect -= OnConnected;
            m_client.onDisconnect -= OnDisconnected;
            m_client.onData -= OnData;
            StopAllCoroutines();
        }

    }
}