using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionSection : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private TMP_Text _questionCounterText;

    [SerializeField] private AlphaTransition _sectionUIAlphaController;
    [SerializeField] private AlphaTransition _questionTextAlphaController;
    public IEnumerator SetQuestionText(string text)
    {
        float fadeDuration = 1.5f;
        
        _questionTextAlphaController.StartFadeOut(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        _questionText.text = text;
        _questionTextAlphaController.StartFadeIn(fadeDuration);
    }

    public void SetQuestionCounterText(int currentQuestion, int questionsAmount)
    {
        _questionCounterText.text = $"Вопрос {currentQuestion} из {questionsAmount}";
    }

    public void HideSectionUI()
    {
        _sectionUIAlphaController.StartFadeOut();
    }

    public void ShowSectionUI()
    {
        _sectionUIAlphaController.StartFadeIn();
    }
    
    
}
