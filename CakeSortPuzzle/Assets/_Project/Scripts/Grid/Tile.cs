using UnityEngine;

public class Tile : MonoBehaviour
{
    private TilePosition tilePosition;
    private Plate plate;
    
    public void SetTilePosition(TilePosition tilePosition)
    {
        this.tilePosition = tilePosition;
    }
    
    public TilePosition GetTilePosition()
    {
        return tilePosition;
    }
    
    public void SetTilePlate(Plate tileObject)
    {
        this.plate = tileObject;
    }
    
    public Plate GetTilePlate()
    {
        return plate;
    }
    
    public bool IsTileEmpty()
    {
        return plate == null;
    }
}
