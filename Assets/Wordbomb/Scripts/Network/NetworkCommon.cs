using System.Security.Authentication;
using JamesFrowen.SimpleWeb;
using UnityEngine;
using WordBomb.Network;
namespace WordBomb.Network
{
    public class NetworkCommon : MonoBehaviour
    {
        public static ushort PORT = 7777;
        public const int MaxMessageSize = 32000;
        public const int MaxHandShakeSize = 5000;
        public const bool NoDelay = true;
        public const int SendTimeout = 5000;
        public const int ReceiveTimeout = 5000;
        public const int MaxMessagePerTick = 5000;
        public const bool SslEnabled = false;
        public static WaitForSeconds YieldKeepAliveForSeconds = new WaitForSeconds(2);
        public static TcpConfig TcpConfig = new TcpConfig(NoDelay, SendTimeout, ReceiveTimeout);
        public const string SslCertJson = "./cert.json";
        public static SslProtocols SslProtocols = SslProtocols.Tls12;
        public static SslConfig SslConfig => SslConfigLoader.Load(SslEnabled, SslCertJson, SslProtocols);
        public static string HOST = "localhost";
    }
}

public static class NetworkCommonUtilities
{
    public static byte[] ToByteArray(this MessageType messageType)
    {
        return new byte[] { (byte)messageType };
    }
}