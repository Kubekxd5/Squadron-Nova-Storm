using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile settings:")]
    public float weight = 1f;
    public Tile[] upNeighbours;
    public Tile[] rightNeighbours;
    public Tile[] bottomNeighbours;
    public Tile[] leftNeighbours;
}
