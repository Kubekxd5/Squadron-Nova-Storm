using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    [Header("Tile settings:")]
    public float weight = 1f;
    public Tile[] upNeighbours;
    public Tile[] rightNeighbours;
    public Tile[] bottomNeighbours;
    public Tile[] leftNeighbours;

    [Header("Spawn Points")] public Transform[] points;
    [Range(0f, 1f)] public float spawnChance = 0.5f;
    public Boolean canSpawn;
    
    [Header("Buildings")] public GameObject[] buildings;
    public float[] buildingWeights;

    

    private void Awake()
    {
        if (canSpawn)
        {
            ActivateSpawnPoints();
        }
    }

    private void ActivateSpawnPoints()
    {
        if (buildings.Length != buildingWeights.Length)
        {
            Debug.LogError("Buildings and buildingWeights arrays must have the same length!");
            return;
        }

        foreach (Transform spawnPoint in points)
        {
            if (Random.value <= spawnChance)
            {
                SpawnBuilding(spawnPoint);
            }
        }
    }
    
    private void SpawnBuilding(Transform spawnPoint)
    {
        int selectedBuildingIndex = GetWeightedRandomIndex(buildingWeights);

        if (selectedBuildingIndex != -1)
        {
            Quaternion prefabRotation = buildings[selectedBuildingIndex].transform.rotation;
            Instantiate(buildings[selectedBuildingIndex], spawnPoint.position, prefabRotation);

        }
    }
    
    private int GetWeightedRandomIndex(float[] weights)
    {
        float totalWeight = weights.Sum();

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("Total weight is zero or negative!");
            return -1;
        }

        float randomValue = Random.value * totalWeight;
        float cumulativeWeight = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return i;
            }
        }

        return -1;
    }
}
