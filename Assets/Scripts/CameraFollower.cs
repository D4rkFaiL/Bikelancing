using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] Transform _objectToFollow;
    [SerializeField] Vector3 offset;
    void Update()
    {
        transform.position = _objectToFollow.position + offset;
    }
}
