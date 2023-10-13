using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FinalScreenController : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _secondaryText;
    [SerializeField] private UnityEvent _onScreenShowEnds;
    [SerializeField] private AlphaTransition _alphaTransition => GetComponent<AlphaTransition>();
    
    [SerializeField] private float _fadeInDuration = 1;
    [SerializeField] private float _showDuration = 5;
    [SerializeField] private float _fadeOutDuration = 1;
    public void SetFinalScreenText(int treesValue, float co2Value)
    {
        _titleText.text = $"{treesValue} Деревьев";
        _secondaryText.text = $"{co2Value} тонн CO2 в год";
    }

    public IEnumerator ShowFinalScreen(int treesValue, float co2Value)
    {
        _alphaTransition.StartFadeIn(_fadeInDuration);
        SetFinalScreenText(treesValue, co2Value);
        yield return new WaitForSeconds(_showDuration);
        _alphaTransition.StartFadeOut(_fadeOutDuration);
        
        _onScreenShowEnds.Invoke();
    }
    
    
}
