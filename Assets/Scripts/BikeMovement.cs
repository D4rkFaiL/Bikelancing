using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeMovement : MonoBehaviour
{
    [Header("Movement Info")]
    [SerializeField] float _momentum;
    [SerializeField] float _momentumLoss;

    [Header("Steering")]
    [SerializeField] float _steerValue;
    [SerializeField] float _steerForce;

    [Header("Pedal Info")]
    [SerializeField] float _pedalForce;

    [Header("Mesh Update")]
    [SerializeField] Transform _backWheel;
    [SerializeField] Transform _frontWheel;
    [SerializeField] Transform _guidao;
    [SerializeField] Transform _pedal;

    [Header("FrontWheel")]
    [SerializeField] float _frontWheelSmoothVelocity;
    [SerializeField] float _frontWheelTurnAngle;
    float _frontWheelRotation;
    float _frontWheelRotationSmooth;

    [Header("Pedal Stuff")]
    [SerializeField] float _pedalSmoothVelocity;
    int _pedalIndex;
    int _pedalRotation;
    float _pedalRotationSmooth;

    [Header("Particles")]
    [SerializeField] ParticleSystem dashParticle;
    
    [Header("Audio")]
    [SerializeField] float _clickTime = 0;
    [SerializeField] float _clickRatio = 10;
    [SerializeField] AudioClip[] _audioClip;
    [SerializeField] AudioSource[] _audioSource;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        if(!GameManager.instance.RunStarted()) return;

        _momentum -= Time.deltaTime * _momentumLoss;
        _momentum = Mathf.Clamp(_momentum,0,9999999999);
        if(_clickTime > _clickRatio / _momentum && _momentum > 100){
            _clickTime = 0;
            _audioSource[1].pitch = Random.Range(0.85f,1.15f);
            _audioSource[1].PlayOneShot(_audioClip[1]);
        }
        _clickTime += Time.deltaTime;

        var velo = -transform.forward * _momentum * Time.deltaTime;
        rb.velocity = new Vector3(velo.x,rb.velocity.y,velo.z);

        _steerValue += (_balance2 * _steerForce * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0,_steerValue,transform.eulerAngles.z);

        UpdateMesh();
    }

    void UpdateMesh(){
        _pedalRotationSmooth = Mathf.Lerp(_pedalRotationSmooth,_pedalRotation,(Time.deltaTime * _pedalSmoothVelocity));
        _pedal.localRotation = Quaternion.Euler(_pedalRotationSmooth,0,0);

        _frontWheelRotation += Mathf.Abs(rb.velocity.magnitude) * _frontWheelSmoothVelocity * -1;//Mathf.Lerp(_frontWheelRotation,_momentum,Time.deltaTime * _frontWheelSmoothVelocity);
        var frontWheelYRotation = 90 + (_frontWheelTurnAngle * _balance2/90);
        _frontWheel.localRotation = Quaternion.Euler(0, frontWheelYRotation, _frontWheelRotation);

        _backWheel.localRotation = Quaternion.Euler(0, 90,_frontWheelRotation);

        var guidaoRot = (frontWheelYRotation - 90) + _guidaoPlayerRot;
        _guidao.localRotation = Quaternion.Euler(-90,0,guidaoRot);
    }

    float _guidaoPlayerRot = 0;
    [SerializeField] float _guidaoRotMultiplier = 2;
    public void GuidaoAddRot(float amount){
        _guidaoPlayerRot += amount * _guidaoRotMultiplier;
        _guidaoPlayerRot = Mathf.Clamp(_guidaoPlayerRot,-45,45);
    }
    
    void AddMomentum(float amount){
        _momentum += amount;
    }

    [SerializeField] Transform _dashWave;
    public void PedalBike(){
        
        _audioSource[0].pitch = Random.Range(0.85f,1.15f);
        _audioSource[0].PlayOneShot(_audioClip[0]);

        AddMomentum(_pedalForce);
        //dashParticle.Play();
        var insta = Instantiate(_dashWave,transform);
        insta.localPosition = Vector3.zero;
        insta.localRotation = Quaternion.Euler(180,0,0);
    }

    float _balance2 = 0;
    public void UpdateBalance(float value){
        _balance2 = value;
    }

    public void MovePedals(){
        _pedalRotation = -90 * _pedalIndex;
        _pedalIndex++;

        GameManager.instance.StartTimer();
    }

    void OnCollisionEnter(Collision other)
    {
        _momentum = 0;
    }

    public void ResetValues(){
        rb.velocity = Vector3.zero;
        _guidaoPlayerRot = 0;
        _momentum = 0;
        _steerValue = -180;
        _pedalIndex = 0;
        _balance2 = 0;
        _pedalRotationSmooth = 0;
        UpdateMesh();     
    }
}
