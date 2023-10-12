using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class QuizView : MonoBehaviour, IQuizView
{
    
    [SerializeField] private QuizController _quizController;
    
    [Header("UI Elements")]
    [SerializeField] private CarouselController _answersCarousel;
    [SerializeField] private QuestionSection _questionSection;
    public void SetQuestion(AbstractQuizQuestion question)
    {
        CO2QuizQuestion co2question = (CO2QuizQuestion)question;
        _answersCarousel.PlaceButtons(co2question.AnswerKeys, co2question.AnswerValues);
        
        _questionSection.SetQuestionText(co2question.Question);
        _questionSection.SetQuestionCounterText(_quizController.GetCurrentQuestionIndex() + 1, _quizController.GetQuestionsAmount());
    }
}
