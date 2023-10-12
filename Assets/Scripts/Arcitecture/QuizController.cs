using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour, IQuizController
{
    public TextAsset jsonFile;
    [SerializeField] private QuizView _quizView;

    private List<CO2QuizQuestion> _quizQuestions = new List<CO2QuizQuestion>();
    private int _currentQuestionIndex;
    
    private void Start()
    {
        Debug.Log(jsonFile);
        _quizQuestions = LoadQuizQuestions();
        StartNewGame();
    }

    private List<CO2QuizQuestion> LoadQuizQuestions()
    {
        List<CO2QuizQuestion> quizQuestions = DataLoader.GetListFromJSONfile<CO2QuizQuestion>(jsonFile.text);
        
        Debug.Log($"Loaded {quizQuestions.Count} questions from file");
        return quizQuestions;
    }
    
    private void OnQuesionsEnds()
    {
        Debug.Log($"There is no more questions!!!");
        StartNewGame();
    }

    private void StartNewGame()
    {
        if (_quizQuestions.Count == 0)
        {
            throw new Exception("There is no questions loaded!!!");
        }
        _currentQuestionIndex = 0;
        _quizView.SetQuestion(_quizQuestions[_currentQuestionIndex]);
    }
    
    public void SetNextQuestion()
    {
        int lastQuestionIndex = _quizQuestions.Count - 1;
        
        if (_currentQuestionIndex + 1 > lastQuestionIndex)
        {
            OnQuesionsEnds();
            return;
        }
        
        if (_currentQuestionIndex + 1 <= lastQuestionIndex)
        {
            ++_currentQuestionIndex;
            _quizView.SetQuestion(_quizQuestions[_currentQuestionIndex]);
        }
    }
    public int GetQuestionsAmount() => _quizQuestions.Count;
    public int GetCurrentQuestionIndex() => _currentQuestionIndex;

}
