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
    public float loadingScreenFadeTime = 0.5f;
    public float minTimeToLoad = 2f;

    [Space]
    [Tooltip("Optional. Image to use for progress. Must be set to Image Type: Filled.")]
    public Image progressBar;
    [Tooltip("Higher values will move the bar faster. Set to something like 1000 for almost instant.")]
    public float progressUpdateSpeed = 3f;

    [Range(0f, 1f)]
    public float progress = 0f;

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
        {
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, progress, progressUpdateSpeed * Time.deltaTime);
        }
    }

    private void ResetProgress()
    {
        progress = 0;
        if (progressBar)
            progressBar.fillAmount = 0f;
    }
}
