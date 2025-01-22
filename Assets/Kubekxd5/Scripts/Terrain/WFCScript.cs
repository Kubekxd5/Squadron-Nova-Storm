using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WFC_Script : MonoBehaviour
{
    public int width, height;
    public float cellSpacing;
    public Tile[] tiles;
    public List<Cell> gridComponents;
    public Cell cellObj;

    public Tile backupTile;
    public Tile tile1; // First tile to fill the rims of generated tiles
    public Tile tile2; // Second tile to fill the rest
    public float timeLimit = 5f; // Time in seconds after which filling occurs
    private int _dimension;

    private List<Cell> cellsToProcess; // List of cells that need to be processed
    private float elapsedTime; // Timer to track elapsed time

    private bool generationStopped; // Flag to stop further generation
    private int iteration;

    public void Awake()
    {
        _dimension = width * height;
        gridComponents = new List<Cell>(_dimension * _dimension);
        cellsToProcess = new List<Cell>(_dimension * _dimension);
        InitializeGrid();
    }

    public bool IsGenerationComplete()
    {
        return generationStopped;
    }

    private void InitializeGrid()
    {
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
        {
            var position = new Vector3(x * cellSpacing, 0, y * cellSpacing);
            var newCell = Instantiate(cellObj, position, Quaternion.identity);
            newCell.CreateCell(false, tiles);
            gridComponents.Add(newCell);
            cellsToProcess.Add(newCell);
        }

        // Start the entropy-based check (collapse one cell at a time)
        StartCoroutine(CheckEntropy());
    }

    // The original entropy-based method (collapsed one cell at a time)
    private IEnumerator CheckEntropy()
    {
        while (cellsToProcess.Count > 0 && !generationStopped)
        {
            // Sort the cells based on tile options length in ascending order (constrained cells first)
            cellsToProcess.Sort((a, b) => a.tileOptions.Length.CompareTo(b.tileOptions.Length));
            var cellToCollapse = cellsToProcess[0];

            CollapseCell(cellToCollapse);

            yield return new WaitForSeconds(0.01f); // Prevents freezing by yielding

            // After collapsing, remove the cell from the list
            cellsToProcess.RemoveAt(0);

            UpdateGeneration();

            // Check the time elapsed to decide when to stop generation
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= timeLimit && !generationStopped)
            {
                generationStopped = true;
                FillRimsWithTile1(); // Fill the rims of already generated tiles with tile1 after the time limit
            }

            // Wait for the next frame before continuing the process
            yield return null;
        }

        // After the process stops, fill the remaining inner cells with tile2
        if (generationStopped)
        {
            FillInnerCellsWithTile2();
            FillOuterEdgesWithTile2(); // Ensure the outermost edges are filled with tile2
        }
    }

    private void CollapseCell(Cell cellToCollapse)
    {
        cellToCollapse.isCollapsed = true;

        // Create a list of tiles with weights
        var weightedTiles = new List<Tile>();

        // Add each tile multiple times based on its weight
        foreach (var tile in cellToCollapse.tileOptions)
        {
            var count = Mathf.CeilToInt(tile.weight); // Use tile's weight to determine frequency
            for (var i = 0; i < count; i++) weightedTiles.Add(tile);
        }

        // Randomly select a tile from the weighted list
        var selectedTile = weightedTiles.Count > 0
            ? weightedTiles[Random.Range(0, weightedTiles.Count)]
            : backupTile;

        cellToCollapse.tileOptions = new[] { selectedTile };
        Instantiate(selectedTile, cellToCollapse.transform.position, selectedTile.transform.rotation);
    }

    private void UpdateGeneration()
    {
        // Reuse the existing list to minimize allocations
        var newGenerationCell = new List<Cell>(gridComponents);

        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
        {
            var index = x + y * width;

            if (gridComponents[index].isCollapsed) continue;

            var validOptions = new List<Tile>(tiles);

            CheckNeighborValidity(x, y, validOptions);

            if (validOptions.Count > 0)
                // Update the cell with the valid options
                newGenerationCell[index].RecreateCell(validOptions.ToArray());
        }

        gridComponents = newGenerationCell;
        iteration++;
    }

    private void CheckNeighborValidity(int x, int y, List<Tile> validOptions)
    {
        if (y > 0) // Check up neighbor
        {
            var up = gridComponents[x + (y - 1) * width];
            FilterValidOptions(ref validOptions, up.tileOptions, tile => tile.bottomNeighbours);
        }

        if (x < width - 1) // Check right neighbor
        {
            var right = gridComponents[x + 1 + y * width];
            FilterValidOptions(ref validOptions, right.tileOptions, tile => tile.leftNeighbours);
        }

        if (y < height - 1) // Check down neighbor
        {
            var down = gridComponents[x + (y + 1) * width];
            FilterValidOptions(ref validOptions, down.tileOptions, tile => tile.upNeighbours);
        }

        if (x > 0) // Check left neighbor
        {
            var left = gridComponents[x - 1 + y * width];
            FilterValidOptions(ref validOptions, left.tileOptions, tile => tile.rightNeighbours);
        }
    }

    private void FilterValidOptions(ref List<Tile> validOptions, Tile[] neighborOptions,
        Func<Tile, Tile[]> neighborFilter)
    {
        var validNeighborTiles = new HashSet<Tile>();

        foreach (var option in neighborOptions) validNeighborTiles.UnionWith(neighborFilter(option));

        validOptions.RemoveAll(tile => !validNeighborTiles.Contains(tile));
    }

    // Fill the rims (edges) of already generated tiles with tile1
    private void FillRimsWithTile1()
    {
        // Track cells that are collapsed
        var collapsedCells = new HashSet<Cell>();
        foreach (var cell in gridComponents)
            if (cell.isCollapsed)
                collapsedCells.Add(cell);

        // Now, iterate over the collapsed cells to identify the edge ones
        foreach (var cell in collapsedCells)
        {
            var x = Mathf.RoundToInt(cell.transform.position.x / cellSpacing);
            var y = Mathf.RoundToInt(cell.transform.position.z / cellSpacing);

            // Check the neighbors to find the edge cells
            if (x > 0 && x < width - 1 && y > 0 && y < height - 1) // Skip the outermost grid cells
            {
                // Check neighboring cells and fill if they're not collapsed
                if (x > 0 && !collapsedCells.Contains(gridComponents[x - 1 + y * width])) // Left
                    FillEdgeCell(x - 1, y);
                if (x < width - 1 && !collapsedCells.Contains(gridComponents[x + 1 + y * width])) // Right
                    FillEdgeCell(x + 1, y);
                if (y > 0 && !collapsedCells.Contains(gridComponents[x + (y - 1) * width])) // Down
                    FillEdgeCell(x, y - 1);
                if (y < height - 1 && !collapsedCells.Contains(gridComponents[x + (y + 1) * width])) // Up
                    FillEdgeCell(x, y + 1);
            }
        }
    }

    // Fills the given edge cell with tile1
    private void FillEdgeCell(int x, int y)
    {
        var index = x + y * width;
        if (!gridComponents[index].isCollapsed)
        {
            gridComponents[index].tileOptions = new[] { tile1 };
            Instantiate(tile1, gridComponents[index].transform.position, tile1.transform.rotation);
            gridComponents[index].isCollapsed = true;
        }
    }

    // Fill the remaining inner cells with tile2
    private void FillInnerCellsWithTile2()
    {
        for (var y = 1; y < height - 1; y++) // Skip the outer edges of the entire grid
        for (var x = 1; x < width - 1; x++)
        {
            var index = x + y * width;
            if (gridComponents[index].isCollapsed) continue;

            gridComponents[index].tileOptions = new[] { tile2 };
            Instantiate(tile2, gridComponents[index].transform.position, tile2.transform.rotation);
            gridComponents[index].isCollapsed = true;
        }
    }

    // Fill the outermost edge with tile2
    private void FillOuterEdgesWithTile2()
    {
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            if ((x == 0 || x == width - 1 || y == 0 || y == height - 1) && !gridComponents[x + y * width].isCollapsed)
            {
                gridComponents[x + y * width].tileOptions = new[] { tile2 };
                Instantiate(tile2, gridComponents[x + y * width].transform.position, tile2.transform.rotation);
                gridComponents[x + y * width].isCollapsed = true;
            }
    }
}