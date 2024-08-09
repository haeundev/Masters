using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTables;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EvaluationTrial
{
    public EvaluationStimuli info;
    public string speakerID;
    public string answer;
}

[Serializable]
public class MiniTestTrial
{
    public Stimuli info;
    public string answer;
}

public class EvaluationController : MonoBehaviour
{
    [SerializeField] private int pair = 2;
    [SerializeField] private EvaluationStimulis evaluationStimuliSO;
    [SerializeField] private Stimulis trainingStimuliSO;
    [SerializeField] private SessionInfos sessionInfosAsset;
    [SerializeField] private EvaluationInfos evaluationInfosSO;

    [SerializeField] private GameObject icon;
    [SerializeField] private TextMeshProUGUI remainingText;
    
    [SerializeField] private Button optionA;
    [SerializeField] private Button optionB;
    [SerializeField] private bool isMiniTest;
    [SerializeField] private bool isPreTest;
    [SerializeField] private bool isMidTest;
    [SerializeField] private bool isPostTest;

    private float _eachDistance;
    
    [SerializeField] public List<EvaluationTrial> evaluationTrials = new();
    [SerializeField] public List<MiniTestTrial> miniTestTrials = new();
    
    [Title("Today's Session Info", "$CombinedSessionInfo")]
    [OnValueChanged("GetSessionInfo")]
    [SerializeField][Range(1, 40)] private int participantID;
    [OnValueChanged("GetSessionInfo")]
    [SerializeField][Range(1, 8)] private int sessionID;
    
    [HideInInspector] public string SpeakerID;
    private SessionInfo sessionInfo;
    
    public string CombinedSessionInfo => $"Speaker: {SpeakerID}";
    
    private void GetSessionInfo()
    {
        sessionInfo = sessionInfosAsset.Values.FirstOrDefault(p => p.ParticipantID == participantID);
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
    }

