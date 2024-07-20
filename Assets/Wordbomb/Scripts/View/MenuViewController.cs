using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using WordBomb.Network;

namespace WordBomb.View
{
    public enum MenuViewState
    {
        OFFLINE,
        ONLINE,
        SEARCHING_GAME,
    }
    public class MenuViewController : MonoBehaviour
    {
        [SerializeField]
        private MenuView m_menuView;
        private NetworkClient m_networkClient;
        public float StartSearchTime;
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
        public void Update()
        {
            if (State == MenuViewState.SEARCHING_GAME)
            {
                var current = Time.unscaledTime - StartSearchTime;
                m_menuView.LookingForAGameTimeText.text = ((int)current).ToString("00:00");
            }
        }

        private void HandleEventListeners()
        {
            if (m_networkClient)
            {
                m_networkClient.ConnectedToServer += OnConnectedToServer;
                m_networkClient.DisconnectedFromServer += OnDisconnectedFromServer;
            }
            if (m_menuView)
            {
                m_menuView.PlayButton.onClick.AddListener(() =>
                {
                    StartSearchTime = Time.unscaledTime;
                    UpdateState(MenuViewState.SEARCHING_GAME);
                    SendPlayRequest(Language.ENGLISH);
                });
                m_menuView.CancelButton.onClick.AddListener(() =>
                {
                    StartSearchTime = 0;
                    UpdateState(MenuViewState.ONLINE);
                    SendCancelPlayRequest();
                });
            }
        }

        private void SendCancelPlayRequest()
        {
            m_networkClient.Send(MessageType.CancelPlayRequest.ToByteArray());
        }

        private void SendPlayRequest(Language language)
        {
            var bytes = new byte[] { (byte)MessageType.PlayRequest, (byte)language };
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
                    m_menuView.LookingForAGamePanel.gameObject.SetActive(false);
                    m_menuView.LookingForAGameTimeText.text = StartSearchTime.ToString("00:00");
                    m_menuView.PlaySearchSettngButton.gameObject.SetActive(true);
                    m_menuView.CustomGameButton.gameObject.SetActive(true);
                    m_menuView.CancelButton.gameObject.SetActive(false);
                    m_menuView.PlayButton.gameObject.SetActive(true);
                    break;
                case MenuViewState.SEARCHING_GAME:
                    m_menuView.gameObject.SetActive(true);
                    m_menuView.LookingForAGamePanel.gameObject.SetActive(true);
                    m_menuView.LookingForAGameTimeText.text = StartSearchTime.ToString("00:00");
                    m_menuView.PlaySearchSettngButton.gameObject.SetActive(false);
                    m_menuView.CustomGameButton.gameObject.SetActive(false);
                    m_menuView.CancelButton.gameObject.SetActive(true);
                    m_menuView.PlayButton.gameObject.SetActive(false);
                    break;
            }
        }
        public void HandleRemoveListeners()
        {
            if (m_networkClient)
            {
                m_networkClient.ConnectedToServer -= OnConnectedToServer;
                m_networkClient.DisconnectedFromServer -= OnDisconnectedFromServer;
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