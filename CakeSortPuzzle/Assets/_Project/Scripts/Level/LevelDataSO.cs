using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDetailSO", menuName = "Level/LevelDetailSO")]
public class LevelDataSO : ScriptableObject
{
    public GameObject LevelPrefab;
    public List<PlateData> LevelPlates;
    public int TargetSortPlateCount;
}
