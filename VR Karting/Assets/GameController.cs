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

public enum DriveMode { Manual, Auto }
public enum NoiseMode { None, SingleTalker, Environmental }

public class GameController : MonoBehaviour
{
    [Title("Today's speaker", "$Speaker")]
    [OnValueChanged("GetSessionInfo")]
    [SerializeField][Range(1, 40)] private int participantID;
    [SerializeField][Range(1, 8)] private int sessionID;
    
    private void GetSessionInfo()
    {
        var session = speakersSO.Values.FirstOrDefault(p => p.ParticipantID == participantID);
        Speaker = sessionID switch
        {
            1 => session.Session1,
            2 => session.Session2,
            3 => session.Session3,
            4 => session.Session4,
            5 => session.Session5,
            6 => session.Session6,
            7 => session.Session7,
            8 => session.Session8,
            _ => Speaker
        };
        Debug.Log($"Speaker: {Speaker}");
    }
    
    [HideInInspector]
    public string Speaker;
    
    [Title("Test Settings")]
    [SerializeField] public float speed = 1f;
    [SerializeField] public DriveMode driveMode = DriveMode.Auto;
    [SerializeField] public NoiseMode noiseMode = NoiseMode.None;
    
    private Transform _lookAt;
    private StimuliSpawner _stimuliSpawner;
    private List<FoodItem> _items;
    private KartController _kartController;
    
    private List<Transform> _stimuli = new(); // List of targets to move towards
    private int _currentTargetIndex = 0; // Index of the current target in the list
    
    [FoldoutGroup("Refs")] [SerializeField] private Button buttonA;
    [FoldoutGroup("Refs")] [SerializeField] private Button buttonB;
    [FoldoutGroup("Refs")] [SerializeField] private TextMeshProUGUI instructionText;
    [FoldoutGroup("Refs")] [SerializeField] private TextMeshProUGUI textA;
    [FoldoutGroup("Refs")] [SerializeField] private TextMeshProUGUI textB;
    [FoldoutGroup("Refs")] [SerializeField] public EnvironmentController environmentController;
    [FoldoutGroup("Refs")] [SerializeField] public NoiseController noiseController;
    [FoldoutGroup("Refs")] [SerializeField] private Speakers speakersSO;
    [FoldoutGroup("Refs")] [SerializeField] private Button startButton;

    [SerializeField] private bool speedToggle;
    
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
        
        startButton.onClick.AddListener(UserStart);
    }
    
    private void Start()
    {
        textA.transform.parent.localScale = Vector3.zero;
        textB.transform.parent.localScale = Vector3.zero;
        instructionText.transform.localScale = Vector3.zero;
        textA.text = "";
        textB.text = "";
        
        environmentController.SetRandomEnvironment();
        noiseController.Init(noiseMode, sessionID > 4);
    }

    private void UserStart()
    {
        StartCoroutine(CoUserStart());
    }

    private IEnumerator CoUserStart()
    {
        var text = startButton.GetComponentInChildren<TextMeshProUGUI>();
        
        text.text = "3";
        yield return YieldInstructionCache.WaitForSeconds(1f);
        
        text.text = "2";
        yield return YieldInstructionCache.WaitForSeconds(1f);
        
        text.text = "1";
        yield return YieldInstructionCache.WaitForSeconds(1f);
        
        text.text = "Start!";
        yield return YieldInstructionCache.WaitForSeconds(1f);
        
        startButton.transform.parent.gameObject.SetActive(false);
        
        _stimuliSpawner.Init();
        _stimuli = _stimuliSpawner.transform.GetComponentsInChildren<StimuliObject>().Select(p => p.transform).ToList();
        Debug.Log($"Total targets: {_stimuli.Count}");
        
        _kartController.speedInput = true;
        speedToggle = true;
        GlobalInfo.IsFirstStimuli = true;
    }
    
    [Button(SdfIconType.Bicycle, "Start / Stop")]
    public void ToggleSpeed()
    {
        speedToggle = !speedToggle;
        _kartController.speedInput = speedToggle;
    }
    
    [Button]
    public void SetSpeed()
    {
        _kartController.acceleration *= speed;
    }

    private void OnStimuliTriggered(GameObject target, Stimuli info, string answer)
    {
        _currentInfo = info;
        _currentAnswer = answer;
        
        GlobalInfo.IsStimuliAnswered = false;
        
        SetDisplayedInfo(_currentInfo);
        
        textA.transform.parent.DoScale();
        textB.transform.parent.DoScale();
        instructionText.transform.DoScale();
        
        //target.GetComponentInChildren<Collider>().enabled = false;
        //target.transform.DoScaleToZero();
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
        
        GlobalInfo.IsStimuliAnswered = true;
        
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
        if (driveMode == DriveMode.Auto)
            MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        if (_stimuli.Count == 0 || _isEnded)
            return;
        
        // check index
        if (_currentTargetIndex >= _stimuli.Count)
        {
            Debug.Log("No more targets to move towards");
            _isEnded = true;
            StartCoroutine(OnEnded());
            return;
        }
        
        var currentTarget = _stimuli[_currentTargetIndex];
        var kartPos = _kartController.transform.position;
        var targetPos = currentTarget.position;
        var targetPosition = new Vector3(targetPos.x, kartPos.y, targetPos.z); // Target position on the X and Z axes, Y axis is ignored
        
        if (driveMode == DriveMode.Auto) // auto rotate
        {
            var targetDirection = new Vector3(targetPosition.x, kartPos.y, targetPosition.z) - kartPos;
            targetDirection.Normalize();
            var newForward = Vector3.Lerp(_kartController.transform.forward, targetDirection, Time.deltaTime * 3f);
            _kartController.transform.forward = newForward;
        }
        
        // Move on to the next target
        if (Vector3.Distance(_kartController.transform.position, targetPosition) < 2f)
        {
            if (GlobalInfo.IsStimuliAnswered)
            {
                MoveOnToNextTarget();
            }
            else
            {
                Debug.Log("Waiting for response...");
                if (GlobalInfo.IsFirstStimuli)
                {
                    MoveOnToNextTarget();
                }
            }
        }
    }
    
    private void MoveOnToNextTarget()
    {
        _currentTargetIndex++;
            
        if (_currentTargetIndex >= _stimuli.Count)
        {
            _isEnded = true;
            return;
        }
            
        Debug.Log("Target reached. Next target: " + _stimuli[_currentTargetIndex].name);
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

    private void OnDestroy()
    {
        startButton.onClick.RemoveAllListeners();
    }
}