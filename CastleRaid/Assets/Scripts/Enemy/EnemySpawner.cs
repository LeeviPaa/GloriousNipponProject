using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject enemyPrefab;
    [SerializeField]
    PatrolPoint[] patrolPoints;

    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy() {
		GameObject enemyObject = Instantiate(enemyPrefab, patrolPoints[0].transform.position, patrolPoints[0].transform.rotation);
        EnemyPatroller enemyScript = enemyObject.GetComponent<EnemyPatroller>();
        
        if(enemyScript != null)
        {
            enemyScript.InitializeEnemy(patrolPoints);
        }
    }

}