    private void Awake()
    {
        optionA.onClick.AddListener(() => _answer = optionA.GetComponentInChildren<TextMeshProUGUI>().text);
        optionB.onClick.AddListener(() => _answer = optionB.GetComponentInChildren<TextMeshProUGUI>().text);
        
        optionA.gameObject.SetActive(false);
        optionB.gameObject.SetActive(false);
        
        icon.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            optionA.onClick.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            optionB.onClick.Invoke();
        }
    }

    private void Start()
    {
        StartEvaluation();
    }

    private TextFileHandler _fileHandler;
    
    [Button]
    private void MakeTrialQueue()
    {
        miniTestTrials.Clear();
        evaluationTrials.Clear();
        
        if (isMiniTest)
        {
            var data = trainingStimuliSO.Values.Where(p => p.ID % 7 == (participantID + sessionID - 1) % 7).ToList();
            var index = 0;
            for (var i = 0; i < pair; i++)
            {
                for (var j = 0; j < data.Count; j++)
                {
                    var isOddPair = i % 2 == 0;
                    var info = data[j];
                
                    var miniTestTrial = new MiniTestTrial()
                    {
                        info = info,
                        answer = isOddPair ? info.A : info.B
                    };
                
                    miniTestTrials.Add(miniTestTrial);
                
                    index++;
                }
            }
            
            miniTestTrials.Shuffle();
        }
        else
        {
            var data = evaluationStimuliSO.Values;
            var index = 0;
            var novelSpeakerIDs = new List<string>() { "M3", "F4"};
            
            foreach (var speaker in novelSpeakerIDs)
            {
                for (var i = 0; i < pair; i++)
                {
                    for (var j = 0; j < data.Count; j++)
                    {
                        var isOddPair = i % 2 == 0;
                        var info = data[j];
                
                        var evaluationTrial = new EvaluationTrial
                        {
                            info = info,
                            speakerID = speaker,
                            answer = isOddPair ? info.A : info.B
                        };
                
                        evaluationTrials.Add(evaluationTrial);

                        index++;
                    }
                }
            }
            
            evaluationTrials.Shuffle();
        }
    }
    
    private void ClearTrials()
    {
        evaluationTrials.Clear();
        miniTestTrials.Clear();
    }
    
    [Button]
    private void StartEvaluation()
    {
        ClearTrials();
        MakeTrialQueue();
        
        var date = DateTime.Today.ToString("dd-MM-yyyy");
        
        remainingText.gameObject.SetActive(!isMiniTest);
        
        if (isMiniTest)
        {
            _fileHandler = new TextFileHandler(Application.persistentDataPath, $"MiniTest p{participantID} {SpeakerID} {date}.txt");
            StartCoroutine(RunMiniTestTrials());
        }
        else
        {
            _fileHandler = new TextFileHandler(Application.persistentDataPath, $"Evaluation p{participantID} {SpeakerID} {date}.txt");
            StartCoroutine(RunEvaluationTrials());
        }
    }

    private string _answer;
    
    private IEnumerator RunMiniTestTrials()
    {
        foreach (var trial in miniTestTrials)
        {
            // hide options
            optionA.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            optionB.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            optionA.gameObject.SetActive(false);
            optionB.gameObject.SetActive(false);
            
            icon.SetActive(true);

            yield return new WaitForSeconds(.6f);
            
            var word = new GameObject("Word").AddComponent<AudioSource>();
            word.clip = Resources.Load<AudioClip>($"Audio/Words/Target/{SpeakerID}/{trial.answer}");
            word.Play();
            word.spatialize = false;
            
            _answer = string.Empty;
            
            yield return new WaitForSeconds(word.clip.length);
            
            icon.SetActive(false);
            Destroy(word.gameObject);
            
            // show options
            optionA.GetComponentInChildren<TextMeshProUGUI>().text = trial.info.A;
            optionB.GetComponentInChildren<TextMeshProUGUI>().text = trial.info.B;
            optionA.gameObject.SetActive(true);
            optionB.gameObject.SetActive(true);
            
            // shuffle options position
            var rnd = new System.Random();
            var isSwapped = rnd.Next(0, 2) == 0;
            if (isSwapped)
                (optionA.transform.position, optionB.transform.position) = (optionB.transform.position, optionA.transform.position);
            
            yield return new WaitUntil(() => _answer != string.Empty);
            
            var isCorrect = _answer == trial.answer;
            _fileHandler.WriteLine($"{trial.answer}, {isCorrect}");

            //Debug.Log($"Answer: {_answer}, Correct?: {isCorrect}");
        }

        yield return new WaitForSeconds(1f);
        OnFinished();
    }

    private IEnumerator RunEvaluationTrials()
    {
        var participantEvalData = evaluationInfosSO.Values.FirstOrDefault(p => p.ParticipantID == participantID);
        if (participantEvalData == null)
        {
            Debug.LogError($"Failed to find participant data for ID: {participantID}");
            yield break;
        }

        var noiseConditionOrder = new List<string>();
        if (isPreTest)
        {
            participantEvalData.PreBlocks.ForEach(p => noiseConditionOrder.Add(p));
        }
        else if (isMidTest)
        {
            participantEvalData.MidBlocks.ForEach(p => noiseConditionOrder.Add(p));
        }
        else if (isPostTest)
        {
            participantEvalData.PostBlocks.ForEach(p => noiseConditionOrder.Add(p));
        }
        else
        {
            Debug.LogError("Failed to find block data");
            yield break;
        }

        var noiseConditionPrefixes = noiseConditionOrder.Select(noiseCondition => $"Audio/Words/Evaluation/{noiseCondition}").ToList();
        
        var filePrefixes = noiseConditionOrder.Select(noiseCondition => noiseCondition switch
        {
            "PinkNoise" => "PinkNoise_SNR_-4_dB_",
            "SingleTalker" => "SingleTalker_SNR_-4_dB_",
            _ => string.Empty
        }).ToList();
        
        var totalTrials = evaluationTrials.Count * noiseConditionOrder.Count * 2;
        var remainingTrials = totalTrials;
        
        for (var j = 0; j < 2; j++)
        {
            for (var i = 0; i < noiseConditionPrefixes.Count; i++)
            {
                evaluationTrials.Shuffle();

                var pathPrefix = noiseConditionPrefixes[i];
                foreach (var trial in evaluationTrials)
                {
                    // hide options
                    optionA.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                    optionB.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                    optionA.gameObject.SetActive(false);
                    optionB.gameObject.SetActive(false);

                    // shuffle options position
                    var rnd = new System.Random();
                    var isSwapped = rnd.Next(0, 2) == 0;
                    if (isSwapped)
                        (optionA.transform.position, optionB.transform.position) =
                            (optionB.transform.position, optionA.transform.position);

                    icon.SetActive(true);

                    yield return new WaitForSeconds(.6f);

                    var word = new GameObject("Word").AddComponent<AudioSource>();
                    var path = $"{pathPrefix}/{trial.speakerID}/{filePrefixes[i]}{trial.answer}";
                    word.clip = Resources.Load<AudioClip>(path);

                    if (word.clip == null)
                    {
                        Debug.LogError($"Failed to load at: {path}");
                    }

                    word.Play();
                    word.spatialize = false;

                    _answer = string.Empty;

                    yield return new WaitForSeconds(word.clip.length);

                    icon.SetActive(false);
                    Destroy(word.gameObject);

                    // show options
                    optionA.GetComponentInChildren<TextMeshProUGUI>().text = trial.info.A;
                    optionB.GetComponentInChildren<TextMeshProUGUI>().text = trial.info.B;
                    optionA.gameObject.SetActive(true);
                    optionB.gameObject.SetActive(true);

                    yield return new WaitUntil(() => _answer != string.Empty);

                    var isCorrect = _answer == trial.answer;
                    _fileHandler.WriteLine(
                        $"{trial.answer}, {isCorrect}, {pathPrefix.Replace("Audio/Words/Evaluation/", "")}");
                    
                    remainingTrials--;
                    
                    remainingText.text = $"{totalTrials - remainingTrials} / {totalTrials}";

                    // Debug.Log($"Answer: {_answer}, Correct?: {isCorrect}");
                }
            }
        }

        yield return new WaitForSeconds(1f);
        OnFinished();
    }

    private void OnFinished()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnApplicationQuit()
    {
        _fileHandler.CloseFile();
    }
}
