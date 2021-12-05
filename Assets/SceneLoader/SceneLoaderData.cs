using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneLoaderData : ScriptableObject
{
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void EnsureDataAssetCreation()
    {
        if (Instance)
            return;

        _instance = CreateInstance<SceneLoaderData>();
        AssetDatabase.CreateFolder("Assets/", "Resources");
        AssetDatabase.CreateAsset(_instance, "Assets/Resources/SceneLoaderData.asset");
        AssetDatabase.SaveAssets();
    }
#endif

    static SceneLoaderData _instance = null;
    public static SceneLoaderData Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.Load<SceneLoaderData>("SceneLoaderData");

            return _instance;
        }
    }

    public GameObject loadingScreenPrefab;
}
