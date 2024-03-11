using System.Collections.Generic;
using System.Linq;
using DataTables;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] public bool isAutoDrive = true;
    private bool _isLookAtSet;
    private Transform _lookAt;
    private ItemSpawner _itemSpawner;
    private int _currentTargetIndex;
    private List<Item> _items;
    private KartController _kartController;
    
    public List<Transform> targets; // List of targets to move towards
    public float speed = 1f; // Movement speed
    private int currentTargetIndex = 0; // Index of the current target in the list
    

    private void Awake()
    {
        _kartController = FindObjectOfType<KartController>();
        _itemSpawner = FindObjectOfType<ItemSpawner>();
        GameEvents.OnStimuliTriggered += OnStimuliTriggered;
    }

    private void Start()
    {
        targets = _itemSpawner.transform.GetComponentsInChildren<Item>().Select(p => p.transform).ToList();
    }

    private void OnStimuliTriggered(GameObject target, Stimuli info, string answer)
    {
        _isLookAtSet = false;
        
        Destroy(target);
        
        Debug.Log($"Stimuli triggered: {info.Contrast} - {answer}");
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        if (targets.Count == 0)
            return; // No targets to move towards

        Transform currentTarget = targets[currentTargetIndex];
        Vector3 targetPosition = new Vector3(currentTarget.position.x, _kartController.transform.position.y, currentTarget.position.z); // Target position on the X and Z axes, Y axis is ignored
        
        _kartController.transform.position = Vector3.MoveTowards(_kartController.transform.position, targetPosition, speed * Time.deltaTime); // Move towards the target
        
        Debug.Log($"Moving towards target: {currentTarget.name}");
        
// Calculate the target direction
        Vector3 targetDirection = new Vector3(targetPosition.x, _kartController.transform.position.y, targetPosition.z) - _kartController.transform.position;
// Normalize the target direction
        targetDirection.Normalize();

// Determine the new forward direction by interpolating between the current forward direction and the target direction
        Vector3 newForward = Vector3.Lerp(_kartController.transform.forward, targetDirection, Time.deltaTime * 3f);

// Apply the new forward direction to the kart
        _kartController.transform.forward = newForward;        
        
        // Check if the object has reached the target
        if (Vector3.Distance(_kartController.transform.position, targetPosition) < 2f) // Adjust the 0.1f value to specify when the target is considered "reached"
        {
            currentTargetIndex++; // Move on to the next target
            Debug.Log("Target reached");
            if (currentTargetIndex >= targets.Count) // If there are no more targets, you can loop or stop
            {
                currentTargetIndex = 0; // Loop to the first target again or implement another behavior
            }
        }
    }
}