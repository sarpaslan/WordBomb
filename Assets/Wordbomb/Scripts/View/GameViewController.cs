using System.Collections.Generic;
using UnityEngine;
using WordBomb.Network;

namespace WordBomb.View
{
    public class GameViewController : MonoBehaviour
    {
        public MenuView MenuView;
        public GameView GameView;
        public NetworkClient Client;
        public Lobby Lobby;
        private Dictionary<int, PlayerView> m_playerViews = new Dictionary<int, PlayerView>();
        public void Start()
        {
            Client.MessageListeners.Add(MessageType.JoinedLobbyResponse, OnJoinedLobby);
        }
        private void OnJoinedLobby(NetworkMemoryStream stream)
        {
            var lobby = new JoinedLobbyResponse();
            lobby.ReadFrom(stream.Reader);
            this.Lobby = lobby.LobbyState.Lobby;
            GameView.gameObject.SetActive(true);
            MenuView.gameObject.SetActive(false);
            CreateGame(Lobby);
        }
        private void CreateGame(Lobby lobby)
        {
            foreach (var client in lobby.Clients)
            {
                var pl = Instantiate(GameView.m_playerPrefab, GameView.m_playersContainer);
                pl.Name.text = client.Name;
                pl.Level.text = client.Exp.ToString();
                pl.LastWord.text = "";
                for (int i = 0; i < client.Health; i++)
                {
                    var heart = Instantiate(pl.HeartPrefab, pl.HeartContainer);
                    heart.gameObject.SetActive(true);
                }
                m_playerViews.Add(client.Id, pl);
            }
        }
        private void OnDestroy()
        {
            m_playerViews.Clear();
            Client?.MessageListeners.Remove(MessageType.JoinedLobbyResponse);
        }
    }
}
