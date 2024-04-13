using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
#region Static Stuff

    public static SceneLoaderManager Instance { get; private set; }

    public static event Action OnStartLoad;
    public static event Action OnLoaded;
    
    /// <summary>
    /// Use StartCoroutine() to start the AsyncOperation, which will immediately call the callback the frame
    /// the Scene has completed its loading.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="isAdditive"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IEnumerator SimpleCallbackLoad(string sceneName, bool isAdditive, Action callback)
    {
        if (!SceneManager.GetSceneByName(sceneName).IsValid())
        {
            Debug.LogWarning($"Scene name {sceneName} provided for SimpleLoad does not reflect any valid scenes in build");
            yield break;
        }

        yield return SceneManager.LoadSceneAsync(sceneName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);

        callback?.Invoke();
    }
    
    /// <summary>
    /// Use StartCoroutine() to start the AsyncOperation, which will immediately call the callback the frame
    /// the Scene has finished unloading.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IEnumerator SimpleCallbackUnLoad(string sceneName, Action callback)
    {
        if (!SceneManager.GetSceneByName(sceneName).IsValid())
        {
            Debug.LogWarning($"Scene name {sceneName} provided for SimpleUnLoad does not reflect any valid scenes in build");
            yield break;
        }

        // Safety measure so it is never attempted to unload a scene the same frame it has been loaded 
        yield return null;
        
        yield return SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        callback?.Invoke();
    }
    
    /// <summary>
    /// Loads the given scene with a loading screen. Ensure the
    /// SceneLoaderData in resources has a valid loading screen prefab.
    /// </summary>
    /// <param name="sceneName">String name of the scene</param>
    /// <param name="isAdditive">Whether to load this new scene additively</param>
    /// <param name="onLoadedCallback">Will be called when everything is done loading</param>
    public static void LoadScene(string sceneName, bool isAdditive = false, Action onLoadedCallback = null)
    {
        LoadScenes(new string[] { sceneName }, isAdditive, onLoadedCallback);
    }

    /// <summary>
    /// Loads the given scenes with a loading screen. Ensure the
    /// SceneLoaderData in resources has a valid loading screen prefab.
    /// </summary>
    /// <param name="sceneNames">Array containing the string names of the scenes</param>
    /// <param name="isAdditive">Whether to load these new scenes additively. Should be set to true for multiple scenes.</param>
    /// <param name="onLoadedCallback">Will be called when everything is done loading</param>
    public static void LoadScenes(string[] sceneNames, bool isAdditive = false, Action onLoadedCallback = null)
    {
        if (!Instance.loaderUI)
        {
            Debug.LogError("No loading screen UI set.");
            return;
        }
        OnStartLoad?.Invoke();
        Instance.ShowLoading(sceneNames, isAdditive, onLoadedCallback);
    }
    
#endregion

    // Non Static Stuff
    public SceneLoaderUI loaderUI;

    private string[] sceneNames;
    private AsyncOperation[] toLoad;
    private Action onLoaded;

    private bool isLoading = false;
    private float minTime = 0f;

    private float timeStarted = 0f;
    private float ElapsedTime => Time.realtimeSinceStartup - timeStarted;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Show the loading screen. You should probably use <see cref="SceneLoaderManager.LoadScene(string, bool, Action)"/>
    /// </summary>
    /// <param name="toLoad"></param>
    /// <param name="onLoaded"></param>
    public void ShowLoading(string[] sceneNames, bool isAdditive, Action onLoaded)
    {
        loaderUI.FadeIn(() =>
        {
            AsyncOperation[] toLoad = new AsyncOperation[sceneNames.Length];

            for (int i = 0; i < sceneNames.Length; i++)
            {
                toLoad[i] = SceneManager.LoadSceneAsync(sceneNames[i], isAdditive || i > 0 ? LoadSceneMode.Additive : LoadSceneMode.Single);
                toLoad[i].allowSceneActivation = i < toLoad.Length - 1;
            }

            isLoading = true;
            timeStarted = Time.realtimeSinceStartup;
            loaderUI.progress = 0;

            this.toLoad = toLoad;
            this.onLoaded = onLoaded;
            this.minTime = loaderUI.minTimeToLoad;
            this.sceneNames = sceneNames;
        });
    }

    private void Update()
    {
        if (isLoading)
        {
            float totalProgress = 0f;
            string sceneLoading = "";

            for (int i = 0; i < toLoad.Length; i++)
            {
                totalProgress += toLoad[i].progress;
                if (string.IsNullOrEmpty(sceneLoading) && toLoad[i].progress < 1f)
                    sceneLoading = sceneNames[i];
            }

            loaderUI.progress = totalProgress / toLoad.Length;
            loaderUI.sceneName = sceneLoading;

            // Waits for min time and last one before activating it
            if (minTime < ElapsedTime && toLoad[toLoad.Length - 1].progress >= 0.9f)
            {
                toLoad[toLoad.Length - 1].allowSceneActivation = true;
                
                // Invokes so that scene activation doesnt overtake the fade
                toLoad[toLoad.Length - 1].completed += (operation) => { Invoke(nameof(HideLoading), 0.2f); };
                // this is here to avoid multiple invokes of hideloading
                isLoading = false;
            }
        }
    }

    private void HideLoading()
    {
        loaderUI.progress = 1f;
        onLoaded?.Invoke();
        OnLoaded?.Invoke();
        loaderUI.FadeOut();
    }
}
