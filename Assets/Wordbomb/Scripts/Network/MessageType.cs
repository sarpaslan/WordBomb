
namespace WordBomb.Network
{
    public enum MessageType : byte
    {
        KeepAlive = 0,
        NOT_USED_0 = 1,
        NOT_USED_1 = 2,
        LoginRequest = 3,
        QuickPlayRequest = 4,
        ExitLobbyRequest = 5,
        JoinedLobbyResponse = 6,
        Exit = 255
    }
}