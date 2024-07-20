
namespace WordBomb.Network
{
    public enum MessageType : byte
    {
        KeepAlive = 0,
        NOT_USED_0 = 1,
        NOT_USED_1 = 2,
        LoginRequest = 3,
        PlayRequest = 4,
        CancelPlayRequest = 5,
        Exit = 255
    }
}