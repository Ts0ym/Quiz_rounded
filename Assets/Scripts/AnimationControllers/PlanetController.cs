using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlanetController : MonoBehaviour
{
    /*[SerializeField] private CanvasGroup _planetCleanCG;*/
    [SerializeField] private CanvasGroup _planetDestroyedCG;
    [SerializeField] private CanvasGroup _cloudsCG;
    [SerializeField] private CanvasGroup _cleanSpaceCG;

    [SerializeField] private CanvasGroup _envCleanCG;

    [SerializeField] private Transform _environmentCleanTransform;
    [SerializeField] private Transform _environmentDestroyedTransform;

    private float _minEnvScale = 0.7f;
    
    private float[] _destroyedEnvScales = new float[10];
    private int _currentScaleStepDestroyed = 0;
    
    private float[] _cleanEnvScales = new float[10];
    private int _currentScaleStepClean = 0;

    private bool _isMutating = false;
    public bool IsMutating => _isMutating;
    
    private void Start()
    {
        _cloudsCG.alpha = 1;
        _planetDestroyedCG.alpha = 0;

        _envCleanCG.alpha = 1;
        

        /*_destroyedEnvScales = GenerateSteps(_minEnvScale, 1, 10);
        _cleanEnvScales = GenerateSteps(1, _minEnvScale, 10);*/

        SetStartParameters();
    }

    private void SetStartParameters()
    {
        /*_planetCleanCG.alpha = 1;*/
        
        
        _currentScaleStepClean = 0;



        /*_environmentCleanTransform.localScale = new Vector3(_cleanEnvScales[_currentScaleStepClean], _cleanEnvScales[_currentScaleStepClean], 1);
        _environmentDestroyedTransform.localScale = new Vector3(_destroyedEnvScales[_currentScaleStepClean], _destroyedEnvScales[_currentScaleStepClean], 1);*/
    }
    
    private static float[] GenerateSteps(float start, float end, int numSteps)
    {
        if (numSteps <= 1)
        {
            throw new ArgumentException("numSteps should be greater than 1.");
        }

        float[] steps = new float[numSteps];
        for (int i = 0; i < numSteps; i++)
        {
            float step = start + (end - start) * i / (numSteps - 1);
            steps[i] = step;
        }

        return steps;
    }

    private IEnumerator FadeCanvasGroupCoroutine(CanvasGroup canvasGroup, float targetAlpha, float fadeDuration = 1f)
    {
        _isMutating = true;
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        _isMutating = false;
    }
    
    private IEnumerator ScaleTransformCoroutine(Transform targetTransform, Vector3 targetScale, float scaleDuration = 1f)
    {
        _isMutating = true;
        Vector3 startScale = targetTransform.localScale;
        float time = 0;

        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / scaleDuration;
            targetTransform.localScale = Vector3.Lerp(startScale, targetScale, normalizedTime);
            yield return null;
        }

        targetTransform.localScale = targetScale;
        _isMutating = false;
    }

    public void SetNextPlanetState()
    {
        /*if (_currentScaleStepClean < 9 && _currentScaleStepDestroyed < 9)
        {
            _currentScaleStepClean++;
            _currentScaleStepDestroyed++;
        }
        

        Vector3 CleanEnvScale = new Vector3(_cleanEnvScales[_currentScaleStepClean],
            _cleanEnvScales[_currentScaleStepClean], 1);

        Vector3 DestroyedEnvScale = new Vector3(_destroyedEnvScales[_currentScaleStepDestroyed],
            _destroyedEnvScales[_currentScaleStepDestroyed], 1);
        
        StartCoroutine(ScaleTransformCoroutine(_environmentCleanTransform, CleanEnvScale));
        StartCoroutine(ScaleTransformCoroutine(_environmentDestroyedTransform, DestroyedEnvScale));*/

        StartCoroutine(FadeCanvasGroupCoroutine(_cloudsCG, _cloudsCG.alpha - 0.1f));
        StartCoroutine(FadeCanvasGroupCoroutine(_cleanSpaceCG, _cleanSpaceCG.alpha - 0.1f));
        StartCoroutine(FadeCanvasGroupCoroutine(_planetDestroyedCG, _planetDestroyedCG.alpha + 0.1f));
        StartCoroutine(FadeCanvasGroupCoroutine(_envCleanCG, _envCleanCG.alpha - 0.1f));


    }

    public void SetStartState()
    {
        /*SetStartParameters();
        Vector3 CleanEnvScale = new Vector3(_cleanEnvScales[_currentScaleStepClean],
            _cleanEnvScales[_currentScaleStepClean], 1);

        Vector3 DestroyedEnvScale = new Vector3(_destroyedEnvScales[_currentScaleStepDestroyed],
            _destroyedEnvScales[_currentScaleStepDestroyed], 1);
        
        StartCoroutine(ScaleTransformCoroutine(_environmentCleanTransform, CleanEnvScale));
        StartCoroutine(ScaleTransformCoroutine(_environmentDestroyedTransform, DestroyedEnvScale));*/
        
        StartCoroutine(FadeCanvasGroupCoroutine(_cloudsCG, 1));
        StartCoroutine(FadeCanvasGroupCoroutine(_cleanSpaceCG, 1));
        StartCoroutine(FadeCanvasGroupCoroutine(_planetDestroyedCG, 0));
        StartCoroutine(FadeCanvasGroupCoroutine(_envCleanCG, 1));
    }


}
