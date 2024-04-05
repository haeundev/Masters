using System;
using UnityEngine;

public class FoodItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnTrigger?.Invoke();
    }

    public event Action OnTrigger;
}