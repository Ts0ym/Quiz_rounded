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
    public UnityEvent onTimerEnds;


    private List<CO2QuizQuestion> _quizQuestions = new List<CO2QuizQuestion>();
    private int _currentQuestionIndex;
    private List<float> _co2Score = new List<float>();
    private bool _isButtonBlocked = false;
    private bool _isPlayMode = false;
    
    public event Action answerButtonClickEvent;
    
    protected float _timer = 0;
    public float TotalTime = 5;
    private bool _isExited = false;
    
    private void Start()
    {
        _quizQuestions = LoadQuizQuestions();
    }

    private void Update()
    {
        if(_isPlayMode) IncreaseTimer();
    }

    private void OnEnable()
    {
        answerButtonClickEvent += () =>
        {
            if (_isButtonBlocked == false)
            {
                _co2Score.Add(_quizView.GetAnswer().AnswerValue);
                Debug.Log(_co2Score);
                _isButtonBlocked = true;
                SetNextQuestion();
                StartCoroutine(UnblockButtonCoroutine());
            }
        };

        onTimerEnds.AddListener(() =>
        {
            _isPlayMode = false;
        });
    }

    private IEnumerator UnblockButtonCoroutine()
    {
        yield return new WaitForSeconds(2);
        _isButtonBlocked = false;
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
        _isPlayMode = false;
        /*StartNewGame();*/
        onQuestionsEnds.Invoke();
        float CO2Value = CalcCO2Value();
        int treesValue = CalcTreesValue(CO2Value);
        StartCoroutine(_finalScreen.ShowFinalScreen(treesValue, CO2Value));
    }

    private void IncreaseTimer()
    {
        _timer += Time.deltaTime;
        
        if (_timer >= TotalTime && !_isExited)
        {
            _isExited = true;
            onTimerEnds.Invoke();
        }
    }

    private void ResetTimer()
    {
        _timer = 0;
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
        
        _isExited = false;
        _isPlayMode = true;
        ResetTimer();
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
        
        ResetTimer();
    }
    public int GetQuestionsAmount() => _quizQuestions.Count;
    public int GetCurrentQuestionIndex() => _currentQuestionIndex;
    
    public void OnAnswerButtonClick(){
        answerButtonClickEvent.Invoke();
    }

    public void OnExitButtonClick()
    {
        onTimerEnds.Invoke();
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
