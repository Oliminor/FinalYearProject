using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagers : MonoBehaviour
{
    [SerializeField] List<RespawnBaseEnemyData> respawnEnemyList = new();
    [SerializeField] Transform enemyParentTransform;
    void Awake()
    {
        LoadEnemies();
    }

    private void Start()
    {
        MainCharacterStats.onGameOver += RespawnEnemies;
    }

    /// <summary>
    /// Respawn the eliminated enemies, when the player dies
    /// </summary>
    public void RespawnEnemies()
    {
        for (int i = 0; i < respawnEnemyList.Count; i++)
        {
            if (respawnEnemyList[i].enemyUnit == null)
            {
                respawnEnemyList[i].savedGameobject.SetActive(true);

                GameObject go = Instantiate(respawnEnemyList[i].savedGameobject, respawnEnemyList[i].position, Quaternion.identity, enemyParentTransform);

                go.SetActive(false);

                respawnEnemyList[i].enemyUnit = go.transform;
            }
        }
    }

    /// <summary>
    /// Load in all enemies on the scene
    /// </summary>
    private void LoadEnemies()
    {
        BaseEnemy[] enemiesOnTheMap = FindObjectsOfType<BaseEnemy>();

        for (int i = 0; i < enemiesOnTheMap.Length; i++)
        {
            GameObject go = Instantiate(enemiesOnTheMap[i].gameObject, enemiesOnTheMap[i].transform.position, Quaternion.identity, enemyParentTransform);
            go.SetActive(false);

            respawnEnemyList.Add(new RespawnBaseEnemyData(enemiesOnTheMap[i].transform, go, enemiesOnTheMap[i].transform.position));
        }
    }
}

[System.Serializable]
public class RespawnBaseEnemyData
{
    public Transform enemyUnit;
    public GameObject savedGameobject;
    public Vector3 position;

    public RespawnBaseEnemyData(Transform _enemyUnit, GameObject _savedGameobjectm, Vector3 _position)
    {
        enemyUnit = _enemyUnit;
        savedGameobject = _savedGameobjectm;
        position = _position;
    }
}