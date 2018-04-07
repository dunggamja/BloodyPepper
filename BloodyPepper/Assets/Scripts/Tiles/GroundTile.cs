using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class GroundTile : Tile
{
    [SerializeField]
    private Sprite[] tiles = null;

    [SerializeField]
    private Sprite preview = null;


#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Tiles/GroundTile")]
    public static void CreateFile()
    {
        string path = EditorUtility.SaveFilePanelInProject("GroundTile", "NewTile", "asset", "tile");
        if (string.IsNullOrEmpty(path))
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GroundTile>(), path);
    }
#endif
}
