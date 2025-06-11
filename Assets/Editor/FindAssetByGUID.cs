using UnityEditor;
using UnityEngine;

/// <summary>Used to find an asset by its internal Unity guid stored within .meta files.</summary>
/// <remarks>This is NOT the same as the GUID we use for save and load!</remarks>
public class GUIDToAssetPath : EditorWindow
{
    string guid = "";
    string path = "";
    [MenuItem("Assets/Custom/GetAssetByGUID")]
    static void CreateWindow()
    {
        GUIDToAssetPath window = (GUIDToAssetPath)EditorWindow.GetWindowWithRect(typeof(GUIDToAssetPath), new Rect(0, 0, 400, 120));
    }

    void OnGUI()
    {
        GUILayout.Label("Enter guid");
        guid = GUILayout.TextField(guid);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Get Asset Path", GUILayout.Width(120)))
            path = GetAssetPath(guid);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Abort", GUILayout.Width(120)))
            Close();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Label(path);
    }
    static string GetAssetPath(string guid)
    {
        string p = AssetDatabase.GUIDToAssetPath(guid);
        Debug.Log(p);
        if (p.Length == 0) p = "not found";
        return p;
    }
}


