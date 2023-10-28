using UnityEngine;

public class RotatableImage : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1f;

    void Update()
    {
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
    }
}
