using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    [SerializeField, HideInInspector] private Vector2Int gridSize;
    [SerializeField, HideInInspector] private float tileSize;
    public float TileSize => tileSize;
    private Tile[,] tilesArray;

    protected void Awake()
    {        
        tilesArray = new Tile[gridSize.x, gridSize.y];
        
        List <Tile> tiles = GetComponentsInChildren<Tile>().ToList();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                tilesArray[x, z] = tiles[0];
                tiles[0].SetTilePosition(new TilePosition(x, z));
                tiles.RemoveAt(0);
            }
        }
    }

    public void SetGridSettings(Vector2Int gridSize, float tileSize)
    {
        this.gridSize = gridSize;
        this.tileSize = tileSize;
    }
    
    public int GetWidth() { return gridSize.x; }
    public int GetHeight() { return gridSize.y; }

    public Vector3 GetWorldPosition(TilePosition tilePosition)
    {
        return new Vector3(tilePosition.x, 0f, tilePosition.z) * tileSize;
    }
    
    public TilePosition GetTilePosition(Vector3 worldPosition)
    {
        return new TilePosition(
            Mathf.RoundToInt(worldPosition.x / tileSize),
            Mathf.RoundToInt(worldPosition.z / tileSize)
        );
    }
    
    public Tile GetTile(TilePosition tilePosition)
    {
        return tilesArray[tilePosition.x, tilePosition.z];
    }

    public int GetTileCount()
    {
        return gridSize.x * gridSize.y;
    }
    
    public bool IsValidTilePosition(TilePosition tilePosition)
    {
        return tilePosition.x >= 0 &&
               tilePosition.x < gridSize.x && 
               tilePosition.z >= 0 && 
               tilePosition.z < gridSize.y;
    }
}
