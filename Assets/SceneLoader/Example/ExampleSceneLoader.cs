using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneLoaderManager.LoadScene(sceneName, false, () => { Debug.Log("Loading Complete"); });
    }

    public void LoadMultiScenes()
    {
        SceneLoaderManager.LoadScenes(new string[] { "ExampleSceneTwo", "ExampleSceneAdditive" }, false, () => { Debug.Log("Loading Complete"); });
    }
}
