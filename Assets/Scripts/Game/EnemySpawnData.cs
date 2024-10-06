using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "ScriptableObjects/SpawnData", order = 1)]
public class EnemySpawnData : ScriptableObject
{
    public enum Type { Melee, Mouth, Tank }
    public Type type;
    public Creature prefab;
    [Header("Times")]
    public float startTime;
    public float maxAmountTime;

    [Header("Spawn Rates")]
    public float minSpawnRate;
    public float maxSpawnRate;

    float lastSpawnTime;

    public bool ShouldSpawn(float time)
    {
        if (time < startTime) return false;

        float spawnRate = Mathf.Lerp(minSpawnRate, maxSpawnRate, Mathf.InverseLerp(startTime, maxAmountTime, time));

        //Modulate randomness
        float expectedSpawnTime = lastSpawnTime + (1 / spawnRate);
        float maxSpawnTime = expectedSpawnTime + (expectedSpawnTime - lastSpawnTime);
        float randMultipler = Mathf.Lerp(2, 0, Mathf.InverseLerp(lastSpawnTime, maxSpawnTime, time));

        bool shouldSpawn = Random.Range(0f, 1f) * randMultipler < spawnRate * Time.deltaTime;
        if (shouldSpawn) lastSpawnTime = time;
        return shouldSpawn;
    }
}
