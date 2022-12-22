using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System;

public class GameView : MonoBehaviour
{
    [SerializeField] Canvas window;
    [SerializeField] Text currentLevel;
    [SerializeField] Button restart;
    [Inject] Game game;

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
    public void EnablePopWindow()
    {
        window.enabled = true;
        restart.enabled = false;
    }
    public void LoadNextLevel()
    {
        StartCoroutine(game.LoadNextLevel());
        window.enabled = false;
    }
    public void Restart()
    {
        StartCoroutine(game.ReloadLevel());
    }
}
