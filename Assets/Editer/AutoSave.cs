using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutoSaveOnPlay
{
    static AutoSaveOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            Debug.Log("Auto-saving before entering Play mode...");
            SaveAll();
        }
    }

    private static void SaveAll()
    {
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }
}

class AutoSaveOnBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Auto-saving before building...");
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }
}
