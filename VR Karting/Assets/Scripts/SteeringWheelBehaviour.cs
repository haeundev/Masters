using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class SteeringWheelBehaviour : MonoBehaviour
{
    [SerializeField] private Transform rotateTarget;
    [SerializeField] private float rotateSpeed = 10f;
    private XRKnob _wheel;
    private float _wheelValue; // between -1 ~ 1
    private bool _isWheelActivated;
    
    private void Awake()
    {
        _wheel = gameObject.GetComponent<XRKnob>();
        var playerObj = GameObject.FindWithTag("Player");
        rotateTarget = playerObj.transform;
        
        _wheel.selectEntered.AddListener(OnWheelActivated);
        _wheel.selectExited.AddListener(OnWheelDeactivated);
        
        _gameController = FindObjectOfType<GameController>();
    }

    private void OnWheelActivated(SelectEnterEventArgs _)
    {
        _isWheelActivated = true;
    }

    private void OnWheelDeactivated(SelectExitEventArgs _)
    {
        _isWheelActivated = false;
        _wheel.value = 0.5f;
    }

    private GameController _gameController;
    
    private void Update()
    {
        if (_gameController.driveMode == DriveMode.Auto)
            return;
        
        _wheelValue = _wheel.value * 2 - 1;

        if (_isWheelActivated == false)
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.A))
            {
                rotateTarget.Rotate(Vector3.up * (rotateSpeed * -1 * Time.deltaTime));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rotateTarget.Rotate(Vector3.up * (rotateSpeed * 1 * Time.deltaTime));
            }
#endif
            return;
        }
        
        rotateTarget.Rotate(Vector3.up * (rotateSpeed * _wheelValue * Time.deltaTime));
    }
}