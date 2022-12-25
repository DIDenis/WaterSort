using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Zenject;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour, IPersistentObject
{
    [Inject] GameView gameView;
    GameDataStorage storage;
    Scene activeScene;
    int currentLevel;
    bool completedLevel;
    bool isTest;

    void Start()
    {
        storage = new GameDataStorage();
        storage.Load(this);
        if (currentLevel < 1)
            currentLevel = 1;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        activeScene = SceneManager.GetActiveScene();
#if UNITY_EDITOR
        if (SceneManager.sceneCount == 1)
            StartCoroutine(LoadNextLevel());
        else if (SceneManager.sceneCount == 2)
        {
            isTest = true;
            gameView.SetLevelName("Test");
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
        }
        else
        {
            Debug.LogError("Количество одновременно загруженных сцен не должно превышать 2");
            UnityEditor.EditorApplication.isPlaying = false;
        }
#else
        StartCoroutine(LoadNextLevel());
#endif
    }

    public void Save(GameDataWriter writer)
    {
        writer.Write(completedLevel);
        writer.Write(currentLevel);
    }
    public void Load(GameDataReader reader)
    {
        completedLevel = reader.ReadBool();
        currentLevel = reader.ReadInt();
    }

    ///<summary>
    ///Завершает текущий уровень
    ///</summary>
    public void CompleteLevel()
    {
#if UNITY_EDITOR
        if (isTest)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
#endif
        completedLevel = true;
        storage.Save(this);
        gameView.EnablePopWindow();
    }

    ///<summary>
    ///Загружает следующий уровень при условии, что текущий завершён
    ///</summary>
    public IEnumerator LoadNextLevel()
    {
        if (currentLevel == SceneManager.sceneCountInBuildSettings - 1)
            currentLevel = 1;
        else if (completedLevel)
            currentLevel++;
        completedLevel = false;
        storage.Save(this);

        if (activeScene.buildIndex != 0)
            yield return SceneManager.UnloadSceneAsync(activeScene);
        yield return SceneManager.LoadSceneAsync(currentLevel, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentLevel));
        activeScene = SceneManager.GetActiveScene();
        gameView.SetLevelName(activeScene.name);
    }

    ///<summary>
    ///Перезагружает текущий уровень
    ///</summary>
    public IEnumerator ReloadLevel()
    {
        yield return SceneManager.UnloadSceneAsync(activeScene);
        yield return SceneManager.LoadSceneAsync(currentLevel, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentLevel));
        activeScene = SceneManager.GetActiveScene();
    }
}
