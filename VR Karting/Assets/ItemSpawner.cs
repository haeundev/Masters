using System.Collections.Generic;
using System.Linq;
using DataTables;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private float randomX;
    [SerializeField] private float totalDistance = 1400f;
    [SerializeField] private int sessionCount = 2;

    [SerializeField] private Stimulis stimuliSO;

    private readonly List<GameObject> _spawnedItems = new();
    
    private void Awake()
    {
        Clear();
        Spawn();
    }

    [Button]
    private void Spawn()
    {
        // split half, shuffle each half, then merge
        var originalList = stimuliSO.Values.ToList();

        // Split the list into two halves
        var halfIndex = originalList.Count / 2;
        var firstHalf = originalList.GetRange(0, halfIndex);
        var secondHalf = originalList.GetRange(halfIndex, originalList.Count - halfIndex);

        // Shuffle each half
        firstHalf.Shuffle();
        secondHalf.Shuffle();

        // Merge the shuffled halves
        var data = Extensions.MergeLists(firstHalf, secondHalf);
        
        var spawnCount = sessionCount * data.Count;
        
        for (var i = 0; i < spawnCount; i++)
        {
            var item = Instantiate(items[Random.Range(0, items.Length)], transform);
            var parentPos = transform.position;
            item.transform.position = new Vector3(
                parentPos.x + Random.Range(-randomX, randomX),
                parentPos.y,
                parentPos.z + i * (totalDistance / spawnCount));
            
            item.GetComponentInChildren<Item>().SetInfo(data[i % stimuliSO.Values.Count], i < stimuliSO.Values.Count / 2);
            
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