using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeTimeTravel : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float _timeForEachRecording;
    [SerializeField] float _lerpingVelocity;
    [SerializeField] AnimationCurve _lerpingCurve;
    float _timer;

    [SerializeField] List<Vector3> _positions = new List<Vector3>();
    [SerializeField] List<Vector3> _rotations = new List<Vector3>();

    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        if(GameManager.instance.RunStarted()){
            if(_timer >= _timeForEachRecording){
                _timer = 0;
                _positions.Add(transform.position);
                _rotations.Add(transform.rotation.eulerAngles);
            }
            _timer += Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.T)){
            print("rewinding time");
            RewindTime();
        }

    }

    public void RewindTime(){
        StartCoroutine(RewindingTime());
    }

    IEnumerator RewindingTime(){

        _positions.Reverse();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        for (int i = 0; i < _positions.Count; i++)
        {
            var time = 0f;
            var animTime = 0f;
            var actualPosition = transform.position;
            var actualRotation = transform.rotation.eulerAngles;

            print("START");
            print("actual > " + actualRotation + " | moving >" + _positions[i]);
            while (animTime != 1)
            {
                time += Time.deltaTime * _lerpingVelocity;
                animTime = _lerpingCurve.Evaluate(time);

                var p = Vector3.Lerp(actualPosition,_positions[i],animTime);      
                var r = Vector3.Lerp(actualRotation,_rotations[i],animTime);    

                transform.position = p;
                transform.rotation = Quaternion.Euler(r.x,r.y,r.z);

                yield return null;         
            }
            print("reached > " + transform.rotation.eulerAngles);
            print("END");
            yield return new WaitForSeconds(0.5f);
        }

        // _positions.Clear();
        //_rotations.Clear();
    }
}
