using DG.Tweening;
using UnityEngine;

public class Plate : MonoBehaviour, IDraggable, ITileObject
{
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private PlateData plateData;

    private Vector3 initialPosition;

    public bool CanDrag { get; set; } = true;

    public void InitializePlate(PlateData plateData, Vector3 initialPosition)
    {
        this.plateData = plateData;
        this.initialPosition = initialPosition;
        transform.DOMove(initialPosition, 0.25f);
        SpawnCakeSlices();
    }

    private void SpawnCakeSlices()
    {
        for (int i = 0; i < plateData.cakeSlices.Length; i++)
        {
            if(plateData.cakeSlices[i] == null) continue;
            Instantiate(plateData.cakeSlices[i], Vector3.zero, Quaternion.Euler(Vector3.up * 45f * i),  transform);
        }
    }

    public void OnDrag(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    public void OnDragBegan(){}

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
