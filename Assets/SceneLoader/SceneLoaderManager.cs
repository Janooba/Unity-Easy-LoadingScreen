using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    #region Static Stuff
    private static SceneLoaderManager _instance;
    public static SceneLoaderManager Instance
    {
        get
        {
            if (!_instance)
                _instance = FindObjectOfType<SceneLoaderManager>();

            if (!_instance)
                CreateInstance();

            return _instance;
        }
    }

    // Creates the loading screen itself
    private static void CreateInstance()
    {
        _instance = new GameObject("Scene Load Manager").AddComponent<SceneLoaderManager>();
        DontDestroyOnLoad(_instance.gameObject);
        var canvas = Instantiate(SceneLoaderData.Instance.loadingScreenPrefab, _instance.transform);
        _instance.loaderUI = canvas.GetComponent<SceneLoaderUI>();

        if (!_instance.loaderUI)
            Debug.LogError("No SceneLoaderUI exists on the loader canvas prefab!");
    }

    /// <summary>
    /// Loads the given scene with a loading screen. Ensure the
    /// SceneLoaderData in resources has a valid loading screen prefab.
    /// </summary>
    /// <param name="sceneName">String name of the scene</param>
    /// <param name="isAdditive">Whether to load this new scene additively</param>
    /// <param name="onLoadedCallback">Will be called when everything is done loading</param>
    public static void LoadScene(string sceneName, bool isAdditive, Action onLoadedCallback = null)
    {
        LoadScenes(new string[] { sceneName }, isAdditive, onLoadedCallback);
    }

    /// <summary>
    /// Loads the given scenes with a loading screen. Ensure the
    /// SceneLoaderData in resources has a valid loading screen prefab.
    /// </summary>
    /// <param name="sceneName">Array containing the string names of the scenes</param>
    /// <param name="isAdditive">Whether to load these new scenes additively. Should be set to true for multiple scenes.</param>
    /// <param name="onLoadedCallback">Will be called when everything is done loading</param>
    public static void LoadScenes(string[] sceneNames, bool isAdditive, Action onLoadedCallback = null)
    {
        if (!SceneLoaderData.Instance.loadingScreenPrefab)
        {
            Debug.LogError("No loading screen prefab set in the data object. See Resources/SceneLoaderData.");
            return;
        }

        AsyncOperation[] asyncOps = new AsyncOperation[sceneNames.Length];

        for (int i = 0; i < sceneNames.Length; i++)
        {
            asyncOps[i] = SceneManager.LoadSceneAsync(sceneNames[i], isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            asyncOps[i].allowSceneActivation = false;
        }

        Instance.ShowLoading(asyncOps, onLoadedCallback);
    }
    #endregion

    // Non Static Stuff
    public SceneLoaderUI loaderUI;

    private AsyncOperation currentOperation;
    private Stack<AsyncOperation> operationTasks;
    private AsyncOperation[] toLoad;
    private Action onLoaded;

    private bool isLoading = false;
    private float minTime = 0f;
    private int tasksToLoad = 0;

    private float timeStarted = 0f;
    private float ElapsedTime => Time.realtimeSinceStartup - timeStarted;

    /// <summary>
    /// Show the loading screen. You should probably use <see cref="SceneLoaderManager.LoadScene(string, bool, Action)"/>
    /// </summary>
    /// <param name="toLoad"></param>
    /// <param name="onLoaded"></param>
    public void ShowLoading(AsyncOperation[] toLoad, Action onLoaded)
    {
        isLoading = true;
        timeStarted = Time.realtimeSinceStartup;
        loaderUI.progress = 0;

        this.toLoad = toLoad;
        this.onLoaded = onLoaded;
        this.minTime = loaderUI.minTimeToLoad;

        loaderUI.FadeIn(StartLoading);
    }

    private void StartLoading()
    {
        operationTasks = new Stack<AsyncOperation>();
        AsyncOperation[] operations = toLoad;
        tasksToLoad = operations.Length;

        for (int i = 0; i < operations.Length; i++)
            operationTasks.Push(operations[i]);

        StartNextOperation();
    }

    private void StartNextOperation()
    {
        if (operationTasks.Count == 0)
            HideLoading();
        else
            currentOperation = operationTasks.Pop();
    }

    private void Update()
    {
        if (isLoading && operationTasks != null)
        {
            loaderUI.progress = currentOperation.progress / tasksToLoad;

            if (currentOperation.isDone)
                StartNextOperation();
            else if (minTime < ElapsedTime)
                currentOperation.allowSceneActivation = true;
        }
    }

    private void HideLoading()
    {
        loaderUI.progress = 1f;

        isLoading = false;
        operationTasks = null;
        onLoaded?.Invoke();
        tasksToLoad = 0;

        loaderUI.FadeOut();
    }
}