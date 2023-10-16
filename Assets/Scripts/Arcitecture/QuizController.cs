using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuizController : MonoBehaviour, IQuizController
{
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private QuizView _quizView;
    [SerializeField] private FinalScreenController _finalScreen;
    public UnityEvent onQuestionsEnds;


    private List<CO2QuizQuestion> _quizQuestions = new List<CO2QuizQuestion>();
    private int _currentQuestionIndex;
    private List<float> _co2Score = new List<float>();

    public event Action answerButtonClickEvent;
    
    private void Start()
    {
        _quizQuestions = LoadQuizQuestions();
    }

    private void OnEnable()
    {
        answerButtonClickEvent += () =>
        {
            _co2Score.Add(_quizView.GetAnswer().AnswerValue);
            Debug.Log(_co2Score);
            SetNextQuestion();
        };
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
        /*StartNewGame();*/
        onQuestionsEnds.Invoke();
        float CO2Value = CalcCO2Value();
        int treesValue = CalcTreesValue(CO2Value);
        StartCoroutine(_finalScreen.ShowFinalScreen(treesValue, CO2Value));
    }

    public void StartNewGame()
    {
        _co2Score = new List<float>();
        
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
    
    public void OnAnswerButtonClick(){
        answerButtonClickEvent.Invoke();
    }

    private float CalcCO2Value()
    {
        if (_co2Score.Count < 10) return 0;
        return _co2Score.Take(8).Sum() + _co2Score[8] * _co2Score[9];
    }

    private int CalcTreesValue(float value)
    {
        return Mathf.FloorToInt(value / 1.1f * 120);
    }

}
