using UnityEngine;

public class Tile : MonoBehaviour
{
    private TilePosition tilePosition;
    
    private ITileObject tileObject;
    
    private Material material;
    
    [SerializeField] private Color emptyColor;
    
    [SerializeField] private Color occupiedColor;
    
    private void Awake()
    {
        material = GetComponentInChildren<MeshRenderer>().material;
    }
    
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
        material.color = tileObject == null ? emptyColor : occupiedColor;
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
