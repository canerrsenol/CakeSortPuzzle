using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Plate : MonoBehaviour, IDraggable
{
    [SerializeField] private GlobalEventsSO globalEventsSO;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 initialPosition;

    public bool CanDrag { get; set; } = true;

    private CakeSlice[] cakeSlices = new CakeSlice[8];

    private Tile placedTile;

    public Tile PlacedTile => placedTile;

    public void InitializePlate(PlateData plateData, Vector3 initialPosition)
    {
        this.initialPosition = initialPosition;
        transform.DOMove(initialPosition, 0.25f);
        SpawnCakeSlices(plateData);
    }

    public bool HasCakeType(CakeSliceType cakeSliceType)
    {
        for (int i = 0; i < cakeSlices.Length; i++)
        {
            if (cakeSlices[i] == null) continue;
            if (cakeSlices[i].CakeSliceType == cakeSliceType) return true;
        }

        return false;
    }

    private void SpawnCakeSlices(PlateData plateData)
    {
        for (int i = 0; i < plateData.cakeSlices.Length; i++)
        {
            if (plateData.cakeSlices[i] == null) continue;
            GameObject cakeSlice = Instantiate(plateData.cakeSlices[i].cakeSlicePrefab, Vector3.zero, Quaternion.Euler(Vector3.up * 45f * i), transform);
            cakeSlice.transform.localPosition = Vector3.zero;

            cakeSlices[i] = cakeSlice.GetComponent<CakeSlice>();
            cakeSlices[i].SetCakeSliceType(plateData.cakeSlices[i].cakeSliceType);
        }
    }

    public List<CakeSliceType> GetAllCakeSliceTypes()
    {
        List<CakeSliceType> cakeSliceTypes = new List<CakeSliceType>();
        for (int i = 0; i < cakeSlices.Length; i++)
        {
            if (cakeSlices[i] == null) continue;
            cakeSliceTypes.Add(cakeSlices[i].CakeSliceType);
        }

        return cakeSliceTypes;
    }

    public bool IsPlateAllSameType()
    {
        CakeSliceType firstCakeSliceType = cakeSlices[0].CakeSliceType;
        for (int i = 1; i < cakeSlices.Length; i++)
        {
            if (cakeSlices[i] == null) return false;
            if (cakeSlices[i].CakeSliceType != firstCakeSliceType) return false;
        }

        return true;
    }

    public List<CakeSlice> GetAllCakeSlicesOfTargetType(CakeSliceType cakeSliceType)
    {
        List<CakeSlice> cakeSlices = new List<CakeSlice>();
        for (int i = 0; i < this.cakeSlices.Length; i++)
        {
            if (this.cakeSlices[i] == null) continue;
            if (this.cakeSlices[i].CakeSliceType == cakeSliceType)
            {
                cakeSlices.Add(this.cakeSlices[i]);
            }
        }

        return cakeSlices;
    }

    public bool HasEmptySlot()
    {
        for (int i = 0; i < cakeSlices.Length; i++)
        {
            if (cakeSlices[i] == null) return true;
        }

        return false;
    }

    public void AddCakeSlice(CakeSlice cakeSlice)
    {
        for (int i = 0; i < cakeSlices.Length; i++)
        {
            if (cakeSlices[i] == null)
            {
                cakeSlices[i] = cakeSlice;
                cakeSlice.transform.SetParent(transform);
                return;
            }
        }
    }

    public void RemoveCakeSlice(CakeSlice cakeSlice)
    {
        for (int i = 0; i < cakeSlices.Length; i++)
        {
            if (cakeSlices[i] == cakeSlice)
            {
                cakeSlices[i] = null;
                return;
            }
        }
    }

    public void ReorderCakeSlices()
    {
        // If all cake slices are null, then the plate is empty and should be destroyed
        if (cakeSlices.All(cakeSlice => cakeSlice == null))
        {
            placedTile.SetTileObject(null);
            Destroy(gameObject);
            return;
        }

        // Order the cake slices by their type
        var groupedCakeSlices = cakeSlices
            .Where(cakeSlice => cakeSlice != null)
            .OrderBy(cakeSlice => cakeSlice.CakeSliceType)
            .ToList();


        // Set the ordered cake slices list to the array
        for (int i = 0; i < cakeSlices.Length; i++)
        {
            if (i < groupedCakeSlices.Count)
            {
                cakeSlices[i] = groupedCakeSlices[i];
            }
            else
            {
                cakeSlices[i] = null;
            }
        }

        // Animate the cake slices to their new positions
        for (int i = 0; i < cakeSlices.Length; i++)
        {
            var cakeSlice = cakeSlices[i];
            if (cakeSlice != null)
            {
                if (cakeSlice.transform.localPosition != Vector3.zero)
                {
                    cakeSlice.transform.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.Linear);
                }

                Quaternion targetRotation = Quaternion.Euler(Vector3.up * 45f * i);
                if (cakeSlice.transform.localRotation != targetRotation)
                {
                    cakeSlice.transform.DOLocalRotateQuaternion(targetRotation, 1f).SetEase(Ease.Linear);
                }
            }
        }
    }

    public void OnDrag(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    public void OnDragBegan() { }

    public void OnDragEnded()
    {
        CanDrag = false;

        // Raycast to the ground layer get the tile position get the tile object from the tile and place the plate
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            var gridBaseInstance = GridManager.Instance;

            var tilePosition = gridBaseInstance.GetTilePosition(hit.point);
            if (!gridBaseInstance.IsValidTilePosition(tilePosition))
            {
                transform.DOMove(initialPosition, 0.25f).OnComplete(() => CanDrag = true);
                return;
            }
            var tile = gridBaseInstance.GetTile(tilePosition);

            if (tile.IsTileEmpty())
            {
                transform.DOMove(gridBaseInstance.GetWorldPosition(tilePosition), 0.25f).OnComplete(()
                =>
                {
                    placedTile = tile;
                    tile.SetTileObject(gameObject);
                    globalEventsSO.OnPlatePlaced?.Invoke(tile);
                });
            }
            else
            {
                // Tile is not empty should return to the previous position
                transform.DOMove(initialPosition, 0.25f).OnComplete(() => CanDrag = true);
            }
        }
    }
}
