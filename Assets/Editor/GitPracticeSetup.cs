using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GitPracticeSetup
{
    const string ScenePath = "Assets/Scenes/GitPractice.unity";

    public static void Execute()
    {
        string[] args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, "-labStage");
        string stage = index >= 0 ? args[index + 1] : "validate";
        if (stage == "base") CreateBase();
        else
        {
            EditorSceneManager.OpenScene(ScenePath);
            var platform = GameObject.Find("Platform");
            if (stage == "first")
            {
                platform.transform.localScale = new Vector3(6, 1, 4);
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "MarkerSphere";
                sphere.transform.position = new Vector3(0, 1, 0);
                sphere.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Marker.mat");
            }
            else if (stage == "second") platform.transform.localScale = new Vector3(4, 1, 6);
            if (stage == "first" || stage == "second")
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
        AssetDatabase.SaveAssets();
        var scale = GameObject.Find("Platform").transform.localScale;
        bool marker = GameObject.Find("MarkerSphere") != null;
        if (stage == "final" && (scale != new Vector3(6, 1, 6) || !marker))
            throw new Exception("Final scene validation failed");
        Debug.Log("LAB CHECK: stage=" + stage + "; platform=" + scale + "; marker=" + marker);
    }

    static void CreateBase()
    {
        Directory.CreateDirectory("Assets/Scenes");
        Directory.CreateDirectory("Assets/Materials");
        EditorSettings.serializationMode = SerializationMode.ForceText;
        VersionControlSettings.mode = "Visible Meta Files";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var blue = new Material(Shader.Find("Standard")) { color = new Color(0.15f, 0.45f, 0.8f) };
        var orange = new Material(Shader.Find("Standard")) { color = new Color(1f, 0.45f, 0.1f) };
        AssetDatabase.CreateAsset(blue, "Assets/Materials/Platform.mat");
        AssetDatabase.CreateAsset(orange, "Assets/Materials/Marker.mat");
        var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = "Platform";
        platform.transform.localScale = new Vector3(4, 1, 4);
        platform.GetComponent<Renderer>().sharedMaterial = blue;
        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        var camera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        camera.transform.position = new Vector3(9, 8, -11);
        camera.transform.LookAt(Vector3.zero);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.1f, 0.16f);
        var light = new GameObject("Directional Light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        light.transform.rotation = Quaternion.Euler(45, -30, 0);
        RenderSettings.ambientLight = new Color(0.45f, 0.45f, 0.45f);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.Refresh();
    }
}
