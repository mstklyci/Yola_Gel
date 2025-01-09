using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSign : MonoBehaviour
{
    [SerializeField] private GameObject[] targetCars;
    private CarAI[] carAIsc;
    public int rotation;

    private void Awake()
    {
        carAIsc = new CarAI[targetCars.Length];
        if (targetCars.Length != 0)
        {
            for (int i = 0; i < targetCars.Length; i++)
            {
                carAIsc[i] = targetCars[i].GetComponent<CarAI>();
            }
        }     
    }

    public void SelectSign(int sign)
    {
        if (targetCars.Length != 0)
        {
            foreach (CarAI carAI in carAIsc)
            {
                if (carAI != null)
                {
                    carAI.SelectRoad(sign);
                }
            }
        }
    }
}