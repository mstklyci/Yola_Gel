using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public void startButton()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");

        foreach (GameObject car in cars)
        {
            Car carMoveSc = car.GetComponent<Car>();
            
            if (carMoveSc!= null)
            {
                carMoveSc.enabled = true;
            }
        }
    }
}