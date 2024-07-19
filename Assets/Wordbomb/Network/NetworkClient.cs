using System;
using System.Collections;
using System.IO;
using JamesFrowen.SimpleWeb;
using UnityEngine;

namespace WordBomb.Network
{
    public class NetworkClient : MonoBehaviour
    {
        public string Name;
        private SimpleWebClient m_client;
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
            StartCoroutine(KeepAliveCoroutine());
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
        void OnDestroy()
        {
            m_client.Send(new ArraySegment<byte>(MessageType.Exit.ToByteArray()));
            m_client.Disconnect();
        }

        public void Update()
        {
            m_client.ProcessMessageQueue();
            if (Input.GetKeyDown(KeyCode.A))
            {
                var login = new LoginRequest();
                login.Password = "55050975";
                login.Name = Name;
                var memoryStream = new MemoryStream();
                var writer = new BinaryWriter(memoryStream);
                login.Write(writer);
                m_client.Send(new ArraySegment<byte>(memoryStream.ToArray()));
            }
        }
    }
}