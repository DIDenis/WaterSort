using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] Canvas window;
        [SerializeField] Text currentLevel;
        [SerializeField] Button restart;

        void Awake()
        {
            window.enabled = false;
            restart.enabled = false;
        }

        public void SetLevelName(string levelName)
        {
            currentLevel.text = levelName;
            restart.enabled = true;
        }
        public void OpenPopWindow()
        {
            window.enabled = true;
            restart.enabled = false;
        }
        public void ClosePopWindow()
        {
            window.enabled = false;
        }
    }
}