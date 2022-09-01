using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Stepping : MonoBehaviour
{
    [SerializeField] private Transform homeTransform;
    [SerializeField] private float stepDistance;
    [SerializeField] private float moveDuration;
    [SerializeField] private float overshootFraction;
    [SerializeField] private int upAxis;
    private Vector3 localUp;

    public bool Moving;

    private void Start()
    {
        if (upAxis == 1)
        {
            localUp = homeTransform.forward;
        }
        else if (upAxis == 2)
        {
            localUp = homeTransform.right;
        }
        else if (upAxis == 3)
        {
            localUp = -homeTransform.right;
        }
    }

    public void TryMove()
    {
        if (Moving) return;

        float distance = Vector3.Distance(transform.position, homeTransform.position);

        if (distance > stepDistance)
        {
            StartCoroutine(MoveToHome());
        }
    }

    IEnumerator MoveToHome()
    {
        Moving = true;
        Quaternion startRotation = transform.rotation;
        Vector3 startPoint = transform.position;

        Quaternion endRotation = homeTransform.rotation;

        // overstepping
        Vector3 homeDirection = homeTransform.position - transform.position;

        float overshootDistance = stepDistance * overshootFraction;
        Vector3 overshootVector = homeDirection * overshootDistance;

        Vector3 endPoint = homeTransform.position + overshootVector;

        Vector3 centerPoint = (startPoint + endPoint) / 2;

        centerPoint += localUp * Vector3.Distance(startPoint, endPoint) / 2f;
        // redo this to actually understand it
        

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.InOutCubic(normalizedTime);

            // nested lerping for some reason
            transform.position =
                Vector3.Lerp(
                        Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                        Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                        normalizedTime
                    );

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        Moving = false;
    }
}
