using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Builder : MonoBehaviour {
    [MenuItem("Build/Windows")]
    static void BuildWindowsPlayer()
    {
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, Application.dataPath + "/../Output/STGDemo.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    }
}
