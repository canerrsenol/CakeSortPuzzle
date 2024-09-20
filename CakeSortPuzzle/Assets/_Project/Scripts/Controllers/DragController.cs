using Blended;
using UnityEngine;

public class DragController : MonoBehaviour
{
    [SerializeField] private LayerMask draggableLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 dragOffset;
    [SerializeField] private float dragSpeed = 5f;
    private IDraggable draggable;

    private void OnEnable()
    {
        TouchManager.Instance.onTouchBegan += OnTouchBegan;
        TouchManager.Instance.onTouchMoved += OnTouchMoved;
        TouchManager.Instance.onTouchEnded += OnTouchEnded;
    }

    private void OnDisable()
    {
        TouchManager.Instance.onTouchBegan -= OnTouchBegan;
        TouchManager.Instance.onTouchMoved -= OnTouchMoved;
        TouchManager.Instance.onTouchEnded -= OnTouchEnded;
    }

    private void OnTouchBegan(TouchInput touch)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.FirstScreenPosition), out RaycastHit hit, Mathf.Infinity, draggableLayer))
        {
            if (hit.collider.TryGetComponent(out IDraggable draggable))
            {
                if (!draggable.CanDrag) return;
                this.draggable = draggable;
                draggable.OnDragBegan();
            }
        }
    }

    private void OnTouchMoved(TouchInput touch)
    {
        // If there is no draggable object, return
        if (draggable == null) return;
        // Raycast to the ground layer to get the position of the touch on the ground add drag offset
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.ScreenPosition), out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = hit.point + dragOffset;
            draggable.OnDrag(targetPosition);
        }
    }

    private void OnTouchEnded(TouchInput touch)
    {
        if (draggable != null)
        {
            draggable.OnDragEnded();
            draggable = null;
        }
    }
}
