using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] GameObject[] EnemyPrefab;
    [SerializeField] float nextSpawn = 3.0f;
    [SerializeField] float minRangeX;
    [SerializeField] float minRangeY;
    [SerializeField] float maxRangeX;
    [SerializeField] float maxRangeY;
    [SerializeField] float groundY;

    void Start()
    {
        StartCoroutine(EnemySpawn());
    }

    IEnumerator EnemySpawn()
    {
        while (true)
        {
            var x = Random.Range(minRangeX, maxRangeX);
            var y = Random.Range(minRangeY, maxRangeY);
            var position = new Vector3(x, y);
            int enemyPrefabIndex = Random.Range(0, EnemyPrefab.Length);
            if(enemyPrefabIndex == 0){ // shadowhands
                Instantiate(EnemyPrefab[enemyPrefabIndex], new Vector3(x, groundY) , Quaternion.identity);
            }
            else{
                Instantiate(EnemyPrefab[enemyPrefabIndex], position, Quaternion.identity);
            }

            yield return new WaitForSeconds(nextSpawn);
        }
    }
}
