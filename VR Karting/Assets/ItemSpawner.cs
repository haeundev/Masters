using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private int spawnCount = 20;
    [SerializeField] private float randomX;
    [SerializeField] private float totalDistance = 1400f;

    private readonly List<GameObject> _spawnedItems = new();
    

    [Button]
    private void Spawn()
    {
        for (var i = 0; i < spawnCount; i++)
        {
            var item = Instantiate(items[Random.Range(0, items.Length)], transform);
            var parentPos = transform.position;
            item.transform.position = new Vector3(
                parentPos.x + Random.Range(-randomX, randomX),
                parentPos.y,
                parentPos.z + i * (totalDistance / spawnCount));
            _spawnedItems.Add(item);
        }
    }

    [Button]
    private void Clear()
    {
        foreach (var item in _spawnedItems)
            DestroyImmediate(item);
        _spawnedItems.Clear();
    }
}