using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RotatableAroundImage : MonoBehaviour
{
    [SerializeField] private Transform _orbitCenter; // Ссылка на объект "OrbitCenter"
    [SerializeField] private float _orbitSpeed = 10.0f; // Скорость вращения

    void Update()
    {
        transform.RotateAround(_orbitCenter.position, Vector3.forward, _orbitSpeed * Time.deltaTime);
    }
}
