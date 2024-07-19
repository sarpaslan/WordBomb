using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Text;
using JamesFrowen.SimpleWeb;
using UnityEngine;
using UnityEngine.Assertions.Must;

[Serializable]
struct LoginRequest
{
    public string Name;
    public string Password;

    public void Write(BinaryWriter writer)
    {
        writer.Write((byte)MessageType.LoginRequest);
        writer.Write(Name);
        writer.Write(Password);
    }
    public void ReadFrom(BinaryReader reader)
    {
        reader.ReadByte();
        Name = reader.ReadString();
        Password = reader.ReadString();
    }
}

public class NetworkManagerClient : MonoBehaviour
{
    public string Name;
    private SimpleWebClient m_client;
    private TcpConfig m_config;
    public void Start()
    {
        m_config = new TcpConfig(noDelay: false, sendTimeout: 5000, receiveTimeout: 20000);
        m_client = SimpleWebClient.Create(ushort.MaxValue, 5000, m_config);
        var builder = new UriBuilder
        {
            Scheme = "ws",
            Host = "localhost",
            Port = 7777
        };
        m_client.Connect(builder.Uri);
    }

    void OnDestroy()
    {
        m_client.Disconnect();
    }

    public void Update()
    {
        m_client.ProcessMessageQueue();
        if (Input.GetKeyDown(KeyCode.A))
        {
            LoginRequest login = new LoginRequest();
            login.Password = "55050975";
            login.Name = Name;
            var memoryStream = new MemoryStream();
            var writer = new BinaryWriter(memoryStream);
            login.Write(writer);
            m_client.Send(new ArraySegment<byte>(memoryStream.ToArray()));
        }
    }
}
public class Message
{
    public MessageType Type;
}
public enum MessageType : byte
{
    Error = 0,
    Notused2 = 1,
    Notused4 = 2,
    LoginRequest = 3,
}
