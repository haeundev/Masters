using System;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    private Quaternion _initialRotation;
    
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis axis = Axis.Y;
    public float speed = 1f;

    private void Awake()
    {
        _initialRotation = transform.rotation;
    }

    private void OnEnable()
    {
        // reset rotation
        transform.rotation = _initialRotation;
    }

    private void Update()
    {
        switch (axis)
        {
            case Axis.X:
                transform.Rotate(speed * Time.deltaTime, 0, 0);
                break;
            case Axis.Y:
                transform.Rotate(0, speed * Time.deltaTime, 0);
                break;
            case Axis.Z:
                transform.Rotate(0, 0, speed * Time.deltaTime);
                break;
        }
    }
}