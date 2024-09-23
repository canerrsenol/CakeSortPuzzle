using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MergeController : MonoBehaviour
{
    [SerializeField] private GlobalEventsSO globalEventsSO;

    private List<Tile> checkTiles = new List<Tile>();

    private GameManager gameManager;

    private LevelManager levelManager;

    private int sortedPlateCount = 0;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        levelManager = LevelManager.Instance;
    }

    private void OnEnable()
    {
        gameManager.OnGameStateChanged += OnGameStateChanged;
        globalEventsSO.OnPlatePlaced += OnPlatePlaced;
        globalEventsSO.OnPlateSorted += OnPlateSorted;
        globalEventsSO.OnMergeAnimationCompleted += CheckForMerge;
        globalEventsSO.OnPlateDestroyed += CheckStackHasDestroyedPlate;
    }

    private void OnDisable()
    {
        gameManager.OnGameStateChanged -= OnGameStateChanged;
        globalEventsSO.OnPlatePlaced -= OnPlatePlaced;
        globalEventsSO.OnPlateSorted -= OnPlateSorted;
        globalEventsSO.OnMergeAnimationCompleted -= CheckForMerge;
        globalEventsSO.OnPlateDestroyed -= CheckStackHasDestroyedPlate;
    }

    private void OnGameStateChanged(GameState state)
    {
        if(state == GameState.LevelLoaded)
        {
            sortedPlateCount = 0;
        }
    }

    private void OnPlateSorted()
    {
        sortedPlateCount++;
        if(sortedPlateCount == levelManager.CurrentLevelData.TargetSortPlateCount)
        {
            gameManager.ChangeGameState(GameState.LevelVictory);
        }
    }

    private void CheckStackHasDestroyedPlate(Plate plate)
    {
        if(checkTiles.Count > 0 && checkTiles.Contains(plate.PlacedTile))
        {
            checkTiles.Remove(plate.PlacedTile);
        }
    }

    private void OnPlatePlaced(Tile tile)
    {
        checkTiles.Add(tile);
        CheckForMerge();
    }

    private void CheckForMerge()
    {
        if (checkTiles.Count == 0) return;

        Tile checkTile = checkTiles.Last();
        Plate currentPlate = checkTile.GetTilePlate();
        var neighbourPlates = GetNeighbourPlates(checkTile);

        List<CakeSliceType> currentPlateCakeSliceTypes = currentPlate.GetAllCakeSliceTypes();
        if(currentPlateCakeSliceTypes.Count == 0) { checkTiles.Remove(checkTile); return; }
        CakeSliceType currentCakeType = currentPlateCakeSliceTypes[0];
        Plate targetPlate = null;

        // Check if the current plate same type and search for the same type in neighbour plates
        if(currentPlate.IsPlateAllSameType())
        {
            for (int i = 0; i < neighbourPlates.Count; i++)
            {
                if(neighbourPlates[i].HasCakeType(currentCakeType))
                {
                    targetPlate = neighbourPlates[i];
                    Merge(targetPlate, currentPlate, currentCakeType);
                    return;
                }
            }
        }

        // Check if the neighbour plates are same type and search for the same type in current plate
        for(int j= 0; j< currentPlateCakeSliceTypes.Count; j++)
        {
            for (int i = 0; i < neighbourPlates.Count; i++)
            {
                if (neighbourPlates[i].IsPlateAllSameType() && neighbourPlates[i].HasCakeType(currentPlateCakeSliceTypes[j]))
                {
                    targetPlate = neighbourPlates[i];
                    Merge(currentPlate, targetPlate, currentPlateCakeSliceTypes[j]);
                    checkTiles.Add(targetPlate.PlacedTile);
                    return;
                }
                if (neighbourPlates[i].HasCakeType(currentPlateCakeSliceTypes[j]))
                {
                    targetPlate = neighbourPlates[i];
                    Merge(currentPlate, targetPlate, currentPlateCakeSliceTypes[j]);
                    checkTiles.Add(targetPlate.PlacedTile);
                    return;
                }
            }
        }

        checkTiles.Remove(checkTile);
        if (checkTiles.Count > 0) { CheckForMerge(); }
    }

    private void Merge(Plate from, Plate to, CakeSliceType cakeSliceType)
    {
        List<CakeSlice> cakeSlices = from.GetAllCakeSlicesOfTargetType(cakeSliceType);

        for (int i = 0; i < cakeSlices.Count; i++)
        {
            if (to.HasEmptySlot())
            {
                to.AddCakeSlice(cakeSlices[i]);
                from.RemoveCakeSlice(cakeSlices[i]);
            }
            else
            {
                return;
            }

            // First reorder the target plate then reorder the source plate
            to.ReorderCakeSlices();
            DOVirtual.DelayedCall(.25f, from.ReorderCakeSlices);
        }
    }

    private List<Plate> GetNeighbourPlates(Tile currentTile)
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
            if (gridSystem.IsValidTilePosition(neighbourPosition))
            {
                Tile neighbourTile = gridSystem.GetTile(neighbourPosition);
                if (!neighbourTile.IsTileEmpty())
                {
                    neighbourList.Add(neighbourTile.GetTilePlate().GetComponent<Plate>());
                }
            }

        }

        return neighbourList;
    }
}
