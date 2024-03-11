using UnityEngine;
using UnityEngine.XR;

public class KartControllerVRInput : MonoBehaviour
{
    [Header("Speed")] public float speedThreshold = 0.7f;
    public XRNode speedXrNode = XRNode.RightHand;

    [Header("Drift")] public float driftThreshold = 0.7f;
    public XRNode driftXRNode = XRNode.LeftHand;

    [Header("Turn")] public HingeJoint wheel;
    public float turnAmount = 1f;
    public float maxValue = 0.35f;
    public float minValue = -0.35f;
    public float turnThreshold = 0.2f;

    private KartController KartController;
    private GameController _gameController;

    // Start is called before the first frame update
    private void Awake()
    {
        KartController = GetComponent<KartController>();
        _gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_gameController.isAutoDrive)
        {
            KartController.speedInput = true;
        }
        else
        {
            //Speed Input
            if (Input.GetKey(KeyCode.UpArrow) ||
                (InputDevices.GetDeviceAtXRNode(speedXrNode).TryGetFeatureValue(CommonUsages.trigger, out var v) &&
                 v > speedThreshold))
                KartController.speedInput = true;
            else
                KartController.speedInput = false;
        }

        //Drift Input
        if (Input.GetKey(KeyCode.Space) ||
            (InputDevices.GetDeviceAtXRNode(driftXRNode).TryGetFeatureValue(CommonUsages.trigger, out var g) &&
             g > driftThreshold))
            KartController.driftInput = true;
        else
            KartController.driftInput = false;

        //Turn Input
        var steeringNormal = Mathf.InverseLerp(minValue, maxValue, wheel.transform.localRotation.x);
        var steeringRange = Mathf.Lerp(-1, 1, steeringNormal);
        if (Mathf.Abs(steeringRange) < turnThreshold) steeringRange = 0;

        if (steeringRange == 0)
            KartController.turnInput = -Input.GetAxis("Horizontal") * turnAmount;
        else
            KartController.turnInput = -steeringRange * turnAmount;
    }
}