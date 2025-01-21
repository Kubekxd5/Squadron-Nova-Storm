using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile settings:")] public float weight = 1f;

    public Tile[] upNeighbours;
    public Tile[] rightNeighbours;
    public Tile[] bottomNeighbours;
    public Tile[] leftNeighbours;

    [Header("Spawn Points")] public Transform[] points;
    [Range(0f, 1f)] public float spawnChance = 0.5f;
    public bool canSpawn;

    [Header("Buildings")] public GameObject[] buildings;
    public float[] buildingWeights;


    private void Awake()
    {
        if (canSpawn) ActivateSpawnPoints();
    }

    private void ActivateSpawnPoints()
    {
        if (buildings.Length != buildingWeights.Length)
        {
            Debug.LogError("Buildings and buildingWeights arrays must have the same length!");
            return;
        }

        foreach (var spawnPoint in points)
            if (Random.value <= spawnChance)
                SpawnBuilding(spawnPoint);
    }

    private void SpawnBuilding(Transform spawnPoint)
    {
        var selectedBuildingIndex = GetWeightedRandomIndex(buildingWeights);

        if (selectedBuildingIndex != -1)
        {
            var prefabRotation = buildings[selectedBuildingIndex].transform.rotation;
            Instantiate(buildings[selectedBuildingIndex], spawnPoint.position, prefabRotation);
        }
    }

    private int GetWeightedRandomIndex(float[] weights)
    {
        var totalWeight = weights.Sum();

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("Total weight is zero or negative!");
            return -1;
        }

        var randomValue = Random.value * totalWeight;
        var cumulativeWeight = 0f;

        for (var i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight) return i;
        }

        return -1;
    }
}