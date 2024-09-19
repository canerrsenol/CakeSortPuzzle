using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

public class GridLevelCreator : MonoBehaviour
{
    [SerializeField] private TileObjectFactory tileObjectFactory;
    
    [SerializeField] private GameObject tilePrefab;

    [SerializeField] private Vector2Int gridSize;
    public Vector2Int GridSize => gridSize;
    
    [SerializeField] private float tileSize = 1.1f;
    public float TileSize => tileSize;

    [HideInInspector] [SerializedDictionary("Tile Position", "Object Transform")] 
    public SerializedDictionary<Vector2Int, Transform> tileObjectsDictionary;
    
    [HideInInspector] public GameObject GridParent;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void CreateGrid()
    {
        if(gridSize.x <= 0 || gridSize.y <= 0) return;
        
        if (GridParent == null)
        {
            GameObject gridParent = new GameObject("GridBase");
            gridParent.gameObject.AddComponent<GridBase>().SetGridSettings(gridSize, tileSize);
            GridParent = gridParent;
        }
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GameObject tile = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                SetObjectPositionAccordingToGrid(tile.transform, new Vector2Int(x, y));
                tile.transform.parent = GridParent.transform;
                tile.transform.name = $"({x}, {y})";
            }
        }
            
        CheckOutOfBoundsTileObjects();
        EditorUtility.SetDirty(gameObject);
    }

    private void CheckOutOfBoundsTileObjects()
    {
        if (tileObjectsDictionary == null || tileObjectsDictionary.Count == 0) return;

        List<Vector2Int> keysToRemove = new List<Vector2Int>();

        foreach (var tileObject in tileObjectsDictionary)
        {
            if (tileObject.Key.x >= gridSize.x || tileObject.Key.y >= gridSize.y)
            {
                keysToRemove.Add(tileObject.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            DestroyTileObjectAtPosition(key);
        }
    }

    public void UpdateGridSize()
    {
        UpdateAllTilesPosition();
        UpdateAllTileObjectsPosition();
        if(GridParent != null) GridParent.GetComponent<GridBase>().SetGridSettings(gridSize, tileSize);
    }

    public void UpdateAllTilesPosition()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                SetObjectPositionAccordingToGrid(GridParent.transform.GetChild(x * gridSize.y + y), new Vector2Int(x, y));
            }
        }
    }
    
    public void UpdateAllTileObjectsPosition()
    {
        if (tileObjectsDictionary == null || tileObjectsDictionary.Count == 0) return;
        
        foreach (var tileObject in tileObjectsDictionary)
        {
            SetObjectPositionAccordingToGrid(tileObject.Value, new Vector2Int(tileObject.Key.x, tileObject.Key.y));
        }
    }

    private void SetObjectPositionAccordingToGrid(Transform targetTransform, Vector2Int tileTargetPosition)
    {
        Vector3 tilePosition = new Vector3(tileTargetPosition.x * tileSize, 0, tileTargetPosition.y * tileSize);
        targetTransform.position = tilePosition;
    }
    
    public void ResetGridSettings()
    {
        DestroyAllTiles();
        DestroyAllTileObjects();
        
        gridSize = Vector2Int.zero;
        tileSize = 1;
    }
    
    private void DestroyTileObjectAtPosition(Vector2Int tilePosition)
    {
        Transform tileObject = tileObjectsDictionary[tilePosition];
        tileObjectsDictionary.Remove(tilePosition);
        DestroyImmediate(tileObject.gameObject);
    }
    
    public void SetNextTileObject(Vector2Int tilePosition)
    {
        int prefabIndex = 0;
        
        if(tileObjectsDictionary.ContainsKey(tilePosition))
        {
            prefabIndex = tileObjectFactory.GetTileObjectPrefabIndex(
                PrefabUtility.GetCorrespondingObjectFromSource(tileObjectsDictionary[tilePosition].gameObject));
            DestroyTileObjectAtPosition(tilePosition);
        }
        
        GameObject tileObject = tileObjectFactory.SpawnNextTileObjectFromIndex(prefabIndex);
        if(tileObject != null)
        {
            tileObjectsDictionary ??= new SerializedDictionary<Vector2Int, Transform>();
            tileObjectsDictionary.Add(tilePosition, tileObject.transform);
            SetObjectPositionAccordingToGrid(tileObject.transform, tilePosition);
        }
        
        EditorUtility.SetDirty(gameObject);
    }
    
    public bool HasTileObject(Vector2Int tilePosition)
    {
        return tileObjectsDictionary.ContainsKey(tilePosition);
    }
    
    public int GetTileObjectPrefabIndex(Vector2Int tilePosition)
    {
        return tileObjectFactory.GetTileObjectPrefabIndex(
            PrefabUtility.GetCorrespondingObjectFromSource(tileObjectsDictionary[tilePosition].gameObject));
    }

    public GameObject GetTileObjectPrefab(int index)
    {
        return tileObjectFactory.GetTileObjectPrefabAtIndex(index);
    }

    public void DestroyAllTiles()
    {
        DestroyImmediate(GridParent.gameObject);
    }
    
    private void DestroyAllTileObjects()
    {
        foreach (var tileObject in tileObjectsDictionary)
        {
            DestroyImmediate(tileObject.Value.gameObject);
        }

        tileObjectsDictionary.Clear();
        EditorUtility.SetDirty(gameObject);
    }
}
