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

    public static EffectManager effectManager
    {
        get
        {
            if (!_effectManager)
            {
                _effectManager = Object.FindObjectOfType<EffectManager>();
            }
            return _effectManager;
        }
    }

    private static EffectManager _effectManager;

    public static AudioManager audioManager
    {
        get
        {
            if (!_audioManager)
            {
                _audioManager = Object.FindObjectOfType<AudioManager>();
            }
            return _audioManager;
        }
    }

    private static AudioManager _audioManager;

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
