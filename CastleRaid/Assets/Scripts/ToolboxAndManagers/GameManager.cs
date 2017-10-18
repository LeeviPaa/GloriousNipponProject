using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    static GameManager()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    public static LevelInstance levelInstance
    {
        get
        {
            if (!_levelInstance)
            {
                _levelInstance = Object.FindObjectOfType<LevelInstance>();
            }
            return _levelInstance;
        }
    }

    private static LevelInstance _levelInstance;

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    static void OnSceneUnloaded(Scene scene)
    {

    }

    public static void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
