using UnityEngine;

public interface IDraggable
{
    bool CanDrag { get; set; }
    void OnDragBegan();
    void OnDrag(Vector3 targetPosition);
    void OnDragEnded();
}