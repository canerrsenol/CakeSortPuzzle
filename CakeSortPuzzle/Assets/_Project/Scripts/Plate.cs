using DG.Tweening;
using UnityEngine;

public class Plate : MonoBehaviour, IDraggable, ITileObject
{
    [SerializeField] private LayerMask groundLayer;

    private Vector3 initialPosition;

    public bool CanDrag { get; set; } = true;

    private CakeSlice[] cakeSlices = new CakeSlice[8];

    public void InitializePlate(PlateData plateData, Vector3 initialPosition)
    {
        this.initialPosition = initialPosition;
        transform.DOMove(initialPosition, 0.25f);
        SpawnCakeSlices(plateData);
    }

    private void SpawnCakeSlices(PlateData plateData)
    {
        for (int i = 0; i < plateData.cakeSlices.Length; i++)
        {
            if(plateData.cakeSlices[i] == null) continue;
            GameObject cakeSlice = Instantiate(plateData.cakeSlices[i].cakeSlicePrefab, Vector3.zero, Quaternion.Euler(Vector3.up * 45f * i), transform);
            cakeSlice.transform.localPosition = Vector3.zero;
            
            cakeSlices[i] = cakeSlice.GetComponent<CakeSlice>();
            cakeSlices[i].SetCakeSliceType(plateData.cakeSlices[i].cakeSliceType);
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
            if(!gridBaseInstance.IsValidTilePosition(tilePosition)){
                transform.DOMove(initialPosition, 0.25f).OnComplete(() => CanDrag = true);
                return;
            } 
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
