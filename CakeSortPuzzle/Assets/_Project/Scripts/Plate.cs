using DG.Tweening;
using UnityEngine;

public class Plate : MonoBehaviour, IDraggable, ITileObject
{
    [SerializeField] private LayerMask groundLayer;

    private Vector3 initialPosition;

    public bool CanDrag { get; set; } = true;

    public void SetInitialPosition(Vector3 initialPosition)
    {
        this.initialPosition = initialPosition;
    }

    public void OnDrag(Vector3 targetPosition)
    {
        // Move the plate to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
    }

    public void OnDragBegan()
    {

    }

    public void OnDragEnded()
    {
        CanDrag = false;

        // Raycast to the ground layer get the tile position get the tile object from the tile and place the plate
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            var gridBaseInstance = GridBase.Instance;

            var tilePosition = gridBaseInstance.GetTilePosition(hit.point);
            var tile = gridBaseInstance.GetTile(tilePosition);

            if(tile.IsTileEmpty())
            {
                tile.SetTileObject(this);
                transform.DOMove(gridBaseInstance.GetWorldPosition(tilePosition), 0.25f);
            }
            else
            {
                // Tile is not empty should return to the previous position
                transform.DOMove(initialPosition, 0.25f).OnComplete(() => CanDrag = true);
            }
        }
    }
}
