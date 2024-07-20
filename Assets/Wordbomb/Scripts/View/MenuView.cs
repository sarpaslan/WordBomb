using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace WordBomb.View
{
    public class MenuView : MonoBehaviour
    {
        public Button PlayButton;
        public Button PlaySearchSettngButton;
        public Button CustomGameButton;
        public Button CancelButton;
        public Button DiscordButton;
        public RectTransform LookingForAGamePanel;
        public TMP_Text LookingForAGameTimeText;
    }
}
