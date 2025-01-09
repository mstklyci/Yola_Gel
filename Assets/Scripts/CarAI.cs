using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    [SerializeField] private WayPoints[] WayPoints;
    private WayPoints currentWayPoint;
    private Vector3 targetPosition;
    private Car car;

    [SerializeField] private float stopTime;

    private void Awake()
    {
        car = GetComponent<Car>();

        currentWayPoint = WayPoints[0];
    }

    private void FixedUpdate()
    {
        float rotationValue;
        rotationValue = TurnNextPoint();
        car.SetInput(rotationValue);
        Road();
    }

    public void SelectRoad(int currentRoad)
    {
        currentWayPoint = WayPoints[currentRoad];
    }

    private void Road()
    { 
        if (currentWayPoint != null)
        {
            if(currentWayPoint.stopSign ==  true)
            {
                car.maxSpeed = 0;
                Invoke(nameof(Stop), stopTime);
            }
            else
            {
                car.maxSpeed = currentWayPoint.pointMaxSpeed;
            }

            targetPosition = currentWayPoint.transform.position;
            float distanceToWayPoint = (targetPosition - transform.position).magnitude;
           
            if (distanceToWayPoint <= currentWayPoint.minDistance)
            {
                currentWayPoint = currentWayPoint.nextPoint;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Stop()
    {
        currentWayPoint.stopSign = false;
    }

    private float TurnNextPoint()
    {
        Vector2 nextPoint = (targetPosition - transform.position).normalized;

        float targetAngle = Vector2.SignedAngle(transform.up, nextPoint);
        targetAngle = Mathf.Clamp(-targetAngle / 45f, -1,1);

        return targetAngle;
    }
}