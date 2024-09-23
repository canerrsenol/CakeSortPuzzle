using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalEventsSO", menuName = "ScriptableObjects/Events/GlobalEventsSO")]
public class GlobalEventsSO : ScriptableObject
{
    public Action<Tile> OnPlatePlacedOnTile;

    public Action OnPlateSorted;

    public Action OnMergeAnimationCompleted;

    public Action<Plate> OnPlateDestroyed;
}
