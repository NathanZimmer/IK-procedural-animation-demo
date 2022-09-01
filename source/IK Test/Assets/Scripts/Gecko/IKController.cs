using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class IKController : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private Transform target;
    [SerializeField] private Transform headBone;
    [SerializeField] private float speed;
    [SerializeField] private float maxAngle;
    [SerializeField] private float eyeSpeed;
    [SerializeField] private float maxEyeAngle;
    [SerializeField] private Transform leftEyeBone;
    [SerializeField] private Transform rightEyeBone;

    [SerializeField] private Stepping rhStepping;
    [SerializeField] private Stepping lhStepping;
    [SerializeField] private Stepping rlStepping;
    [SerializeField] private Stepping llStepping;

    [SerializeField] private float bodyturnSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnAccel;
    [SerializeField] private float moveAccel;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    [SerializeField] private float maxBodyAngle;
    

    public bool ikActive = false;
    public bool showBonePoints = true;
    public Transform rhPoint;
    public Transform rePoint;
    public Transform lhPoint;
    public Transform lePoint;
    public Transform rlPoint;
    public Transform rkPoint;
    public Transform llPoint;
    public Transform lkPoint;

    private Quaternion eyeLookPoint;
    private Quaternion headLookPoint;
    private Vector3 currentVelocity;
    private float currentAngularVelocity;

    private void Awake()
    {
        StartCoroutine(LegMovementCoroutine());
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (!showBonePoints)
        {
            rhPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            rePoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            lhPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            lePoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            rlPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            rkPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            llPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            lkPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void LateUpdate()
    {
        HeadTracking();
        //EyeTracking(leftEyeBone);
        //EyeTracking(rightEyeBone);
        MotionUpdate();
    }

    private void HeadTracking()
    {
        // getting direction between head and target in local coordinates
        Vector3 localDirection = headBone.InverseTransformDirection(target.position - headBone.position);

        // using RotateTowards instead of clamping x, y, z because we can specifiy a max angle 
        Vector3 directionToTarget = Vector3.RotateTowards(Vector3.forward, localDirection, Mathf.Deg2Rad * maxAngle, 0);

        // setting head rotation to a spherical lerp from current rotation to value calculated from the look angle
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, transform.up);
        headLookPoint = Quaternion.Slerp(headBone.localRotation, targetRotation, 1 - Mathf.Exp(-speed * Time.deltaTime));
    }

    private void EyeTracking(Transform eyeBone)
    {
        Vector3 localDirection = eyeBone.InverseTransformDirection(target.position - eyeBone.position);
        Vector3 directionToTarget = Vector3.RotateTowards(Vector3.forward, localDirection, Mathf.Deg2Rad * maxEyeAngle, 0);
        Quaternion tarrgetRotation = Quaternion.LookRotation(directionToTarget, transform.up);
        eyeLookPoint = Quaternion.Slerp(eyeBone.localRotation, tarrgetRotation, 1 - Mathf.Exp(-eyeSpeed * Time.deltaTime));
    }

    private void TailMovement()
    {
        // gonna need to brainstorm this for a while
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator && ikActive)
        {
            // right hand
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rhPoint.position);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rhPoint.rotation);
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, rePoint.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);

            // left hand
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, lhPoint.position);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, lhPoint.rotation);
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, lePoint.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);

            // right leg
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rlPoint.position);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rlPoint.rotation);
            animator.SetIKHintPosition(AvatarIKHint.RightKnee, rkPoint.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1);

            // left leg
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, llPoint.position);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, llPoint.rotation);
            animator.SetIKHintPosition(AvatarIKHint.LeftKnee, lkPoint.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1);

            // eyes and head
            animator.SetBoneLocalRotation(HumanBodyBones.LeftEye, eyeLookPoint);
            animator.SetBoneLocalRotation(HumanBodyBones.RightEye, eyeLookPoint);
            animator.SetBoneLocalRotation(HumanBodyBones.Head, headLookPoint);
        }
    }

    private IEnumerator LegMovementCoroutine()
    {
        while (true)
        {
            do
            {
                lhStepping.TryMove();
                rlStepping.TryMove();

                yield return null;
            }
            while (lhStepping.Moving || rlStepping.Moving);

            do
            {
                rhStepping.TryMove();
                llStepping.TryMove();

                yield return null;
            } 
            while (rhStepping.Moving || llStepping.Moving);
        }
    }

    private void MotionUpdate()
    {
        Vector3 directionToTarget = target.position - transform.position;
        Vector3 directionProjected = Vector3.ProjectOnPlane(directionToTarget, transform.up);

        float angleToTarget = Vector3.SignedAngle(transform.forward, directionProjected, transform.up);

        float targetAngularVelocity = 0;

        if (Mathf.Abs(angleToTarget) > maxBodyAngle)
        {
            if (angleToTarget > 0)
            {
                targetAngularVelocity = bodyturnSpeed;
            }
            else
            {
                targetAngularVelocity = -bodyturnSpeed;
            }
        }

        currentAngularVelocity = Mathf.Lerp(
            currentAngularVelocity,
            targetAngularVelocity,
            1 - Mathf.Exp(-turnAccel * Time.deltaTime)
        );

        transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);

        Vector3 targetVelocity = Vector3.zero;

        if(Mathf.Abs(angleToTarget) < 90)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > maxDistance)
            {
                targetVelocity = moveSpeed * directionProjected.normalized;
            }

            else if (distanceToTarget < minDistance)
            {
                targetVelocity = moveSpeed * -directionProjected.normalized;
            }
        }

        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            1 - Mathf.Exp(-moveAccel * Time.deltaTime)
        );

        transform.position += currentVelocity * Time.deltaTime;
    }
}
