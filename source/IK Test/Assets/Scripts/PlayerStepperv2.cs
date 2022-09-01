using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepperv2 : MonoBehaviour
{

    public Transform home;                           // location that the foot will track to
    [SerializeField] private float stepDistance;     // how far from home the foot can get before it moves
    [SerializeField] private float stepTime;         // how long it takes to move the foot
    [SerializeField] private float overstepAmount;   // how far the foot overshoots the home position
    [SerializeField] private float stepHeightFactor; // how high you step

    [HideInInspector] public bool Moving;

    // starts movement coroutine if one is not already running
    public void TryMoving()
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

        // saving initial info for Lerping and Slerping
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // calculating endpoint of step based on home position, direction to home, and overstep distance
        Vector3 homeDirection = home.position - transform.position;
        float overstepDistance = stepDistance * overstepAmount;
        Vector3 overstepVector = homeDirection * overstepDistance;
        Vector3 endPosition = home.position + overstepVector;

        // calculating centerpoint of step based on distance from start to end point. height is based on distance + a modifier
        Vector3 centerPosition = (startPosition + endPosition) / 2;
        centerPosition += transform.up * Vector3.Distance(startPosition, endPosition) / 2f;

        // actually doing the stepping here
        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / stepTime;

            //Nested Lerping
            transform.position = Vector3.Lerp(
                Vector3.Lerp(startPosition, centerPosition, normalizedTime),
                Vector3.Lerp(centerPosition, endPosition, normalizedTime),
                normalizedTime
            );

            yield return null;
        }
        while (timeElapsed < stepTime);

        Moving = false;
    }
}
