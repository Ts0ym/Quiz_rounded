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

    [Header("Input prefs")]
    [SerializeField] private InputManager _inputManager;
    private bool _isDragging = false;
    
    [Header("Rotating data")]
    [SerializeField] private float _rotationSpeed = 1;
    private Vector2 _startTouchPosition;
    private bool _isLocked = false;

    [Header("Current answers data")]
    private List<AnswerInfo> _answers = new List<AnswerInfo>();
    private AnswerInfo _changedAnswer;

    [Header("Sound prefs")]
    [SerializeField] private AudioController _audioController;
    [SerializeField] private float minRotationSpeed = 1.0f;
    [SerializeField] private int positionsCount = 8; // Количество заранее просчитанных позиций для воспроизведения звука
    [SerializeField] private float maxRotationSpeed = 100f; // Максимальная скорость вращения для максимальной громкости звука
    [SerializeField] private float minVolume = 0.1f; // Минимальная громкость
    [SerializeField] private float maxVolume = 1.0f; // Максимальная громкость
    private int _currentPosition = 0;
    private float _lastRotation = 0f;

    private void Update()
    {
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
        _isDragging = false;
        SetChangedAnswer(GetNearestAnswer(_carouselTransform.rotation.eulerAngles.z));
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

    private AnswerInfo GetNearestAnswer(float currentAngle)
    {
        AnswerInfo nearestAnswer = _answers[0];
        float minDiff = Mathf.Abs(Mathf.Abs(Mathf.DeltaAngle(nearestAnswer.AnswerAngle, currentAngle)));
        
        for (int i = 1; i < _answers.Count; i++)
        {

            float currentDiff = Mathf.Abs(Mathf.DeltaAngle(_answers[i].AnswerAngle, currentAngle));
            if (currentDiff < minDiff)
            {
                nearestAnswer = _answers[i];
                minDiff = currentDiff;
            }
        }
        
        return nearestAnswer;
    }

    private void SetChangedAnswer(AnswerInfo changedAnswer)
    {
        _changedAnswer = changedAnswer;
        StartRotateTo(Quaternion.Euler(0, 0, changedAnswer.AnswerAngle));
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
    
    public void PlaceButtons(List<string> answerKeys, List<float> answerValues)
    {
        if (answerKeys.Count != answerValues.Count)
        {
            return;
        }
        
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
            button.SetOnClickFunction(() => SetChangedAnswer(answer));

            // Вычисляем угол для поворота кнопки к краю круга
            float rotationAngle = angle - 270f; // -90 градусов для направления к краю

            // Создаем Quaternion для поворота кнопки
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotationAngle);

            // Устанавливаем поворот кнопки
            button.transform.localRotation = rotation;
            button.SetText(angle + $" {i}");
            
            // Добавляем объект с информацией о кнопке в список
            _answers.Add(answer);
        }
        SetChangedAnswer(_answers[0]);
    }
}
