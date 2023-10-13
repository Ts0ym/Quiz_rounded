using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class AnswerInfo
{
    public string AnswerKey;
    public float AnswerValue;
    public float AnswerAngle;

    public AnswerInfo(string answerKey, float answerValue, float answerAngle)
    {
        this.AnswerKey = answerKey;
        this.AnswerValue = answerValue;
        this.AnswerAngle = answerAngle;
    }
}

public class CarouselController : MonoBehaviour
{
    [Header("Carousel config")]
    [SerializeField] private CustomButton buttonPrefab; // Префаб кнопки, которую вы хотите разместить
    [SerializeField] private float radius = 100f; // Радиус круга
    private Transform _carouselTransform => GetComponent<Transform>(); // Родительский объект для кнопок
    [SerializeField] private float minAngle = 0f; // Минимальный угол в градусах
    [SerializeField] private float maxAngle = 180f; // Максимальный угол в градусах
    [SerializeField] private float _startPosY;
    
    [Header("Input prefs")]
    [SerializeField] private InputManager _inputManager;
    private bool _isDragging = false;
    
    [Header("Rotating data")]
    [SerializeField] private float _rotationSpeed = 1;
    private Vector2 _startTouchPosition;
    [SerializeField] private bool _isLocked = false;

    [Header("Current answers data")]
    private List<AnswerInfo> _answers = new List<AnswerInfo>();
    private AnswerInfo _changedAnswer;
    public int _changedAnswerIndex;

    private List<CustomButton> _currentButtons = new List<CustomButton>();

    [Header("Sound prefs")]
    [SerializeField] private AudioController _audioController;
    [SerializeField] private float minRotationSpeed = 1.0f;
    [SerializeField] private int positionsCount = 8; // Количество заранее просчитанных позиций для воспроизведения звука
    [SerializeField] private float maxRotationSpeed = 100f; // Максимальная скорость вращения для максимальной громкости звука
    [SerializeField] private float minVolume = 0.1f; // Минимальная громкость
    [SerializeField] private float maxVolume = 1.0f; // Максимальная громкость
    private int _currentPosition = 0;
    private float _lastRotation = 0f;

    [SerializeField] private UIAnimationController _animationController;
    private void Update()
    {
        if (!_animationController.GetState())
        {
            return;
        }
        
        if (!_isLocked)
        {
            TouchDragging();
            
        }
        CircleSoundManager();
    }

    private void OnEnable()
    {
        _inputManager.IsTouched.started += OnTouchStarted;
        _inputManager.IsTouched.canceled += OnTouchCanceled;
    }

    private void OnTouchStarted(InputAction.CallbackContext ctx)
    {
        _isDragging = true;
        _startTouchPosition = _inputManager.TouchPosition.ReadValue<Vector2>();
    }
    
    private void OnTouchCanceled(InputAction.CallbackContext ctx)
    {
        if (_answers.Count == 0)
        {
            return;
        }

        _isDragging = false;

        if (Vector2.Distance(_startTouchPosition, _inputManager.TouchPosition.ReadValue<Vector2>()) > 0.001f)
        {
            SetChangedAnswer(GetNearestAnswerIndex(_carouselTransform.rotation.eulerAngles.z));
        }
    }
    
    private void TouchDragging()
    {
        if (!_isDragging)
        {
            return;
        }

        Vector2 currentTouchPosition = _inputManager.TouchPosition.ReadValue<Vector2>();

        float deltaX = currentTouchPosition.x - _startTouchPosition.x;
        float deltaY = currentTouchPosition.y - _startTouchPosition.y;
        
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            float angle = Vector2.SignedAngle(_startTouchPosition, currentTouchPosition);
            _carouselTransform.Rotate(Vector3.forward, -angle * _rotationSpeed);
        }
             
