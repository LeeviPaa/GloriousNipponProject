﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public static class GameManager
{
    static GameManager()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    public delegate void SceneLoadDelegate(Scene scene, LoadSceneMode mode);
    public static event SceneLoadDelegate sceneLoaded;

    public delegate void SceneUnloadDelegate(Scene scene);
    public static event SceneUnloadDelegate sceneUnloaded;

    public delegate void MonoEventDelegate();
    public static event MonoEventDelegate levelAwake;
    public static event MonoEventDelegate levelStart;
    public static event MonoEventDelegate levelUpdate;

    public static LevelInstance levelInstance;
    public static EffectManager effectManager;
    public static AudioManager audioManager;

    public static SceneLoader sceneLoader;

    public static void OnLevelAwake()
    {
        if (levelAwake != null)
        {
            levelAwake.Invoke();
        }
    }

    public static void OnLevelStart()
    {
        if (levelStart != null)
        {
            levelStart.Invoke();
        }
    }

    public static void OnLevelUpdate()
    {
        if (levelUpdate != null)
        {
            levelUpdate.Invoke();
        }
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneLoaded != null)
        {
            sceneLoaded.Invoke(scene, mode);
        }
    }

    static void OnSceneUnloaded(Scene scene)
    {
        if (sceneLoaded != null)
        {
            sceneUnloaded.Invoke(scene);
        }
    }

    public static void ChangeScene(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);
        if (scene.IsValid())
        {
            ChangeScene(scene);
        }
        else
        {
            Debug.LogWarning("Scene with name [" + name + "] was not found");
        }
    }

    public static void ChangeScene(int index)
    {
        Scene scene = SceneManager.GetSceneByBuildIndex(index);
        if (scene.IsValid())
        {
            ChangeScene(scene);
        }
        else
        {
            Debug.LogWarning("Scene with index [" + index + "] was not found");
        }
    }

    static void ChangeScene(Scene scene)
    {
        if (!sceneLoader)
        {
            sceneLoader = new GameObject("SceneLoader", typeof(SceneLoader)).GetComponent<SceneLoader>();
            sceneLoader.Begin(scene.name);
            //sceneLoader.loadCompleted += () =>
            //{

            //};
        }
        else
        {
            Debug.LogWarning("Scene loading is already in progress.");
        }
    }
}
