using UnityEngine;

public class Tile : MonoBehaviour
{
    private TilePosition tilePosition;
    
    private GameObject tileObject;
    
    public void SetTilePosition(TilePosition tilePosition)
    {
        this.tilePosition = tilePosition;
    }
    
    public TilePosition GetTilePosition()
    {
        return tilePosition;
    }
    
    public void SetTileObject(GameObject tileObject)
    {
        this.tileObject = tileObject;
    }
    
    public GameObject GetTileObject()
    {
        return tileObject;
    }
    
    public bool IsTileEmpty()
    {
        return tileObject == null;
    }
}
