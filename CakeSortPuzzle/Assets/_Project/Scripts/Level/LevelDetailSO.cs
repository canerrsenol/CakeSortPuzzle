using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDetailSO", menuName = "Level/LevelDetailSO")]
public class LevelDetailSO : ScriptableObject
{
    public GameObject LevelPrefab;
    public List<Plate> LevelPlates;
}
