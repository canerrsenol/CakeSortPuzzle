using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TileObjectFactory", menuName = "Factory/TileObjectFactory")]
public class TileObjectFactory : ScriptableObject
{
    [SerializeField] private GameObject[] tileObjects;
    
    private GameObject SpawnTileObject(int index)
    {
        return PrefabUtility.InstantiatePrefab(tileObjects[index]) as GameObject; 
    }
    
    public GameObject GetTileObjectPrefabAtIndex(int index)
    {
        return tileObjects[index];
    }
    
    public int GetTileObjectPrefabIndex(GameObject tileObject)
    {
        for (int i = 0; i < tileObjects.Length; i++)
        {
            if (tileObjects[i] == tileObject)
            {
                return i;
            }
        }

        return -1;
    }
    
    public GameObject SpawnNextTileObjectFromIndex(int index)
    {
        int nextIndex = index + 1;
        if (nextIndex >= tileObjects.Length)
        {
            nextIndex = 0;
        }

        return SpawnTileObject(nextIndex);
    }
}