        _startTouchPosition = currentTouchPosition;
    }

    public IEnumerator RotateTo(Quaternion targetRotation)
    {
        _isLocked = true;
        
        Quaternion startRotation = _carouselTransform.rotation;
        float elapsedTime = 0f;
        
        while (elapsedTime < Mathf.Abs(targetRotation.z - startRotation.z) / _rotationSpeed)
        {
            
            float t = elapsedTime / (Mathf.Abs(targetRotation.z - startRotation.z) / _rotationSpeed);
            
            _carouselTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _carouselTransform.rotation = targetRotation;
        _isLocked = false;
    }

    private void StartRotateTo(Quaternion targetRotation)
    {
        StartCoroutine(RotateTo(targetRotation));
    }

    private int GetNearestAnswerIndex(float currentAngle)
    {
        AnswerInfo nearestAnswer = _answers[0];
        int nearestAnswerIndex = 0;
        float minDiff = Mathf.Abs(Mathf.Abs(Mathf.DeltaAngle(nearestAnswer.AnswerAngle, currentAngle)));
        
        for (int i = 1; i < _answers.Count; i++)
        {

            float currentDiff = Mathf.Abs(Mathf.DeltaAngle(_answers[i].AnswerAngle, currentAngle));
            if (currentDiff < minDiff)
            {
                nearestAnswer = _answers[i];
                nearestAnswerIndex = i;
                minDiff = currentDiff;
            }
        }
        return nearestAnswerIndex;
    }

    private void SetChangedAnswer(int answerIndex)
    {
        if (!(answerIndex >= 0 && answerIndex < _answers.Count))
        {
            return;
        }
        
        _changedAnswer = _answers[answerIndex];
        _changedAnswerIndex = answerIndex;
        StartRotateTo(Quaternion.Euler(0, 0, _changedAnswer.AnswerAngle));
    }

    private void CircleSoundManager()
    {
        float currentRotation = transform.rotation.eulerAngles.z;
        // Вычисляем разницу между текущим и предыдущим углом
        float rotationDelta = Mathf.DeltaAngle(currentRotation, _lastRotation);

        // Рассчитываем текущую скорость вращения
        float rotationSpeed = Mathf.Abs(rotationDelta) / Time.deltaTime;

        // Определяем уровень громкости в зависимости от скорости
        float volume = Mathf.Lerp(minVolume, maxVolume, rotationSpeed / maxRotationSpeed);

        // Вычисляем текущую позицию в зависимости от текущего угла
        int newPosition = (int)((currentRotation / 360f) * positionsCount);

        // Если позиция изменилась
        if (newPosition != _currentPosition)
        {
            _audioController.PlayClickSound(volume);
            _currentPosition = newPosition;
        }
        _lastRotation = currentRotation;
    }

    private void ClearButtons()
    {
        if (_currentButtons.Count == 0)
        {
            return;
        }
        
        foreach (CustomButton button in _currentButtons)
        {
            Destroy(button.gameObject);
        }
        
        _currentButtons.Clear();
        _answers.Clear();
    }

    private IEnumerator FullSpin(float duration)
    {
        _isLocked = true;
        float startRotation = _carouselTransform.rotation.eulerAngles.z;
        float endRotation = -90;
        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / duration;
            float newRotation = Mathf.Lerp(startRotation, endRotation, t);
            _carouselTransform.rotation = Quaternion.Euler(0, 0, newRotation);
            yield return null;
        }

        _isLocked = false;
        SetChangedAnswer(0);
    }
    
    public void PlaceButtons(List<string> answerKeys, List<float> answerValues)
    {

        StartCoroutine(FullSpin(1.5f));
        
        ClearButtons();
        int numberOfButtons = answerKeys.Count;
        // Расчитываем угол между каждой кнопкой в указанном угловом диапазоне
        float angleStep = (maxAngle - minAngle) / numberOfButtons;

        // Создаем кнопки и располагаем их в указанном угловом диапазоне
        for (int i = 0; i < numberOfButtons; i++)
        {
            // Рассчитываем угол для текущей кнопки
            float angle = minAngle + i * angleStep;
            
            // Создаем объект для хранения данных о кнопке
            AnswerInfo answer = new AnswerInfo(answerKeys[i], answerValues[i], -angle-90);
            // Рассчитываем положение кнопки вокруг центра круга
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            // Создаем кнопку и устанавливаем ее позицию
            CustomButton button = Instantiate(buttonPrefab, _carouselTransform);
            button.transform.localPosition = new Vector3(x, y, 0f);
            var i1 = i;
            button.SetOnClickFunction(() =>
            {
                SetChangedAnswer(i1);
            });

            // Вычисляем угол для поворота кнопки к краю круга
            float rotationAngle = angle - 270f; // -90 градусов для направления к краю

            // Создаем Quaternion для поворота кнопки
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAngle);

            // Устанавливаем поворот кнопки
            button.transform.localRotation = rotation;
            button.SetText(answerKeys[i]);
            
            // Добавляем объект с информацией о кнопке в список
            _answers.Add(answer);
            
            _currentButtons.Add(button);
        }
    }

    public void SetNextAnswer()
    {
        if (_answers.Count == 0 || _isLocked)
        {
            return;
        }

        if (_changedAnswerIndex < _answers.Count-1)
        {
            _changedAnswerIndex += 1;
            SetChangedAnswer(_changedAnswerIndex);
            return;
        }
        _changedAnswerIndex = 0;
        SetChangedAnswer(_changedAnswerIndex);
    }

    public void SetPrevAnswer()
    {
        if (_answers.Count == 0 || _isLocked)
        {
            return;
        }
        

        if (_changedAnswerIndex > 0)
        {
            _changedAnswerIndex -= 1;
            SetChangedAnswer(_changedAnswerIndex);
            return;
        }

        _changedAnswerIndex = _answers.Count - 1;
        SetChangedAnswer(_changedAnswerIndex);
    }

    public AnswerInfo GetAnswer()
    {
        return _changedAnswer;
    }
}
