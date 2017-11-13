using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInstance_LevelComplete : LevelInstance_Menu
{
    public string nextLevel;
    [Header("Loot Shower")]
    public Transform spawnAreaCenter;
    public Vector2 spawnAreaExtents;
    public GameObject[] lootPrefabs;
    public float lootSpawnInterval;
    public string lootSpawnSound;
    [Header("Loot Counter")]
    public Text pointCounter;
    public float lootCounterSpeed;
    public Text proceedButtonText;
    public string pointCounterSound;
    [Header("Events")]
    public StepActionMachine stepMachine;
    [Header("Keyboard")]
    public VirtualKeyBoard vkb;
    [Header("Hiscore")]
    public Text hiscoreNameText;
    public Text hiscoreValueText;

    private int lootTotalValue = 0;
    private float lootCurrentValue = 0;
    private float spawnTimer;
    private int state = 0;
    private AudioItem pointCounterSoundItem;
    private List<int> scoreValues = new List<int>();
    private List<string> scoreNames = new List<string>();

    protected override void Start()
    {
        base.Start();

        int data;
        if (DataStorage.TryGetInt(LootBagController.lootTotalValueSaveKey, out data))
        {
            lootTotalValue = data;
        }
        spawnTimer = lootSpawnInterval;
        lootCurrentValue = 0;
        pointCounterSoundItem = GameManager.audioManager.GetAudio(pointCounterSound, true, false, pointCounter.transform.position, pointCounter.transform);

        SetStepMachineEvents();
        ProceedStepMachine();
    }

    protected override void Update()
    {
        base.Update();

        stepMachine.Update();
    }

    // State 0: Counter ticking up to total loot value
    // State 1: Waiting for confirmation to continue
    // State 2: Write username and confirm it
    // State 3: Show hiscores and wait for exit button

    void SetStepMachineEvents()
    {
        stepMachine.AddStepAction(0, StepActionMachine.StepActionTime.Start, () =>
        {
            proceedButtonText.text = "Skip";
        });

        stepMachine.AddStepAction(0, StepActionMachine.StepActionTime.Update, () =>
        {
            if (lootCurrentValue <= lootTotalValue)
            {
                lootCurrentValue += lootCounterSpeed * Time.deltaTime;
                spawnTimer -= Time.deltaTime;

                if (lootCurrentValue >= lootTotalValue)
                {
                    ProceedStepMachine();
                }

                pointCounter.text = Mathf.Round(lootCurrentValue).ToString();

                if (spawnTimer <= 0f)
                {
                    float x = Random.Range(-spawnAreaExtents.x / 2, spawnAreaExtents.x / 2);
                    float z = Random.Range(-spawnAreaExtents.y / 2, spawnAreaExtents.y / 2);
                    Vector3 pos = spawnAreaCenter.position + new Vector3(x, 0f, z);
                    Instantiate(lootPrefabs[Random.Range(0, lootPrefabs.Length)], pos, Random.rotation);
                    spawnTimer = lootSpawnInterval;
                    GameManager.audioManager.GetAudio(lootSpawnSound, true, true, pos, transform);
                }
            }
        });

        stepMachine.AddStepAction(1, StepActionMachine.StepActionTime.Start, () =>
        {
            proceedButtonText.text = "Continue";
            lootCurrentValue = lootTotalValue;
            pointCounterSoundItem.ReturnToPool();
        });

        stepMachine.AddStepAction(2, StepActionMachine.StepActionTime.Start, () =>
        {
            vkb.submit += SaveScore;
        });

        stepMachine.AddStepAction(2, StepActionMachine.StepActionTime.End, () =>
        {
            vkb.submit -= SaveScore;
        });

        stepMachine.AddStepAction(3, StepActionMachine.StepActionTime.Start, () =>
        {
            for (int i = 0; i < scoreNames.Count; i++)
            {
                hiscoreNameText.text += scoreNames[i];
                hiscoreValueText.text += scoreValues[i];
            }          
        });

        stepMachine.AddStepAction(3, StepActionMachine.StepActionTime.End, () =>
        {
            GameManager.ChangeScene(nextLevel);
        });
    }

    public void ProceedStepMachine()
    {
        stepMachine.Next();
    }

    void SaveScore(string value)
    {
        vkb.submit -= SaveScore;
        scoreNames = DataStorage.GetList("ScoreNames", scoreNames);
        scoreValues = DataStorage.GetList("ScoreValues", scoreValues);
        for (int i = 0; i < scoreValues.Count; i++)
        {
            if (lootTotalValue > scoreValues[i])
            {
                scoreNames.Insert(i, value);
                scoreValues.Insert(i, lootTotalValue);
            }
        }
        DataStorage.SetList("ScoreNames", scoreNames);
        DataStorage.SetList("ScoreValues", scoreValues);
        ProceedStepMachine();
    }
}
