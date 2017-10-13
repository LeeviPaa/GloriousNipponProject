using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using System;

public abstract class LevelInstance_Game : LevelInstance
{
    #region Variables
    public GameObject[] playerSpawnPoints;
    public float levelTimeLimit;
    public float levelTimeLeft = 0f;
    public string returnScene = "MainMenu";

    private bool timerActive = false;
    private Transform playArea;
    #endregion

    #region Events
    public delegate void LevelEventHandler(object sender, LevelEventArgs e);
    public event LevelEventHandler levelTimeEnded;

    public class LevelEventArgs : EventArgs
    {

    }

    protected virtual void OnLevelTimeEnded(LevelEventArgs e)
    {
        if (levelTimeEnded != null)
        {
            levelTimeEnded(this, e);
        }
    }
    #endregion

    protected virtual void Awake()
    {
        levelTimeLeft = levelTimeLimit;

        levelTimeEnded += EndScene;
    }

    protected virtual void Start()
    {
        playArea = VRTK_DeviceFinder.PlayAreaTransform();
        SetRandomSpawnpoint();
        StartLevel();
    }

    protected virtual void Update()
    {
        if (timerActive && levelTimeLimit != 0f)
        {
            levelTimeLeft -= Time.deltaTime;
            if (levelTimeLeft <= 0f)
            {
                levelTimeLeft = 0f;
                timerActive = false;
                OnLevelTimeEnded(new LevelEventArgs());
            }
        }
    }

    protected virtual void SetRandomSpawnpoint()
    {
        if (playerSpawnPoints.Length > 0)
        {
            playArea.position = playerSpawnPoints[UnityEngine.Random.Range(0, playerSpawnPoints.Length)].transform.position;
        }
    }

    protected virtual void EndScene(object sender, LevelEventArgs e)
    {
        GameManager.ChangeScene(returnScene);
    }

    public virtual void StartLevel()
    {
        timerActive = true;
    }
}
