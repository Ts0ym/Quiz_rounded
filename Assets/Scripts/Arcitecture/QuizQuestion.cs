using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CO2QuizQuestion: AbstractQuizQuestion
{
    public CO2QuizQuestion(string question, string[] answers, List<string> answerKeys, List<float> answerValues) : base(question, answers)
    {
        this._answerKeys = answerKeys;
        this._answerValues = answerValues;
    }

    private readonly List<string> _answerKeys;
    private readonly List<float> _answerValues;

    public List<string> AnswerKeys => _answerKeys;
    public List<float> AnswerValues => _answerValues;
}
