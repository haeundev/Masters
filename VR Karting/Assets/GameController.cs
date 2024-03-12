using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTables;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] public bool isAutoDrive = true;
    [SerializeField] public float speed = 1f;
    private bool _isLookAtSet;
    private Transform _lookAt;
    private StimuliSpawner _stimuliSpawner;
    private List<Item> _items;
    private KartController _kartController;
    
    private List<Transform> stimulis; // List of targets to move towards
    private int _currentTargetIndex = 0; // Index of the current target in the list
    
    [SerializeField] private TextMeshProUGUI optionA;
    [SerializeField] private TextMeshProUGUI optionB;
    

    private void Awake()
    {
        _kartController = FindObjectOfType<KartController>();
        _stimuliSpawner = FindObjectOfType<StimuliSpawner>();
        GameEvents.OnStimuliTriggered += OnStimuliTriggered;
    }

    private void Start()
    {
        stimulis = _stimuliSpawner.transform.GetComponentsInChildren<StimuliObject>().Select(p => p.transform).ToList();
        Debug.Log($"Total targets: {stimulis.Count}");
        
        optionA.text = "";
        optionB.text = "";
    }
    
    [Button]
    public void SetSpeed()
    {
        _kartController.acceleration *= speed;
    }

    private void OnStimuliTriggered(GameObject target, Stimuli info, string answer)
    {
        _isLookAtSet = false;
        
        SetDisplayedInfo(info);
        
        Destroy(target);
        
        Debug.Log($"Stimuli triggered: {info.Contrast} - {answer}");
    }

    private void Update()
    {
        MoveTowardsTarget();
    }

    private bool _isEnded;

    private void MoveTowardsTarget()
    {
        if (stimulis.Count == 0 || _isEnded)
            return;
        
        // check index
        if (_currentTargetIndex >= stimulis.Count)
        {
            Debug.Log("No more targets to move towards");
            _isEnded = true;
            StartCoroutine(OnEnded());
            return;
        }
        
        var currentTarget = stimulis[_currentTargetIndex];
        var kartPos = _kartController.transform.position;
        var targetPos = currentTarget.position;
        var targetPosition = new Vector3(targetPos.x, kartPos.y, targetPos.z); // Target position on the X and Z axes, Y axis is ignored
        
        if (isAutoDrive) // auto rotate
        {
            var targetDirection = new Vector3(targetPosition.x, kartPos.y, targetPosition.z) - kartPos;
            targetDirection.Normalize();
            var newForward = Vector3.Lerp(_kartController.transform.forward, targetDirection, Time.deltaTime * 3f);
            _kartController.transform.forward = newForward;
        }
        
        // Move on to the next target
        if (Vector3.Distance(_kartController.transform.position, targetPosition) < 2f)
        {
            _currentTargetIndex++;
            
            if (_currentTargetIndex >= stimulis.Count)
            {
                _isEnded = true;
                return;
            }
            
            Debug.Log("Target reached. Next target: " + stimulis[_currentTargetIndex].name);
        }
    }

    private void SetDisplayedInfo(Stimuli info)
    {
        optionA.text = $"{info.A}";
        optionB.text = $"{info.B}";
    }

    private IEnumerator OnEnded()
    {
        Debug.Log("Game ended");

        yield return YieldInstructionCache.WaitForSeconds(5f);
        
        UnityEditor.EditorApplication.isPlaying = false;
    }
}