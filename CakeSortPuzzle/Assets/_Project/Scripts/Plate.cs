using UnityEngine;

public class Plate : MonoBehaviour, IDraggable, ITileObject
{
    private LayerMask groundLayer;

    public bool CanDrag { get; set; }

    public void OnDrag(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    public void OnDragBegan()
    {

    }

    public void OnDragEnded()
    {
        // Raycast to the ground layer get the tile position get the tile object from the tile and place the plate
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            var tilePosition = GridBase.Instance.GetTilePosition(hit.point);
            var tile = GridBase.Instance.GetTile(tilePosition);
            if(tile.IsTileEmpty())
            {
                tile.SetTileObject(this);
            }
            else
            {
                // Tile is not empty should return to the previous position
            }
        }
    }
}
