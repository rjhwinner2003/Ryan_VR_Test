using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThreeMovement : MonoBehaviour
{
    [SerializeField] SaveManager saveManager = new SaveManager();
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject focusObject;
    Quaternion lastRotation = Quaternion.identity;
    Quaternion currentRotation = Quaternion.identity;
    Vector3 lastFacing = Vector3.zero;
    private float testDuration;
    private float currentTime = 0;
    private bool testOver = false;
    bool inThreshold = false;
    int frequencyCounter = 0;
    float frequency = 0;
    float speed = 0;
    float lastTime = 0;
    void Start()
    {
        testDuration = PlayerPrefs.GetFloat("TestThreeDuration");
        focusObject.transform.position = new Vector3(0, camera.transform.position.y, 1);
        focusObject.isStatic = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentTime < testDuration)
        {
            UpdateVariables();
        }
        else
        {
            testOver = true;
        }
        if (testOver)
            saveManager.setTestOver();
    }

    void UpdateVariables()
    {
        //Calc Speed
        if (Time.time - lastTime > 0.01f)
        {
            lastTime = Time.time;
            currentRotation = camera.transform.rotation;
            speed = (Quaternion.Angle(currentRotation, lastRotation) / 0.01f);
            lastRotation = currentRotation;
        }
        //Calc Frequency
        frequency = frequencyCounter;
        //frequencyCounter = 0;
        lastFacing = Camera.main.transform.forward;

        Vector3 target = GetTargetVector();
        float leftAngle = Vector3.Angle(saveManager.currentData.leftGazeDirectionNormalized - saveManager.currentData.leftGazeOriginMM, target - saveManager.currentData.leftGazeOriginMM);
        float rightAngle = Vector3.Angle(saveManager.currentData.leftGazeDirectionNormalized - saveManager.currentData.leftGazeOriginMM, target - saveManager.currentData.leftGazeOriginMM);
        leftAngle /= 180;
        rightAngle /= 180;
        saveManager.currentData.leftEyePrecision = 1 - leftAngle;
        saveManager.currentData.rightEyePrecision = 1 - rightAngle;
        saveManager.currentData.speed = speed;
        saveManager.currentData.frequency = GetFrequencyPerSecond();
    }

    float GetFrequencyPerSecond()
    {
        return frequency / Time.time;
    }

    void IncrementFrequency()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(camera.transform.position, camera.transform.forward, Color.red);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Threshold" && !inThreshold)
            {
                frequencyCounter++;
                inThreshold = true;
            }
            else if (hit.collider.tag != "Threshold" && inThreshold)
                inThreshold = false;
        }
        else
        {
            inThreshold = false;
        }
        /*
        float currentYRotation = headTracker.transform.forward.y;
        Vector3 center = focusObject.transform.position - headTracker.transform.position;
        if((currentYRotation > 0 && lastFacing.y < 0) || (currentYRotation < 0 && lastFacing.y > 0))
        {
            frequencyCounter++;
        }*/
    }

    public Vector3 GetTargetVector()
    {
        return focusObject.transform.localPosition;
    }

    public bool GetTestOver()
    {
        return testOver;
    }
}
