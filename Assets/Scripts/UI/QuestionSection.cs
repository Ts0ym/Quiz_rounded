using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionSection : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private TMP_Text _questionCounterText;
    
    public void SetQuestionText(string text)
    {
        _questionText.text = text;
    }

    public void SetQuestionCounterText(int currentQuestion, int questionsAmount)
    {
        _questionCounterText.text = $"Вопрос {currentQuestion} из {questionsAmount}";
    }
}
