using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class Car : MonoBehaviour
{
    SpecialRoads specialRoad;

    Rigidbody2D rb;

    //Car Variables
    public float maxSpeed;
    [SerializeField] private float acceleration, rotationSpeed, currentSpeed, braking;
    [SerializeField] private float rotation, rotationValue;
    [SerializeField] private float weight, height;

    //Wheel
    [SerializeField] private GameObject WheelLeft, WheelRight;
    private Vector3 CarAngle;
    private float wheelAngle, maxWheelAngle = 30f;

    //Effect
    [SerializeField] private ParticleSystem crashEffect;
    [SerializeField] private SpriteRenderer leftLight, rightLight,backLight;
    [SerializeField] private Sprite rightLightOn, leftLightOn, backLightOn;
    private Sprite backLightOff, rightLightOff, leftLightOff;
    private bool lightAnim;

    //Light
    [SerializeField] private Light2D leftSignalLight, rightSignalLight, leftSignalLightBack, rightSignalLightBack , stopSignalLeft, stopSignalRight;
    [SerializeField] private bool police;
    [SerializeField] private Light2D policeLightRed, policeLightBlue;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rotationValue = transform.eulerAngles.z;
        crashEffect.Stop();

        specialRoad = GameObject.FindWithTag("SpecialRoad").GetComponent<SpecialRoads>();

        rightLightOff = rightLight.sprite;
        leftLightOff = leftLight.sprite;
        backLightOff = backLight.sprite;
        lightAnim = false;

        if(leftSignalLight != null && rightSignalLight != null && leftSignalLightBack != null && rightSignalLightBack != null)
        {
            leftSignalLight.enabled = false;
            rightSignalLight.enabled = false;
            leftSignalLightBack.enabled = false;
            rightSignalLightBack.enabled = false;
        }

        if(stopSignalLeft != null && stopSignalRight != null)
        {
            stopSignalLeft.enabled = false;
            stopSignalRight.enabled = false;
        }

        if(policeLightBlue!= null || policeLightRed != null)
        {
            policeLightBlue.enabled = false;
            policeLightRed.enabled = false;
        }

        if (police == true)
        {
            StartCoroutine(PoliceLight());
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void LateUpdate()
    {
        CarAngle = transform.localEulerAngles;
        CarAngle.z = wheelAngle;
        WheelLeft.transform.localEulerAngles = CarAngle;
        WheelRight.transform.localEulerAngles = CarAngle;
    }

    private void Movement()
    {
        currentSpeed = rb.velocity.magnitude;

        if(maxSpeed > currentSpeed)
        {
            rb.AddForce(transform.up * acceleration, 0);       
        }
        else
        {
            if (currentSpeed > 0.1f) 
            {
                rb.AddForce(-rb.velocity * braking, 0);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
     
        rotationValue -= rotation * rotationSpeed;
        rb.MoveRotation(rotationValue);
        rb.velocity = transform.up * rb.velocity.magnitude;

        if (rotation > 0.5f && !lightAnim)
        {
            StartCoroutine(LightController(rightLight, rightLightOn, rightLightOff, rightSignalLight, rightSignalLightBack));
        }
        else if (rotation < -0.5f && !lightAnim)
        {
            StartCoroutine(LightController(leftLight, leftLightOn, leftLightOff, leftSignalLight, leftSignalLightBack));
        }
        else if (!lightAnim)
        {
            leftLight.sprite = leftLightOff;
            rightLight.sprite = rightLightOff;
        }

        if (maxSpeed + 0.5f > currentSpeed)
        {
            backLight.sprite = backLightOff;
            stopSignalLeft.enabled = false;
            stopSignalRight.enabled = false;
        }
        else
        {
            backLight.sprite = backLightOn;
            stopSignalLeft.enabled = true;
            stopSignalRight.enabled = true;
        }
    }

    public void SetInput(float rotationValue)
    {
        rotation = rotationValue;
        wheelAngle = -rotationValue * maxWheelAngle;
    }

    private IEnumerator LightController(SpriteRenderer currentLight, Sprite currentLightOn, Sprite currentLightOff, Light2D currentSiganl , Light2D currentSiganlBack)
    {
        lightAnim = true;

        for (int i = 0; i < 3; i++) 
        {
            currentLight.sprite = currentLightOn;
            currentSiganl.enabled = true;
            currentSiganlBack.enabled = true;
            yield return new WaitForSeconds(0.5f);
            currentLight.sprite = currentLightOff;
            currentSiganl.enabled = false;
            currentSiganlBack.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }

        lightAnim = false;
    }

    private IEnumerator PoliceLight()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                policeLightBlue.enabled = true;
                policeLightRed.enabled = false;
                yield return new WaitForSeconds(0.1f);
                policeLightBlue.enabled = false;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < 3; i++)
            {
                policeLightRed.enabled = true;
                policeLightBlue.enabled = false;
                yield return new WaitForSeconds(0.1f);
                policeLightRed.enabled = false;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        crashEffect.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "SpecialRoad")
        {
           if(height > specialRoad.maxHeight)
           {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                crashEffect.Play();
           }
           if(weight > specialRoad.maxWeight)
           {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                crashEffect.Play();
           }
        }
    }
}