using System;
using UnityEngine;

public class StimuliTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnTrigger?.Invoke();
    }

    public event Action OnTrigger;
}