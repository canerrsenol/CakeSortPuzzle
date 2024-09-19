using UnityEngine;

public class Tile : MonoBehaviour
{
    private TilePosition tilePosition;
    
    private ITileObject tileObject;
    
    public void SetTilePosition(TilePosition tilePosition)
    {
        this.tilePosition = tilePosition;
    }
    
    public TilePosition GetTilePosition()
    {
        return tilePosition;
    }
    
    public void SetTileObject(ITileObject tileObject)
    {
        this.tileObject = tileObject;
    }
    
    public ITileObject GetTileObject()
    {
        return tileObject;
    }
    
    public bool IsTileEmpty()
    {
        return tileObject == null;
    }
}
