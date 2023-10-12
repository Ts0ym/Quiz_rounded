using System;
using System.Collections;
using UnityEngine;

enum TransitionDirection
{
    None,
    FadeIn,
    FadeOut
}

[RequireComponent(typeof(CanvasGroup))]
public class AlphaTransition: MonoBehaviour
{
    private TransitionDirection _state = TransitionDirection.None;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private IEnumerator Fade(float duration, TransitionDirection direction)
    {
        float startTime = Time.time;
        float startAlpha = _canvasGroup.alpha;
        float endAlpha = direction == TransitionDirection.FadeIn ? 1 : 0;
        
        _state = direction;
        
        while (Time.time - startTime < duration)
        {
            if (_state != direction)
            {
                break;
            }
            
            float t = (Time.time - startTime) / duration;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
    }
    
    private IEnumerator FadeInAndOut(float fadeInDuration, float fadeOutDuration, float delayDuration)
    {
        StartFadeIn(fadeInDuration);
        
        yield return new WaitForSeconds(fadeInDuration + delayDuration);
        
        StartFadeOut(fadeOutDuration);
    }

    public void SetTransparent()
    {
        _canvasGroup.alpha = 0f;
    }

    public void SetOpaque()
    {
        _canvasGroup.alpha = 1f;
    }

    public void StartFadeIn(float duration = 1f)
    {
        StartCoroutine(Fade(duration, TransitionDirection.FadeIn));
    }

    public void StartFadeOut(float duration = 1f)
    {
        StartCoroutine(Fade(duration, TransitionDirection.FadeOut));
    }

    public void StartFadeInAndOut(float fadeInDuration, float fadeOutDuration, float delayDuration)
    {
        StartCoroutine(FadeInAndOut(fadeInDuration, fadeOutDuration, delayDuration));
    }
}