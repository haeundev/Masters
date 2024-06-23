using System.Collections.Generic;
using System.Linq;
using DataTables;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class StimuliSpawner : MonoBehaviour
{
    [SerializeField] private GameObject stimuliPrefab;
    [SerializeField] private float randomX;
    [SerializeField] private float totalDistance = 1400f;
    [SerializeField] private int pair = 2;

    [SerializeField] private Stimulis stimuliSO;

    private readonly List<GameObject> _spawnedItems = new();
    private float _eachDistance;
    private int _sessionID;
    private string _speakerID;
    private NoiseMode _noiseMode;
    
    public static StimuliSpawner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        GameEvents.OnPushAllStimuliForward += PushAllStimuliForward;
    }

    public void Init(int sessionID, string speakerID, NoiseMode noiseMode)
    {
        _sessionID = sessionID;
        _speakerID = speakerID;
        _noiseMode = noiseMode;
        
        Clear();
        Spawn();
    }

    private void PushAllStimuliForward()
    {
        foreach (var item in _spawnedItems.Where(p => p != default))
        {
            var pos = item.transform.position;
            item.transform.position = new Vector3(pos.x, pos.y, pos.z + _eachDistance);
        }
    }

    [Button]
    public void Spawn()
    {
        var data = stimuliSO.Values.ToShuffleList();
        var doubledData = new List<Stimuli>(data.Count * 2);

        // Double the list
        doubledData.AddRange(data);
        doubledData.AddRange(data);

        // Shuffle the doubled list
        doubledData = doubledData.OrderBy(x => Random.value).ToList();

        var index = 0;
        var initialDistance = totalDistance / 2; // 맵에 뒤에 더 추가해놓긴 했는데 그건 스페어로 사용.
        _eachDistance = initialDistance / doubledData.Count;

        // Used to keep track of which option was used for each pair
        var usedAnswers = new HashSet<string>();

        foreach (var info in doubledData)
        {
            var stimuli = Instantiate(stimuliPrefab, transform);
            var parentPos = transform.position;
            stimuli.transform.position = new Vector3(
                parentPos.x + Random.Range(-randomX, randomX),
                parentPos.y,
                parentPos.z + index * _eachDistance);

            // get random answer, between A and B
            var rnd = Random.Range(0, 2);
            var randomAnswer = rnd == 0 ? info.A : info.B;
            var isUsedAnswer = usedAnswers.Add(randomAnswer); // Add returns true if the item was not present in the set

            stimuli.GetComponentInChildren<StimuliObject>().SetInfo(info, _sessionID, _speakerID, _noiseMode, 
                isUsedAnswer ? randomAnswer : rnd == 0 ? info.B : info.A);

            _spawnedItems.Add(stimuli);

            index++;
        }

    }

    [Button]
    private void Clear()
    {
        foreach (var item in _spawnedItems)
            DestroyImmediate(item);
        _spawnedItems.Clear();
    }

    private void OnDestroy()
    {
        GameEvents.OnPushAllStimuliForward -= PushAllStimuliForward;
    }
}