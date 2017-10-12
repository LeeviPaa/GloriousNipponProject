using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInstance_Menu_MainMenu : LevelInstance_Menu
{
    private float testTimer = 0f;

    void Update()
    {
        testTimer += Time.deltaTime;
        if (testTimer >= 1f)
        {
            testTimer = 0f;
            print("test");
        }
    }
}
