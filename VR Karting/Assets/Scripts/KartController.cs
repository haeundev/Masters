using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class KartController : MonoBehaviour
{
    private PostProcessVolume _postVolume;
    private PostProcessProfile _postProfile;

    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    public List<ParticleSystem> primaryParticles = new();
    public List<ParticleSystem> secondaryParticles = new();

    private float _speed, _currentSpeed;
    private float _rotate, _currentRotate;
    private int _driftDirection;
    private float _driftPower;
    private int _driftMode;
    private bool _first, _second, _third;
    private Color _c;

    [Header("INPUT")] public float turnInput;
    public bool speedInput;
    public bool driftInput;

    [Header("Booleans")] public bool drifting;

    [Header("Parameters")] public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;
    public float steerAnimationSpeed = 0.1f;
    public float steerAnimationAmount = 10;
    public float driftSteer = 2;
    public float driftKartAnimationSpeed = 0.1f;
    public float driftKartRotationAdd = 15;
    public float driftRecoverRotationDuration = 1;
    [Header("Model Parts")] public Transform[] frontWheels;
    public Transform[] backWheels;
    public Transform steeringWheel;

    [Header("Particles")] public Transform wheelParticles;
    public Transform flashParticles;
    public Color[] turboColors;

    private void Start()
    {
        for (var i = 0; i < wheelParticles.GetChild(0).childCount; i++)
            primaryParticles.Add(wheelParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());

        for (var i = 0; i < wheelParticles.GetChild(1).childCount; i++)
            primaryParticles.Add(wheelParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());

        foreach (var p in flashParticles.GetComponentsInChildren<ParticleSystem>()) secondaryParticles.Add(p);
    }

    private void Update()
    {
        //Follow Collider
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

        //Accelerate
        if (speedInput)
            _speed = acceleration;

        //Steer
        if (turnInput != 0)
        {
            var dir = turnInput > 0 ? 1 : -1;
            var amount = Mathf.Abs(turnInput);
            Steer(dir, amount);
        }

        //Drift
        if (driftInput && !drifting && turnInput != 0)
        {
            drifting = true;
            _driftDirection = turnInput > 0 ? 1 : -1;

            foreach (var p in primaryParticles)
            {
                p.startColor = Color.clear;
                p.Play();
            }

            //kartModel.parent.DOComplete();
            //kartModel.parent.DOPunchPosition(transform.up * .2f, .3f, 5, 1);
        }

        if (drifting)
        {
            var control = _driftDirection == 1
                ? turnInput.Remap(-1, 1, 0, driftSteer)
                : turnInput.Remap(-1, 1, driftSteer, 0);
            var powerControl = _driftDirection == 1 ? turnInput.Remap(-1, 1, .2f, 1) : turnInput.Remap(-1, 1, 1, .2f);
            Steer(_driftDirection, control);
            _driftPower += powerControl;

            ColorDrift();
        }

        if (driftInput && drifting) Boost();

        //currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        //currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;
        _currentSpeed = _speed;
        _speed = 0;
        _currentRotate = _rotate;
        _rotate = 0;
        //Animations    

        //a) Kart
        if (!drifting)
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles,
                new Vector3(0, 90 + turnInput * steerAnimationAmount, kartModel.localEulerAngles.z),
                steerAnimationSpeed);
        }
        else
        {
            var control = _driftDirection == 1
                ? turnInput.Remap(-1, 1, .5f, driftSteer)
                : turnInput.Remap(-1, 1, driftSteer, .5f);
            kartModel.parent.localRotation = Quaternion.Euler(0,
                Mathf.LerpAngle(kartModel.parent.localEulerAngles.y, control * driftKartRotationAdd * _driftDirection,
                    driftKartAnimationSpeed), 0);
        }

        //b) Wheels
        foreach (var item in frontWheels)
        {
            item.localEulerAngles = new Vector3(0, turnInput * steering, item.localEulerAngles.z);
            item.localEulerAngles += new Vector3(sphere.velocity.magnitude / 2, 0, 0);
        }

        foreach (var item in backWheels) item.localEulerAngles += new Vector3(sphere.velocity.magnitude / 2, 0, 0);

        //c) Steering Wheel
        if (steeringWheel)
            steeringWheel.localEulerAngles = new Vector3(-25, 90, turnInput * 45);
    }

    private void FixedUpdate()
    {
        //Forward Acceleration
        if (!drifting)
            sphere.AddForce(-kartModel.transform.right * _currentSpeed, ForceMode.Acceleration);
        else
            sphere.AddForce(transform.forward * _currentSpeed, ForceMode.Acceleration);

        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
            new Vector3(0, transform.eulerAngles.y + _currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + transform.up * .1f, Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + transform.up * .1f, Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void Boost()
    {
        drifting = false;

        if (_driftMode > 0)
        {
            DOVirtual.Float(_currentSpeed * 3, _currentSpeed, .3f * _driftMode, Speed);
            DOVirtual.Float(0, 1, .5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, .5f, ChromaticAmount));
            kartModel.Find("Tube001").GetComponentInChildren<ParticleSystem>().Play();
            kartModel.Find("Tube002").GetComponentInChildren<ParticleSystem>().Play();
        }

        _driftPower = 0;
        _driftMode = 0;
        _first = false;
        _second = false;
        _third = false;

        foreach (var p in primaryParticles)
        {
            p.startColor = Color.clear;
            p.Stop();
        }

        kartModel.parent.DOLocalRotate(Vector3.zero, driftRecoverRotationDuration).SetEase(Ease.OutBack);
    }

    public void Steer(int direction, float amount)
    {
        _rotate = steering * direction * amount;
    }

    public void ColorDrift()
    {
        if (!_first)
            _c = Color.clear;

        if (_driftPower > 50 && _driftPower < 100 - 1 && !_first)
        {
            _first = true;
            _c = turboColors[0];
            _driftMode = 1;

            PlayFlashParticle(_c);
        }

        if (_driftPower > 100 && _driftPower < 150 - 1 && !_second)
        {
            _second = true;
            _c = turboColors[1];
            _driftMode = 2;

            PlayFlashParticle(_c);
        }

        if (_driftPower > 150 && !_third)
        {
            _third = true;
            _c = turboColors[2];
            _driftMode = 3;

            PlayFlashParticle(_c);
        }

        foreach (var p in primaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = _c;
        }

        foreach (var p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = _c;
        }
    }

    private void PlayFlashParticle(Color c)
    {
        return;
        GameObject.Find("CM vcam1").GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        foreach (var p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
            p.Play();
        }
    }

    public void SetAcceleration(float x)
    {
        acceleration = x;
    }

    public float GetSpeed()
    {
        return _currentSpeed;
    }

    private void Speed(float x)
    {
        _currentSpeed = x;
    }

    private void ChromaticAmount(float x)
    {
        _postProfile.GetSetting<ChromaticAberration>().intensity.value = x;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    //}
}