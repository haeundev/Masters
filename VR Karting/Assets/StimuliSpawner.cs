using System.Collections.Generic;
using DataTables;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class StimuliSpawner : MonoBehaviour
{
    [SerializeField] private GameObject stimuliPrefab;
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
        // // split half, shuffle each half, then merge
        // var originalList = stimuliSO.Values.ToList();
        //
        // // Split the list into two halves
        // var halfIndex = originalList.Count / 2;
        // var firstHalf = originalList.GetRange(0, halfIndex);
        // var secondHalf = originalList.GetRange(halfIndex, originalList.Count - halfIndex);
        //
        // // Shuffle each half
        // firstHalf.Shuffle();
        // secondHalf.Shuffle();
        //
        // // Merge the shuffled halves
        // var data = Extensions.MergeLists(firstHalf, secondHalf);
        var data = stimuliSO.Values;

        var spawnCount = sessionCount * data.Count;

        for (var i = 0; i < spawnCount; i++)
        {
            var stimuli = Instantiate(stimuliPrefab, transform);
            var parentPos = transform.position;
            stimuli.transform.position = new Vector3(
                parentPos.x + Random.Range(-randomX, randomX),
                parentPos.y,
                parentPos.z + i * (totalDistance / spawnCount));

            stimuli.GetComponentInChildren<StimuliObject>()
                .SetInfo(data[i % stimuliSO.Values.Count], i < stimuliSO.Values.Count / 2);

            _spawnedItems.Add(stimuli);
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