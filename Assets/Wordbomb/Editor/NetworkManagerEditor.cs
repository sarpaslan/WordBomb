using UnityEditor;
using UnityEngine;
namespace WordBomb.Network
{
    [CustomEditor(typeof(NetworkServer))]
    public class NetworkManagerEditor : Editor
    {
        private NetworkServer m_networkManager;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (m_networkManager == null)
                m_networkManager = target as NetworkServer;

            GUILayout.BeginVertical();
            foreach (var conn in m_networkManager.Connections)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(conn.Value.Adress);
                GUILayout.Label(conn.Value.KeepAlive.ToString());
                GUILayout.Label(conn.Value.Name);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}