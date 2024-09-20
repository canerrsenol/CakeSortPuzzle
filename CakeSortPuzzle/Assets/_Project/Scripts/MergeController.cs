using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [SerializeField] private GlobalEventsSO globalEventsSO;

    private Stack<Tile> checkTiles = new Stack<Tile>();

    private void OnEnable()
    {
        globalEventsSO.OnPlatePlaced += OnPlatePlaced;
    }

    private void OnDisable()
    {
        globalEventsSO.OnPlatePlaced -= OnPlatePlaced;
    }

    private void OnPlatePlaced(Tile tile)
    {
        checkTiles.Push(tile);
        CheckForMerge();
    }

    private void CheckForMerge()
    {
        Plate currentPlate = checkTiles.Peek().GetTileObject().GetComponent<Plate>();
        var neighbourPlates = GetNeighbourList(checkTiles.Peek());

        
    }

    private void Update()
    {
        
    }

    private List<Plate> GetNeighbourList(Tile currentTile)
    {
        GridManager gridSystem = GridManager.Instance;

        List<Plate> neighbourList = new List<Plate>();
        TilePosition gridPosition = currentTile.GetTilePosition();

        List<TilePosition> neighbourOffsets = new List<TilePosition>
        {
            new TilePosition(-1, 0),  // Left
            new TilePosition(1, 0),   // Right
            new TilePosition(0, -1),  // Down
            new TilePosition(0, 1),   // Up
        };

        foreach (TilePosition offset in neighbourOffsets)
        {
            TilePosition neighbourPosition = gridPosition + offset;
            if(gridSystem.IsValidTilePosition(neighbourPosition))
            {
                Tile neighbourTile = gridSystem.GetTile(neighbourPosition);
                if (!neighbourTile.IsTileEmpty())
                {
                    neighbourList.Add(neighbourTile.GetTileObject().GetComponent<Plate>());
                }
            }

        }

        return neighbourList;
    }
}
