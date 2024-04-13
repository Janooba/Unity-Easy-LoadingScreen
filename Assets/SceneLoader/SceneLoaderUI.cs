using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoaderUI : MonoBehaviour
{
    [Tooltip("Canvas Group handles fading the loading screen in and out")]
    public CanvasGroup canvasGroup;

    [Space]
    [Tooltip("Fade animation length for showing and hiding loading screen.")]
    public float loadingScreenFadeTime = 0.5f;
    [Tooltip("Loading screen will be displayed for this long at minimum.")]
    public float minTimeToLoad = 2f;

    [Space]
    [Tooltip("Optional. Image to use for progress. Must be set to Image Type: Filled.")]
    public Image progressBar;
    [Tooltip("Higher values will move the bar faster. Set to something like 1000 for almost instant.")]
    public float progressUpdateSpeed = 3f;

    [Space]
    [Tooltip("Optional. Text component for showing which scene is currently loading.")]
    public Text sceneNameText;
    [Tooltip("Optional. TMP Text component for showing which scene is currently loading.")]
    public TMPro.TextMeshProUGUI sceneNameTMPUGUI;
    [Tooltip("Optional. For when displaying the loading scene, formats the text using string.Format.")]
    public string nameTextFormat = "Loading Scene: {0}";

    [Space]
    [Tooltip("Optional. Text component for showing which scene is currently loading.")]
    public Text percentText;
    [Tooltip("Optional. TMP Text component for showing which scene is currently loading.")]
    public TMPro.TextMeshProUGUI percentTMPUGUI;
    [Tooltip("Optional. For when displaying the loading scene, formats the text using string.Format.")]
    public string percentTextFormat = "{0:0}%";

    [Header("Debug Values")]
    [Range(0f, 1f)]
    public float progress = 0f;
    public string sceneName = "";

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void FadeIn(Action callback = null)
    {
        ResetProgress();

        canvasGroup.alpha = 0f;

        // Animate in
        DOTween.To(
            () => canvasGroup.alpha,
            (x) => canvasGroup.alpha = x,
            1f,
            loadingScreenFadeTime)
            .SetUpdate(true)
            .OnComplete(() => { callback?.Invoke(); });
    }

    public void FadeOut()
    {
        // Animate out
        DOTween.To(
            () => canvasGroup.alpha,
            (x) => canvasGroup.alpha = x,
            0f,
            loadingScreenFadeTime)
            .SetUpdate(true);
    }

    private void Update()
    {
        if (progressBar)
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, progress, progressUpdateSpeed * Time.deltaTime);

        if (sceneNameText)
            sceneNameText.text = string.Format(nameTextFormat, sceneName);

        if (sceneNameTMPUGUI)
            sceneNameTMPUGUI.text = string.Format(nameTextFormat, sceneName);

        if (percentText)
            percentText.text = string.Format(percentTextFormat, progress * 100f);

        if (percentTMPUGUI)
            percentTMPUGUI.text = string.Format(percentTextFormat, progress * 100f);
    }

    private void ResetProgress()
    {
        progress = 0;
        if (progressBar)
            progressBar.fillAmount = 0f;
    }
}
