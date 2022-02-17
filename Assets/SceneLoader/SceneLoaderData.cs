using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneLoaderData : ScriptableObject
{
// Auto creation of this is broken for the timebeing

//#if UNITY_EDITOR
//    [InitializeOnLoadMethod]
//    private static void EnsureDataAssetCreation()
//    {
//        if (Instance)
//            return;

//        Debug.Log("Loading SceneLoaderData.asset into instance...");
//        _instance = AssetDatabase.LoadAssetAtPath<SceneLoaderData>("Assets/Resources/SceneLoaderData.asset");

//        if (Instance)
//            return;

//        Debug.Log("No SceneLoaderData.asset found. Creating new...");

//        _instance = CreateInstance<SceneLoaderData>();
//        AssetDatabase.CreateFolder("Assets/", "Resources");
//        AssetDatabase.CreateAsset(_instance, "Assets/Resources/SceneLoaderData.asset");
//        AssetDatabase.SaveAssets();
//    }
//#endif

    static SceneLoaderData _instance = null;
    public static SceneLoaderData Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.Load<SceneLoaderData>("SceneLoaderData");

            if (!_instance)
                Debug.LogError("No SceneLoaderData Found!");

            return _instance;
        }
    }

    public GameObject loadingScreenPrefab;
}
