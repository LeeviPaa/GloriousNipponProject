using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInstance_LevelComplete : LevelInstance_Menu
{
    public Transform spawnAreaCenter;
    public Vector2 spawnAreaExtents;
    public GameObject[] lootPrefabs;
    public float spawnInterval;
    public Text pointCounter;
    public float lootCounterSpeed;
    public Text proceedButtonText;
    public string nextLevel;
    public string lootSpawnSound;
    public string victorySound;
    public string pointCounterSound;

    private int lootTotalValue = 0;
    private float lootCurrentValue = 0;
    private float spawnTimer;
    private int state = 0;
    private AudioItem pointCounterSoundItem;

    protected override void Start()
    {
        base.Start();

        int data;
        if (DataStorage.TryGetInt(LootBagController.lootTotalValueSaveKey, out data))
        {
            lootTotalValue = data;
        }
        spawnTimer = spawnInterval;
        lootCurrentValue = 0;
        GameManager.audioManager.GetAudio(victorySound, true, true, Vector3.zero, transform);
        pointCounterSoundItem = GameManager.audioManager.GetAudio(pointCounterSound, true, false, pointCounter.transform.position, pointCounter.transform);
    }

    protected override void Update()
    {
        base.Update();

        switch (state)
        {
            case 0:
                if (lootCurrentValue <= lootTotalValue)
                {
                    lootCurrentValue += lootCounterSpeed * Time.deltaTime;
                    spawnTimer -= Time.deltaTime;

                    if (lootCurrentValue >= lootTotalValue)
                    {
                        FinishCurrentState();
                    }

                    pointCounter.text = Mathf.Round(lootCurrentValue).ToString();

                    if (spawnTimer <= 0f)
                    {
                        float x = Random.Range(-spawnAreaExtents.x / 2, spawnAreaExtents.x / 2);
                        float z = Random.Range(-spawnAreaExtents.y / 2, spawnAreaExtents.y / 2);
                        Vector3 pos = spawnAreaCenter.position + new Vector3(x, 0f, z);
                        Instantiate(lootPrefabs[Random.Range(0, lootPrefabs.Length)], pos, Random.rotation);
                        spawnTimer = spawnInterval;
                        GameManager.audioManager.GetAudio(lootSpawnSound, true, true, pos, transform);
                    }
                }
                break;

            default:
                break;
        }
    }

    public void FinishCurrentState()
    {
        switch (state)
        {
            case 0:
                lootCurrentValue = lootTotalValue;
                proceedButtonText.text = "Continue";
                pointCounterSoundItem.ReturnToPool();
                break;

            case 1:
                GameManager.ChangeScene(nextLevel);
                break;

            default:
                break;
        }
        state++;
    }
}
