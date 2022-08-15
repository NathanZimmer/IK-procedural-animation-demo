using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform headBone;
    [SerializeField] private float speed;
    [SerializeField] private float maxAngle;
    [SerializeField] private float eyeSpeed;
    [SerializeField] private float maxEyeAngle;
    [SerializeField] private Transform leftEyeBone;
    [SerializeField] private Transform rightEyeBone;

    private void LateUpdate()
    {
        HeadTracking();
        EyeTracking(leftEyeBone);
        EyeTracking(rightEyeBone);
    }

    private void HeadTracking()
    {
        // getting direction between head and target in local coordinates
        Vector3 localDirection = headBone.InverseTransformDirection(target.position - headBone.position);

        // using RotateTowards instead of clamping x, y, z because we can specifiy a max angle 
        Vector3 directionToTarget = Vector3.RotateTowards(Vector3.forward, localDirection, Mathf.Deg2Rad * maxAngle, 0);

        // setting head rotation to a spherical lerp from current rotation to value calculated from the look angle
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, transform.up);
        headBone.localRotation = Quaternion.Slerp(headBone.localRotation, targetRotation, 1 - Mathf.Exp(-speed * Time.deltaTime));
    }

    private void EyeTracking(Transform eyeBone)
    {
        // left eye
        Vector3 localDirection = eyeBone.InverseTransformDirection(target.position - eyeBone.position);
        Vector3 directionToTarget = Vector3.RotateTowards(Vector3.forward, localDirection, Mathf.Deg2Rad * maxEyeAngle, 0);
        Quaternion tarrgetRotation = Quaternion.LookRotation(directionToTarget, transform.up);
        eyeBone.localRotation = Quaternion.Slerp(eyeBone.localRotation, tarrgetRotation, 1 - Mathf.Exp(-eyeSpeed * Time.deltaTime));
    }
}
