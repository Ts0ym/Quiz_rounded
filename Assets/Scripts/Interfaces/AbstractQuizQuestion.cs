using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractQuizQuestion
{
    private readonly string _question;
    private readonly string[] _answers;

    public string Question => _question;
    public string[] Answers => _answers;

    public AbstractQuizQuestion(string question, string[] answers)
    {
        this._question = question;
        this._answers = answers;
    }
    
}
