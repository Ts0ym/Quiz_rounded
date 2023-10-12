using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionSection : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private TMP_Text _questionCounterText;

    [SerializeField] private AlphaTransition _sectionUIAlphaController;
    public void SetQuestionText(string text)
    {
        _questionText.text = text;
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
