using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKController : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private bool ikActive;
    [SerializeField] private bool showBonePoints;
    [Header("Bone Points")]
    [SerializeField] private Transform rhPoint;
    [SerializeField] private Transform lhPoint;
    [SerializeField] private Transform rePoint;
    [SerializeField] private Transform lePoint;
    [SerializeField] private Transform rfPoint;
    [SerializeField] private Transform lfPoint;
    [SerializeField] private Transform rkPoint;
    [SerializeField] private Transform lkPoint;

    [Header("Steppers")]
    [SerializeField] private PlayerStepper rfStepper;
    [SerializeField] private PlayerStepper lfStepper;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        if (!showBonePoints)
        {
            rhPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            rePoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            lhPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            lePoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            rfPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            rkPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            lfPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            lkPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void Awake()
    {
        StartCoroutine(PlayerStepping());
    }

    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, 0.002f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(0, 0, 0.002f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(0.002f, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= new Vector3(0.002f, 0, 0);
        }
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
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rfPoint.position);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rfPoint.rotation);
            animator.SetIKHintPosition(AvatarIKHint.RightKnee, rkPoint.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1);

            // left leg
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, lfPoint.position);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, lfPoint.rotation);
            animator.SetIKHintPosition(AvatarIKHint.LeftKnee, lkPoint.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1);
        }
    }

    private IEnumerator PlayerStepping()
    {
        while (true)
        {
            do
            {
                rfStepper.TryMove();

                yield return null;
            } 
            while (rfStepper.Moving);

            do
            {
                lfStepper.TryMove();

                yield return null;
            }
            while (lfStepper.Moving);
        }
    }
}
