using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTables;
using DG.Tweening;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Extensions;

public class GameController : MonoBehaviour
{
    [SerializeField] public bool isAutoDrive = true;
    [SerializeField] public EnvironmentController environmentController;
    [SerializeField] public float speed = 1f;
    private bool _isLookAtSet;
    private Transform _lookAt;
    private StimuliSpawner _stimuliSpawner;
    private List<FoodItem> _items;
    private KartController _kartController;
    
    private List<Transform> stimulis; // List of targets to move towards
    private int _currentTargetIndex = 0; // Index of the current target in the list
    
    [SerializeField] private Button buttonA;
    [SerializeField] private Button buttonB;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI textA;
    [SerializeField] private TextMeshProUGUI textB;
    
    private bool _isEnded;
    private Stimuli _currentInfo;
    private string _currentAnswer;
    private bool _isWaitForResponse;

    private void Awake()
    {
        _kartController = FindObjectOfType<KartController>();
        _stimuliSpawner = FindObjectOfType<StimuliSpawner>();
        GameEvents.OnStimuliTriggered += OnStimuliTriggered;
        
        buttonA.onClick.AddListener(() => OnResponse("A"));
        buttonB.onClick.AddListener(() => OnResponse("B"));
    }

    private void Start()
    {
        stimulis = _stimuliSpawner.transform.GetComponentsInChildren<StimuliObject>().Select(p => p.transform).ToList();
        Debug.Log($"Total targets: {stimulis.Count}");

        textA.transform.parent.localScale = Vector3.zero;
        textB.transform.parent.localScale = Vector3.zero;
        instructionText.transform.localScale = Vector3.zero;
        textA.text = "";
        textB.text = "";
        
        environmentController.SetRandomEnvironment();
    }
    
    [Button]
    public void SetSpeed()
    {
        _kartController.acceleration *= speed;
    }

    private void OnStimuliTriggered(GameObject target, Stimuli info, string answer)
    {
        _isLookAtSet = false;
        
        _currentInfo = info;
        _currentAnswer = answer;
        
        SetDisplayedInfo(_currentInfo);
        
        textA.transform.parent.DoScale();
        textB.transform.parent.DoScale();
        instructionText.transform.DoScale();
        
        Destroy(target);
        
        Debug.Log($"Stimuli triggered: {_currentInfo.Contrast} - {_currentAnswer}");
        
        BeginWaitForResponse();
    }

    private void BeginWaitForResponse()
    {
        _isWaitForResponse = true;
    }

    private void OnResponse(string response)
    {
        if (!_isWaitForResponse)
            return;
        
        Debug.Log($"Response: {response}");
        
        if (textA.text == _currentAnswer && response == "A" ||
            textB.text == _currentAnswer && response == "B")
        {
            Debug.Log("Correct!");
            
            GlobalInfo.Score++;
            
            FloatEffect.Play(true);
        }
        else
        {
            Debug.Log("Incorrect!");
            
            FloatEffect.Play(false);
        }
        
        _isWaitForResponse = false;

        buttonA.transform.DOScale(Vector3.zero, 0.3f);
        buttonB.transform.DOScale(Vector3.zero, 0.3f);
        instructionText.transform.DOScale(Vector3.zero, 0.3f);
    }

    private void Update()
    {
        if (isAutoDrive)
            MoveTowardsTarget();
    }

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
        textA.text = $"{info.A}";
        textB.text = $"{info.B}";
    }

    private IEnumerator OnEnded()
    {
        Debug.Log("Game ended");

        yield return YieldInstructionCache.WaitForSeconds(5f);
        
        Application.Quit();
    }
}