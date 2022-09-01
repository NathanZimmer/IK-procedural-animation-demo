using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerStepper : MonoBehaviour
{
    [SerializeField] private Transform home;
    [SerializeField] private float stepDistance;
    [SerializeField] private float stepTime;
    [SerializeField] private float overStepingFraction;
    [SerializeField] private float movementModifier;

    [HideInInspector] public bool Moving;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            home.position += movementModifier * transform.forward;
        }
        if(Input.GetKeyUp(KeyCode.W))
        {
            home.position -= movementModifier * transform.forward;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            home.position -= movementModifier * transform.forward;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            home.position += movementModifier * transform.forward;
        }
    }

    public void TryMove()
    {
        if (Moving) return;

        float distance = Vector3.Distance(transform.position, home.position);

        if (distance > stepDistance)
        {
            StartCoroutine(MoveToHome());
        }
    }

    private IEnumerator MoveToHome()
    {
        Moving = true;
        Quaternion startRotation = transform.rotation;
        Vector3 startPoint = transform.position;

        Vector3 homeDirection = home.position - transform.position;
        float overshootDistance = stepDistance * overStepingFraction;
        Vector3 overstepVector = homeDirection * overshootDistance;
        
        Vector3 endPoint = home.position + overstepVector;

        Vector3 centerPoint = (startPoint + endPoint) / 2;
        centerPoint += transform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / stepTime;
            normalizedTime = Easing.InOutCubic(normalizedTime);

            
            transform.SetPositionAndRotation(
                Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime), 
                normalizedTime), 
                Quaternion.Slerp(startRotation, home.rotation, normalizedTime)
            );

            yield return null;
        }
        while (timeElapsed < stepTime);

        Moving = false;
    }
}
