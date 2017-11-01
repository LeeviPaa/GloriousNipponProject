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
    public bool useBgmAudio;
    public string bgmAudioName;

    private bool timerActive = false;
    private AudioItem bgmAudio;
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

    protected virtual void InitBgmAudio()
    {
        if (useBgmAudio)
        {
            bgmAudio = GameManager.audioManager.GetAudio(bgmAudioName, true, false, Vector3.zero, transform);
            if (!bgmAudio)
            {
                useBgmAudio = false;
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

    protected virtual void EndScene()
    {
        GameManager.ChangeScene(completeScene);
    }

    public virtual void StartLevel()
    {
        timerActive = true;
    }
}
