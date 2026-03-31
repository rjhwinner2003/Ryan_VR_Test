using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Valve.VR;
using ViveSR.anipal.Eye;

public class TestThreeDataManager : MonoBehaviour
{
    [SerializeField] TestThreeMovement testThreeManager;
    [SerializeField] GameObject headTracker;
    [SerializeField] GameObject focusObject;
    [SerializeField] GameObject frequencyThreshold;
    [SerializeField] FocusManager focusManager;
    [SerializeField] Camera headCamera;
    float lastTime = 0;
    bool saved = false;
    float speed = 0;
    int frequencyCounter = 0;
    float frequency = 0;
    Vector3 lastFacing = Vector3.zero;
    Quaternion lastRotation = Quaternion.identity;
    Quaternion currentRotation = Quaternion.identity;
    bool inThreshold = false;
    FrequencyTracker frequencyTracker;

    [System.Serializable]
    public class TestData
    {
        public Vector3 leftGazeDirectionNormalized;
        public Vector3 rightGazeDirectionNormalized;
        public Vector3 leftGazeOriginMM;
        public Vector3 rightGazeOriginMM;
        public float leftEyePrecision;
        public float rightEyePrecision;
        public float speed;
        public float frequency;
    }
    List<TestData> testData = new List<TestData>();
    // Start is called before the first frame update
    void Start()
    {
        lastRotation = headTracker.transform.rotation;
        frequencyTracker = new FrequencyTracker(-15,15,headTracker.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastTime > 0.01f)
        {
            //Calc Speed
            lastTime = Time.time;
            currentRotation = headTracker.transform.rotation;
            Debug.Log("Current is: " + currentRotation);
            Debug.Log("Current is: " + lastRotation);
            speed = (Quaternion.Angle(currentRotation, lastRotation) / 0.01f);
            lastRotation = currentRotation;
            //Calc Frequency
            frequency = frequencyCounter;
            //frequencyCounter = 0;
            lastFacing = headTracker.transform.forward;
        }
        IncrementFrequency();
        TestData temp = new TestData();
        VerboseData currentData = new VerboseData();
        currentData = focusManager.GetVerboseData();
        temp.leftGazeDirectionNormalized = currentData.left.gaze_direction_normalized;
        temp.rightGazeDirectionNormalized = currentData.right.gaze_direction_normalized;
        temp.leftGazeOriginMM = currentData.left.gaze_origin_mm;
        temp.rightGazeOriginMM = currentData.right.gaze_origin_mm;
        float leftAngle = Vector3.Angle(temp.leftGazeDirectionNormalized - temp.leftGazeOriginMM, testThreeManager.GetTargetVector());
        float rightAngle = Vector3.Angle(temp.rightGazeDirectionNormalized - temp.rightGazeOriginMM, testThreeManager.GetTargetVector());
        Debug.Log("Left Angle is" + leftAngle);
        leftAngle /= 180;
        rightAngle /= 180;
        temp.leftEyePrecision = 1 - leftAngle;
        temp.rightEyePrecision = 1 - rightAngle;
        temp.speed = speed;
        temp.frequency = GetFrequencyPerSecond();
        testData.Add(temp);
        if (testThreeManager.GetTestOver() && !saved)
        {
            Save();
            Debug.Log("SAVING");
        }
        if (saved)
            NextScene();
    }

    float GetFrequencyPerSecond()
    {
        return frequency / Time.time;
    }

    void IncrementFrequency()
    {
        Ray ray = new Ray(headTracker.transform.position, headTracker.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(headTracker.transform.position, headTracker.transform.forward, Color.red);
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

    void NextScene()
    {
        SceneManager.LoadScene(4);
    }

    void Save()
    {
        string path = "PatientResults\\" + PlayerPrefs.GetString("Name") + "\\Test Results " + System.DateTime.Today.ToString("MM-dd-yyyy");
        string filename = path + "\\Test 3.csv";
        bool dirExists = Directory.Exists(path);
        if(!dirExists)
            Directory.CreateDirectory(path);

        using (var stream = File.Open(filename, FileMode.Create))
        {
            var wtr = new StreamWriter(stream, System.Text.Encoding.UTF8);
            //write first line
            wtr.WriteLine("Left Gaze Direction Normalized" + "," +
                          "Right Gaze Direction Normalized" + "," +
                          "Left Gaze Origin MM" + "," +
                          "Right Gaze Origin MM" + "," +
                          "Left Eye Precision" + "," +
                          "Right Eye Precision" + "," +
                          "Speed" + "," +
                          "Frequency");
            for (int i = 0; i < testData.Count; i++)
            {
                string line = ToCSVFriendlyString(testData[i].leftGazeDirectionNormalized) + "," +
                              ToCSVFriendlyString(testData[i].rightGazeDirectionNormalized) + "," +
                              ToCSVFriendlyString(testData[i].leftGazeOriginMM) + "," +
                              ToCSVFriendlyString(testData[i].rightGazeOriginMM) + "," +
                              testData[i].leftEyePrecision + "," +
                              testData[i].rightEyePrecision + "," +
                              testData[i].speed + "," +
                              testData[i].frequency;
                wtr.WriteLine(line);
            }
            wtr.Close();
        }
        saved = true;
    }

    string ToCSVFriendlyString(Vector3 vector)
    {
        string output = "(" + vector.x + " ; " + vector.y + " ; " + vector.z + ")";
        return output;
    }
}

