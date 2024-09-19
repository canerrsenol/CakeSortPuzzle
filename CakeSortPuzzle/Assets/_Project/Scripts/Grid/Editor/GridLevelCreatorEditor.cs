using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridLevelCreator))]
public class GridLevelCreatorEditor : Editor
{
    private int previousWidth;
    private int previousHeight;
    private float previousTileSize;

    private GridLevelCreator gridLevelCreator;
    
    private void OnEnable()
    {
        gridLevelCreator = (GridLevelCreator)target;
        previousWidth = gridLevelCreator.GridSize.x;
        previousHeight = gridLevelCreator.GridSize.y;
        previousTileSize = gridLevelCreator.TileSize;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (previousWidth != gridLevelCreator.GridSize.x
            || previousHeight != gridLevelCreator.GridSize.y)
        {
            gridLevelCreator.DestroyAllTiles();
            gridLevelCreator.CreateGrid();
            
            previousHeight = gridLevelCreator.GridSize.x;
            previousWidth = gridLevelCreator.GridSize.y;
        }

        if (!Mathf.Approximately(previousTileSize, gridLevelCreator.TileSize))
        {
            gridLevelCreator.UpdateGridSize();

            previousTileSize = gridLevelCreator.TileSize;
        }
        
        if (GUILayout.Button("Clear All"))
        {
            gridLevelCreator.DestroyAllTiles();
            gridLevelCreator.ResetGridSettings();
        }

        if (gridLevelCreator.GridSize.x > 0 && gridLevelCreator.GridSize.y > 0)
        {
            float inspectorWidth = EditorGUIUtility.currentViewWidth * .8f;
            float buttonSize = inspectorWidth / gridLevelCreator.GridSize.x;
            
            for (int y = gridLevelCreator.GridSize.y - 1; y >= 0; y--)
            {
                GUILayout.BeginHorizontal();
                
                for (int x = 0; x < gridLevelCreator.GridSize.x; x++)
                {
                    Texture2D previewTexture = null;
                    
                    if(gridLevelCreator.HasTileObject(new Vector2Int(x, y)))
                    {
                        int index = gridLevelCreator.GetTileObjectPrefabIndex(new Vector2Int(x, y));
                        previewTexture = AssetPreview.GetAssetPreview(gridLevelCreator.GetTileObjectPrefab(index));
                    }
                    
                    if (GUILayout.Button(previewTexture, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        gridLevelCreator.SetNextTileObject(new Vector2Int(x, y));
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
