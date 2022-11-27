#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;

public class BuildScript : MonoBehaviour
{
    [MenuItem("Builds/Run Builds/Build All")]
    public static void BuildAll()
    {
        BuildStandaloneOSX();
        BuildStandaloneWindows64();
    }

    [MenuItem("Builds/Run Builds/Build StandaloneOSX")]
    public static void BuildStandaloneOSX()
    {
        Build(GetScenes(), "./Builds/MacOSX/MacOXS.app", BuildTarget.StandaloneOSX);
    }

    [MenuItem("Builds/Run Builds/Build StandaloneWindows64")]
    public static void BuildStandaloneWindows64()
    {
        Build(GetScenes(), "./Builds/Windows/MacOXS.exe", BuildTarget.StandaloneWindows64);
    }

    private static void Build(string[] scenes, string buildPath, BuildTarget buildTarget, BuildOptions buildOptions = BuildOptions.None)
    {
        BuildReport report = BuildPipeline.BuildPlayer(scenes, buildPath, buildTarget, buildOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build test: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    private static string[] GetScenes()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
        }
        return scenes;
    }
}

#endif