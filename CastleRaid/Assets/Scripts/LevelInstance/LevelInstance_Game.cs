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
    public string completeScene = "LevelComplete";

    private bool timerActive = false;
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

		EndScene();
    }
    #endregion

    protected override void Awake()
    {
		base.Awake();

		levelTimeLeft = levelTimeLimit;
    }

    protected override void Start()
    {
		base.Start();

        SetRandomSpawnpoint();
        StartLevel();
        InitBgmAudio();
    }

    protected override void Update()
    {
		base.Update();

        if (timerActive && levelTimeLimit > 0f)
        {
            levelTimeLeft -= Time.deltaTime;
            if (levelTimeLeft <= 0f)
            {
                levelTimeLeft = 0f;
                SetTimerActive(false);
                OnLevelTimeEnded(new LevelEventArgs());
            }
        }
    }



    protected virtual void SetRandomSpawnpoint()
    {
        if (playerSpawnPoints.Length > 0)
        {
            playerRelocator.RelocatePlayer(playerSpawnPoints[UnityEngine.Random.Range(0, playerSpawnPoints.Length)].transform.position, Quaternion.identity);
        }
    }

    protected virtual void EndScene()
    {
        GameManager.ChangeScene(completeScene);
    }

    public virtual void StartLevel()
    {
        SetTimerActive(true);
    }

    public virtual void SetTimerActive(bool state)
    {
        timerActive = state;
    }

    public virtual void InprisonPlayer()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Prison");
        if (go)
        {
            playerRelocator.RelocatePlayer(go.transform.position, go.transform.rotation);
        }
    }
}
