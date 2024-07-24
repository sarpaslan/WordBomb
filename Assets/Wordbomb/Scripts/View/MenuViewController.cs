using System;
using UnityEngine;
using WordBomb.Network;

namespace WordBomb.View
{
    public enum MenuViewState
    {
        OFFLINE,
        ONLINE,
    }

    public class MenuViewController : MonoBehaviour
    {
        [SerializeField]
        private MenuView m_menuView;
        private NetworkClient m_networkClient;
        private float m_internalTime;
        public MenuViewState State = MenuViewState.OFFLINE;

        private void OnEnable()
        {
            m_networkClient = FindObjectOfType<NetworkClient>(); ;
            HandleEventListeners();
            if (m_networkClient && m_networkClient.Connected)
                State = MenuViewState.ONLINE;
            UpdateState(State);
        }
        private void HandleEventListeners()
        {
            if (m_networkClient)
            {
                m_networkClient.ConnectedToServer.AddListener(OnConnectedToServer);
                m_networkClient.DisconnectedFromServer.AddListener(OnDisconnectedFromServer);
            }
            if (m_menuView)
            {
                m_menuView.PlayButton.onClick.AddListener(() =>
                {
                    SendPlayRequest(Language.ENGLISH);
                });
                m_menuView.CancelButton.onClick.AddListener(() =>
                {
                    UpdateState(MenuViewState.ONLINE);
                    SendCancelPlayRequest();
                });
            }
        }

        private void SendCancelPlayRequest()
        {
            m_networkClient.Send(MessageType.ExitLobbyRequest.ToByteArray());
        }

        private void SendPlayRequest(Language language)
        {
            var bytes = new byte[] { (byte)MessageType.QuickPlayRequest, (byte)language };
            var segment = new ArraySegment<byte>(bytes);
            m_networkClient.Send(segment);
        }

        private void UpdateState(MenuViewState state)
        {
            if (this.State == state)
                return;
            this.State = state;
            switch (state)
            {
                case MenuViewState.OFFLINE:
                    m_menuView.gameObject.SetActive(false);
                    break;
                case MenuViewState.ONLINE:
                    m_menuView.gameObject.SetActive(true);
                    m_menuView.PlaySearchSettngButton.gameObject.SetActive(true);
                    m_menuView.CustomGameButton.gameObject.SetActive(true);
                    m_menuView.CancelButton.gameObject.SetActive(false);
                    m_menuView.PlayButton.gameObject.SetActive(true);
                    break;
            }
        }
        public void HandleRemoveListeners()
        {
            if (m_networkClient)
            {
                m_networkClient.ConnectedToServer.RemoveListener(OnConnectedToServer);
                m_networkClient.DisconnectedFromServer.RemoveListener(OnDisconnectedFromServer);
            }
            if (m_menuView)
            {
                m_menuView.PlayButton.onClick.RemoveAllListeners();
            }
        }

        private void OnDisconnectedFromServer()
        {
            UpdateState(MenuViewState.OFFLINE);
        }

        private void OnConnectedToServer()
        {
            UpdateState(MenuViewState.ONLINE);
        }
        private void OnDisable()
        {
            HandleRemoveListeners();
        }
    }
}