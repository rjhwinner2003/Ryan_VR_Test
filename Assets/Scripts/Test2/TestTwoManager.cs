using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using ViveSR.anipal.Eye;

public class TestTwoManager : MonoBehaviour
{
    [SerializeField] SaveManager saveManager = new SaveManager();

    [SerializeField] private GameObject leftAngle;
    [SerializeField] private GameObject rightAngle;
    [SerializeField] private GameObject leftDestination;
    [SerializeField] private GameObject rightDestination;
    [SerializeField] private GameObject focusObject;
    private float angleToMove;
    private float testDuration;
    private float currentTime;
    private float maxSpeed;
    bool atLeft = false;
    bool atRight = true;
    bool moving = false;
    float totalDistance;
    float sinIterator = -90;
    bool testOver = false;
    // Start is called before the first frame update
    void Start()
    {
        angleToMove = PlayerPrefs.GetFloat("TestTwoDegreesFromCenter");
        testDuration = 30; PlayerPrefs.GetFloat("TestTwoDuration");
        maxSpeed = 0.8f; //Initialize to num in playerprefs
        currentTime = 0;
        InitializeAngles();
        focusObject.transform.position = rightDestination.transform.position;
        totalDistance = Vector3.Distance(rightDestination.transform.position, leftDestination.transform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (currentTime < testDuration)
        {
            HandleTesting();
            currentTime += Time.deltaTime;
            sinIterator += 0.02f;
            UpdateVariables();
        }
        else
            testOver = true;
        if (testOver)
            saveManager.setTestOver();
    }

    void UpdateVariables()
    {
        float leftAngle = Vector3.Angle(saveManager.currentData.leftGazeDirectionNormalized, GetTargetVector());
        float rightAngle = Vector3.Angle(saveManager.currentData.rightGazeDirectionNormalized, GetTargetVector());
        leftAngle /= 180;
        rightAngle /= 180;
        saveManager.currentData.leftEyePrecision = 1 - leftAngle;
        saveManager.currentData.rightEyePrecision = 1 - rightAngle;
    }

    public Vector3 GetTargetVector()
    {
        return focusObject.transform.localPosition - gameObject.transform.localPosition;
    }

    public bool GetTestOver()
    {
        return testOver;
    }

    void HandleTesting()
    {
        currentTime += Time.deltaTime;
        if (atLeft)
        {
            HandleMovement(rightDestination, true);
        }
        else if (atRight)
        {
            HandleMovement(leftDestination, false);
        }
    }

    void InitializeAngles()
    {
        leftAngle.transform.Rotate(Vector3.up, -angleToMove);
        rightAngle.transform.Rotate(Vector3.up, angleToMove);
        leftDestination.transform.localPosition += Vector3.forward;
        rightDestination.transform.localPosition += Vector3.forward;
        focusObject.transform.position = rightDestination.transform.position;
    }

    void HandleMovement(GameObject endDestination, bool headingRight)
    {
        float distance = Vector3.Distance(focusObject.transform.position, endDestination.transform.position);
        //float percentComplete = Mathf.Abs(distance) / Mathf.Abs(totalDistance);
        /*
        float speed = Mathf.Sin(percentComplete * Mathf.PI);
        speed *= maxSpeed;
        if (speed < 0.012f)
            speed = 0.012f;
        */
        if (sinIterator >= 90)
            sinIterator = -90;
        float pos = Mathf.Sin(sinIterator);
        pos += 1;
        pos /= 2;
        //focusObject.transform.position = Vector3.MoveTowards(focusObject.transform.position, endDestination.transform.position, Mathf.Abs(speed)*Time.deltaTime);
        focusObject.transform.position = Vector3.Lerp(leftDestination.transform.position, rightDestination.transform.position, pos);
        Debug.Log(Time.time);

        if (atRight && focusObject.transform.position == endDestination.transform.position)
        {
            atRight = false;
            atLeft = true;
        }
        else if(atLeft && focusObject.transform.position == endDestination.transform.position)
        {
            atRight = true;
            atLeft = false;
        }
    }
}
