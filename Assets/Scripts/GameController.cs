using UnityEngine;
using Zenject;

namespace WaterSort
{
    public class GameController : MonoBehaviour
    {
        [Inject] Game game;

        public void LoadNextLevel()
        {
            StartCoroutine(game.LoadNextLevel());
        }
        public void RestartLevel()
        {
            StartCoroutine(game.ReloadLevel());
        }
    }
}