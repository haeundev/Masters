using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTables;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EvaluationTrial
{
    public EvaluationStimuli info;
    public string answer;
}

public class EvaluationController : MonoBehaviour
{
    [SerializeField] private int pair = 2;
    [SerializeField] private EvaluationStimulis evaluationStimuliSO;
    [SerializeField] private SessionInfos sessionInfosAsset;

    [SerializeField] private GameObject icon;
    
    [SerializeField] private Button optionA;
    [SerializeField] private Button optionB;

    private float _eachDistance;
    
    [SerializeField] public List<EvaluationTrial> trials = new();
    
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

    [Button]
    private void MakeTrialQueue()
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
        var data = evaluationStimuliSO.Values;
        var index = 0;
        
        for (var i = 0; i < pair; i++)
        {
            for (var j = 0; j < data.Count; j++)
            {
                var isOddPair = i % 2 == 0;
                var info = data[j];
                
                var evaluationTrial = new EvaluationTrial
                {
                    info = info,
                    answer = isOddPair ? info.A : info.B
                };
                
                trials.Add(evaluationTrial);
                
                index++;
            }
        }
    }
    
    private void ClearTrials()
    {
        trials.Clear();
    }
    
    [Button]
    private void StartEvaluation()
    {
        ClearTrials();
        MakeTrialQueue();
        
        StartCoroutine(RunTrials());
    }

    private string _answer;
    
    private IEnumerator RunTrials()
    {
        foreach (var trial in trials)
        {
            // hide options
            optionA.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            optionB.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            optionA.gameObject.SetActive(false);
            optionB.gameObject.SetActive(false);
            
            icon.SetActive(true);

            yield return new WaitForSeconds(2f);
            
            var word = new GameObject("Word").AddComponent<AudioSource>();
            word.clip = Resources.Load<AudioClip>($"Audio/Words/Evaluation/{SpeakerID}/{trial.answer}");
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
            
            Debug.Log($"Answer: {_answer}, Correct?: {isCorrect}");
        }
    }
}
