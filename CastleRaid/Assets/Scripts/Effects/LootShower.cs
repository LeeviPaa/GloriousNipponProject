using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootShower : MonoBehaviour
{
    public Transform spawnAreaCenter;
    public Vector2 spawnAreaExtents;
    public GameObject[] lootPrefabs;
    public float spawnInterval;
    public Text valueCounter;
    public float lootDrainSpeed;
    public Text buttonText;

    private int lootTotalValue = 0;
    private float lootCurrentValue = 0;
    private float spawnTimer;
    private int state = 0;

    void Start()
    {
        object data;
        if (DataStorage.GetStoredData(LootBagController.lootTotalValueSaveKey, out data, true))
        {
            lootTotalValue = (int)data;
        }
        spawnTimer = spawnInterval;
		lootCurrentValue = 0;
    }

    void Update()
    {
        switch (state)
        {
            case 0:
                if (lootCurrentValue < lootTotalValue)
                {
                    lootCurrentValue += lootDrainSpeed * Time.deltaTime;
					spawnTimer -= Time.deltaTime;

					if (lootCurrentValue >= lootTotalValue)
                    {
                        FinishCurrentState();
                    }

                    valueCounter.text = Mathf.Round(lootCurrentValue).ToString();

                    if (spawnTimer <= 0f)
                    {
                        float x = Random.Range(-spawnAreaExtents.x / 2, spawnAreaExtents.x / 2);
                        float z = Random.Range(-spawnAreaExtents.y / 2, spawnAreaExtents.y / 2);
                        Vector3 pos = spawnAreaCenter.position + new Vector3(x, 0f, z);
                        Instantiate(lootPrefabs[Random.Range(0, lootPrefabs.Length)], pos, Random.rotation);
						spawnTimer = spawnInterval;
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
                buttonText.text = "Continue";
                break;

            case 1:
                GameManager.ChangeScene("MainMenu");
                break;

            default:
                break;
        }
        state++;
    }
}
