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

    private List<Plate> activePlates = new List<Plate>();

    private void Awake()
    {
        gameManager = GameManager.Instance;
        levelManager = LevelManager.Instance;
    }

    private void OnEnable()
    {
        gameManager.OnGameStateChanged += OnGameStateChanged;
        globalEventsSO.OnPlatePlacedOnTile += OnPlatePlacedOnTile;
        globalEventsSO.OnPlateSorted += OnPlateSorted;
        globalEventsSO.OnMergeAnimationCompleted += CheckForMerge;
        globalEventsSO.OnPlateDestroyed += OnPlateDestroyed;
    }

    private void OnDisable()
    {
        gameManager.OnGameStateChanged -= OnGameStateChanged;
        globalEventsSO.OnPlatePlacedOnTile -= OnPlatePlacedOnTile;
        globalEventsSO.OnPlateSorted -= OnPlateSorted;
        globalEventsSO.OnMergeAnimationCompleted -= CheckForMerge;
        globalEventsSO.OnPlateDestroyed -= OnPlateDestroyed;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.LevelLoaded)
        {
            sortedPlateCount = 0;
        }
    }

    private void OnPlateSorted()
    {
        sortedPlateCount++;
        if (sortedPlateCount == levelManager.CurrentLevelData.TargetSortPlateCount)
        {
            DOVirtual.DelayedCall(1f, () => gameManager.ChangeGameState(GameState.LevelVictory));
        }
    }

    private void OnPlateDestroyed(Plate plate)
    {
        activePlates.Remove(plate);
        if (checkTiles.Count > 0 && checkTiles.Contains(plate.PlacedTile))
        {
            checkTiles.Remove(plate.PlacedTile);
        }
    }

    private void OnPlatePlacedOnTile(Tile tile)
    {
        activePlates.Add(tile.GetTilePlate());
        checkTiles.Add(tile);
        CheckForMerge();
    }

    private void CheckForMerge()
    {
        if (checkTiles.Count == 0) return;

        Tile checkTile = checkTiles.Last();
        Plate currentPlate = checkTile.GetTilePlate();

        var neighbourTiles = GridManager.Instance.GetNeighbourTiles(checkTile);
        var neighbourPlates = neighbourTiles.Select(tile => tile.GetTilePlate()).ToList();

        List<CakeSliceType> currentPlateCakeSliceTypes = currentPlate.GetAllCakeSliceTypes();
        if (currentPlateCakeSliceTypes.Count == 0) { checkTiles.Remove(checkTile); return; }
        CakeSliceType currentCakeType = currentPlateCakeSliceTypes[0];

        // Check if the current plate same type and search for the same type in neighbour plates
        if (currentPlate.IsPlateAllSameType())
        {
            for (int i = 0; i < neighbourTiles.Count; i++)
            {
                if (neighbourPlates[i].HasCakeType(currentCakeType))
                {
                    Merge(neighbourPlates[i], currentPlate, currentCakeType);
                    return;
                }
            }
        }

        // Check if the neighbour plates are all same type and search for the same type in current plate
        for (int j = 0; j < currentPlateCakeSliceTypes.Count; j++)
        {
            for (int i = 0; i < neighbourTiles.Count; i++)
            {
                if (neighbourPlates[i].IsPlateAllSameType() && neighbourPlates[i].HasCakeType(currentPlateCakeSliceTypes[j]))
                {
                    Merge(currentPlate, neighbourPlates[i], currentPlateCakeSliceTypes[j]);
                    checkTiles.Add(neighbourPlates[i].PlacedTile);
                    return;
                }
            }
        }

        // Check if the neighbour plates have the same type and search for the same type in current plate
        for (int j = 0; j < currentPlateCakeSliceTypes.Count; j++)
        {
            for (int i = 0; i < neighbourTiles.Count; i++)
            {
                if (neighbourPlates[i].HasCakeType(currentPlateCakeSliceTypes[j]))
                {
                    Merge(currentPlate, neighbourPlates[i], currentPlateCakeSliceTypes[j]);
                    checkTiles.Add(neighbourPlates[i].PlacedTile);
                    return;
                }
            }
        }

        checkTiles.Remove(checkTile);
        
        if (checkTiles.Count > 0) { CheckForMerge(); }
        else { DOVirtual.DelayedCall(1f, CheckForLose); }
    }

    private void CheckForLose()
    {
        int tileCount = GridManager.Instance.GetTileCount();
        if (activePlates.Count == tileCount)
        {
            gameManager.ChangeGameState(GameState.LevelDefeated);
        }
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
                break;
            }
        }

        // reorder and animate
        to.ReorderCakeSlices(true);
        from.ReorderCakeSlices(false);
    }
}
