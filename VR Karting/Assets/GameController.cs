using System;
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
    [Title("Today's Session Info", "$CombinedSessionInfo")]
    [OnValueChanged("GetSessionInfo")]
    [SerializeField][Range(1, 36)] private int participantID;
    [OnValueChanged("GetSessionInfo")]
    [SerializeField][Range(1, 8)] private int sessionID;
    
    public string CombinedSessionInfo => $"P{participantID}  |  Speaker: {SpeakerID} |  IsAutoDrive: {sessionInfo.IsAutoDrive} |  Noise: {noiseMode.ToString()}";

    private NoiseMode GetTodaysNoise()
    {
        noiseMode = sessionInfo.NoiseMode switch
        {
            "SingleTalker" => NoiseMode.SingleTalker,
            "Environmental" => NoiseMode.Environmental,
            _ => NoiseMode.None
        };

        return noiseMode;
    }

    private void GetSessionInfo()
    {
        sessionInfo = sessionInfosAsset.Values.FirstOrDefault(p => p.ParticipantID == participantID);
        
        driveMode = sessionInfo.IsAutoDrive ? DriveMode.Auto : DriveMode.Manual;
        
        SpeakerID = sessionID switch
        {
            1 => sessionInfo.Session1,
            2 => sessionInfo.Session2,
            3 => sessionInfo.Session3,
            4 => sessionInfo.Session4,
            5 => sessionInfo.Session5,
            6 => sessionInfo.Session6,
            7 => sessionInfo.Session7,
            8 => sessionInfo.Session8,
            _ => SpeakerID
        };
        
        IsWeek2 = sessionID switch
        {
            5 => true,
            6 => true,
            7 => true,
            8 => true,
            _ => false
        };
        
        GetTodaysNoise();
    }
    
    [HideInInspector] public SessionInfo sessionInfo;
    [HideInInspector] public string SpeakerID;
    [HideInInspector] public bool IsWeek2;
    
    [Title("Settings")]
    [SerializeField] public float speed = 1f;
    [LabelText("Force Drive (For Test Purpose Only)")] [SerializeField] public DriveMode driveMode = DriveMode.Auto;
    [LabelText("Force Noise (For Test Purpose Only)")] [SerializeField] public NoiseMode noiseMode = NoiseMode.None;
    
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
    [FoldoutGroup("Refs")] [SerializeField] private SessionInfos sessionInfosAsset;
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
    
    private TextFileHandler _fileHandler;
    
    private void Start()
    {
        textA.transform.parent.localScale = Vector3.zero;
        textB.transform.parent.localScale = Vector3.zero;
        instructionText.transform.localScale = Vector3.zero;
        textA.text = "";
        textB.text = "";
        
        noiseController.Init(noiseMode, IsWeek2, SpeakerID);
        
        var date = DateTime.Today.ToString("dd-MM-yyyy");
        _fileHandler = new TextFileHandler(Application.persistentDataPath, $"Training p{participantID} {SpeakerID} {date}.txt");
    }

    private void UserStart()
    {
        StartCoroutine(CoUserStart());
    }
    
    private bool _isMovingForward;

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
        
        _stimuliSpawner.Init(sessionID, SpeakerID, noiseMode);
        _stimuli = _stimuliSpawner.transform.GetComponentsInChildren<StimuliObject>().Select(p => p.transform).ToList();
        Debug.Log($"Total targets: {_stimuli.Count}");
        
        _isMovingForward = true;
        speedToggle = true;
        GlobalInfo.IsFirstStimuli = true;
    }
    
    [Button(SdfIconType.Bicycle, "Start / Stop")]
    public void ToggleSpeed()
    {
        speedToggle = !speedToggle;
        _isMovingForward = speedToggle;
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
        
        var isCorrect = textA.text == _currentAnswer && response == "A" || textB.text == _currentAnswer && response == "B";
        
        if (isCorrect)
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
        
        _fileHandler.WriteLine($"{_currentAnswer}, {isCorrect}");
        
        _isWaitForResponse = false;

        buttonA.transform.DOScale(Vector3.zero, 0.3f);
        buttonB.transform.DOScale(Vector3.zero, 0.3f);
        instructionText.transform.DOScale(Vector3.zero, 0.3f);
    }

    [SerializeField] private Transform kartTransform;
    
    private Vector3 _forward;
    
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
            UserStart();
        else if (Input.GetKeyDown(KeyCode.Q))
            OnResponse("A");
        else if (Input.GetKeyDown(KeyCode.E))
            OnResponse("B");
#endif
        
        if (_isMovingForward == false)
            return;
        
        if (driveMode == DriveMode.Auto)
        {
            MoveTowardsTarget();
        }
        else // manual drive
        {
            _forward = kartTransform.transform.forward;
            kartTransform.position += _forward * Time.deltaTime * speed;
        }
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
        var kartPos = kartTransform.position;
        var targetPos = currentTarget.position;
        var targetPosition = new Vector3(targetPos.x, kartPos.y, targetPos.z); // Target position on the X and Z axes, Y axis is ignored
        
        if (driveMode == DriveMode.Auto) // auto rotate
        {
            var targetDirection = new Vector3(targetPosition.x, kartPos.y, targetPosition.z) - kartPos;
            targetDirection.Normalize();
            var lookRotation = Quaternion.LookRotation(targetDirection);
            kartTransform.rotation = Quaternion.Slerp(kartTransform.rotation, lookRotation, Time.deltaTime * 2f);
            kartTransform.position += kartTransform.forward * Time.deltaTime * speed;
        }
        
        // Move on to the next target
        if (Vector3.Distance(kartTransform.position, targetPosition) < 2f)
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
            
        Debug.Log("Target reached. Next target: " + _stimuli[_currentTargetIndex].name + "\nCurrent target index: " + _currentTargetIndex);
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
    
    private void OnApplicationQuit()
    {
        _fileHandler.CloseFile();
    }
}