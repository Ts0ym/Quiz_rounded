using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour
{
    private Button _button;
    private TMP_Text _buttonText;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _buttonText = _button?.GetComponentInChildren<TMP_Text>();
    }

    public void SetText(string text)
    {
        _buttonText.text = text;
    }

    public void SetImage(Image image)
    {
        _button.image = image;
    }

    public void SetSize(Vector2 size)
    {
        RectTransform rectTransform = _button.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
    }

    public void SetOnClickFunction(Action action)
    {
        _button.onClick.AddListener(() => action());
    }

    public void SetButton(string text, Action action)
    {
        SetText(text);
        SetOnClickFunction(action);
    }
}
