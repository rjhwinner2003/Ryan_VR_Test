using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViveSR.anipal.Eye;
using Microsoft.Win32;
using System;
using Valve.VR;
using static UnityEngine.UI.Image;

public class SaveManager : MonoBehaviour
{
    [SerializeField] GameObject headset;
    [SerializeField] GameObject targetObj;
    public SaveManager()
    {
        currentData = new TestData();
        currentData.latency = 0;
        currentData.leftEyePrecision = 0;
        currentData.rightEyePrecision = 0;
        currentData.combinedPrecision = 0;
        currentData.speed = 0;
        currentData.frequency = 0;
    }

    VerboseData thisVerboseData = new VerboseData();

    [SerializeField] FocusManager focusManager;

    int currentTest;
    float currentTime = 0;
    bool testOver = false;
    bool saved = false;

    bool lvalid = false;
    bool rvalid = false;
    bool cvalid = false;

    //The data to be updated from the test
    public TestData currentData;

    //The data to save, collected from focusManager and currentData (above)
    List<TestData> testData;
    public class TestData
    {
        public Vector3 leftGazeDirectionNormalized;
        public Vector3 rightGazeDirectionNormalized;
        public Vector3 leftGazeOriginMM;
        public Vector3 rightGazeOriginMM;
        public float latency; // Test 1 Data
        public float leftEyePrecision; // Test 2/3 Data
        public float rightEyePrecision; // Test 2/3 Data
        public float combinedPrecision; // Test 2/3 Data
        public float speed; // Test 3 Data
        public float frequency; // Test 3 Data 
    }

    TestData lastValidTest;

    FrequencyTracker frequencyTracker;

    //call this function to end the test and initiate the save
    public void setTestOver()
    {
        testOver = true;
    }

    // Start is called before the first frame update
    void Start()
    {
         thisVerboseData = focusManager.GetVerboseData();
         currentData = new TestData();
         lastValidTest = new TestData();
         testData = new List<TestData>();
         currentTest = SceneManager.GetActiveScene().buildIndex;
         frequencyTracker = new FrequencyTracker(-15, 15, headset.transform);
    }

    // Update is called once per frame
    void Update()
    {
        thisVerboseData = focusManager.GetVerboseData();
        frequencyTracker.checkRotation();
        UpdateCurrentData();
        CollectData();
    }

    void UpdateCurrentData()
    {
        currentData.leftGazeDirectionNormalized = thisVerboseData.left.gaze_direction_normalized;
        currentData.rightGazeDirectionNormalized = thisVerboseData.right.gaze_direction_normalized;
        currentData.leftGazeOriginMM = thisVerboseData.left.gaze_origin_mm;
        currentData.rightGazeOriginMM = thisVerboseData.right.gaze_origin_mm;
    }

    void CollectData()
    {
        TestData temp = new TestData();
        VerboseData thisVerboseData = focusManager.GetVerboseData();
        lvalid = thisVerboseData.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
        rvalid = thisVerboseData.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
        cvalid = thisVerboseData.combined.eye_data.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
        if (lvalid)
        {
            temp.leftGazeDirectionNormalized = thisVerboseData.left.gaze_direction_normalized;
            temp.leftGazeOriginMM = thisVerboseData.left.gaze_origin_mm;
            lastValidTest.leftGazeDirectionNormalized = thisVerboseData.left.gaze_direction_normalized;
            lastValidTest.leftGazeOriginMM = thisVerboseData.left.gaze_origin_mm;
        }
        if (rvalid)
        {
            temp.rightGazeDirectionNormalized = thisVerboseData.right.gaze_direction_normalized;
            temp.rightGazeOriginMM = thisVerboseData.right.gaze_origin_mm;
            lastValidTest.rightGazeDirectionNormalized = thisVerboseData.right.gaze_direction_normalized;
            lastValidTest.rightGazeOriginMM = thisVerboseData.right.gaze_origin_mm;
        }
        GetTestVariables(temp);
        temp.frequency = frequencyTracker.getFrequency() / currentTime;
        //Debug.Log("Frequency is: " + temp.frequency);
        testData.Add(temp);
        currentTime += Time.deltaTime;
        if (testOver)
            Save();
        if (saved)
            NextScene();
    }

    void GetTestVariables(TestData temp)
    {
        if(currentTest == 1)
        {
            temp.latency = currentData.latency;
        }
        else if(currentTest == 2)
        {
            if (lvalid)
            {
                //left eye precision:
                Vector3 ldirection = GetDirection(temp.leftGazeDirectionNormalized);
                Vector3 lorigin = GetOrigin(temp.leftGazeOriginMM * 0.001f);
                lorigin.x *= -1;
                temp.leftEyePrecision = GetPrecision(lorigin, ldirection);
                lastValidTest.leftEyePrecision = temp.leftEyePrecision;
            }
            else
            {
                temp.leftEyePrecision = lastValidTest.leftEyePrecision;
            }
            if (rvalid)
            {
                //right eye precision:
                Vector3 rdirection = GetDirection(temp.rightGazeDirectionNormalized);
                Vector3 rorigin = GetOrigin(temp.rightGazeOriginMM * 0.001f);
                rorigin.x *= -1;
                temp.rightEyePrecision = GetPrecision(rorigin, rdirection);
                lastValidTest.rightEyePrecision = temp.rightEyePrecision;
            }
            else
            {
                temp.rightEyePrecision = lastValidTest.rightEyePrecision;
            }
            if (cvalid)
            {
                //combined precision
                Vector3 cdirection = GetDirection(focusManager.GetVerboseData().combined.eye_data.gaze_direction_normalized);
                Vector3 corigin = GetOrigin(focusManager.GetVerboseData().combined.eye_data.gaze_origin_mm * 0.001f);
                temp.combinedPrecision = GetPrecision(corigin, cdirection);
                lastValidTest.combinedPrecision = temp.combinedPrecision;
            }
            else
            {
                temp.combinedPrecision = lastValidTest.combinedPrecision;
            }
            //temp.speed = -1;
            //temp.frequency = -1;
        }
        else if(currentTest == 3)
        {
            //temp.latency = -1;
            if (lvalid)
            {
                //left eye precision:
                Vector3 ldirection = GetDirection(temp.leftGazeDirectionNormalized);
                Vector3 lorigin = GetOrigin(temp.leftGazeOriginMM * 0.001f);
                lorigin.x *= -1;
                temp.leftEyePrecision = GetPrecision(lorigin, ldirection);
                lastValidTest.leftEyePrecision= temp.leftEyePrecision;
            }
            else
            {
                temp.leftEyePrecision = lastValidTest.leftEyePrecision;
            }
            if (rvalid)
            {
                //right eye precision:
                Vector3 rdirection = GetDirection(temp.rightGazeDirectionNormalized);
                Vector3 rorigin = GetOrigin(temp.rightGazeOriginMM * 0.001f);
                rorigin.x *= -1;
                temp.rightEyePrecision = GetPrecision(rorigin, rdirection);
                lastValidTest.rightEyePrecision = temp.rightEyePrecision;
            }
            else
            {
                temp.rightEyePrecision = lastValidTest.rightEyePrecision;
            }
            if (cvalid)
            {
                //combined precision
                Vector3 cdirection = GetDirection(focusManager.GetVerboseData().combined.eye_data.gaze_direction_normalized);
                Vector3 corigin = GetOrigin(focusManager.GetVerboseData().combined.eye_data.gaze_origin_mm * 0.001f);
                temp.combinedPrecision = GetPrecision(corigin, cdirection);
                lastValidTest.combinedPrecision = temp.combinedPrecision;
            }
            else
            {
                temp.combinedPrecision = lastValidTest.combinedPrecision;
            }
            temp.speed = currentData.speed;
            temp.frequency = currentData.frequency;
        }
    }

    float GetInvalidPlaceholder()
    {
        return 9;
    }

    float GetPrecision(Vector3 origin, Vector3 direction)
    {
        float precision = Vector3.Angle(targetObj.transform.position - origin, direction) / 180;
        precision -= 0.5f;
        precision *= -2f;
        return precision;
    }

    Vector3 GetDirection(Vector3 direction)
    {
        return Camera.main.transform.TransformDirection(direction);
    }

    Vector3 GetOrigin(Vector3 origin)
    {
        return Camera.main.transform.TransformPoint(origin);
    }

    void Save()
    {
        string path = "PatientResults\\" + PlayerPrefs.GetString("Name") + "\\Test Results " + System.DateTime.Today.ToString("MM-dd-yyyy");
        string filename = path + "\\Test" + currentTest + ".csv";
        bool dirExists = Directory.Exists(path);
        if (!dirExists)
            Directory.CreateDirectory(path);

        using (var stream = File.Open(filename, FileMode.Create))
        {
            var wtr = new StreamWriter(stream, System.Text.Encoding.UTF8);
            //write first line
            wtr.WriteLine("Left Gaze Direction Normalized" + "," +
                          "Right Gaze Direction Normalized" + "," +
                          "Left Gaze Origin MM" + "," +
                          "Right Gaze Origin MM" + "," +
                          TestCSV());
            for (int i = 0; i < testData.Count; i++)
            {
                string line = ToCSVFriendlyString(testData[i].leftGazeDirectionNormalized) + "," +
                              ToCSVFriendlyString(testData[i].rightGazeDirectionNormalized) + "," +
                              ToCSVFriendlyString(testData[i].leftGazeOriginMM) + "," +
                              ToCSVFriendlyString(testData[i].rightGazeOriginMM) + ",";
                line += GetTestDataString(testData[i]);
                wtr.WriteLine(line);
            }
            wtr.Close();
        }
        saved = true;
    }

    string GetTestDataString(TestData testData)
    {
        string testDataString = "";
        if (currentTest == 1)
        {
            testDataString += testData.latency;
        }
        else if (currentTest == 2)
        {
            testDataString += testData.leftEyePrecision
                            + ","
                            + testData.rightEyePrecision
                            + ","
                            + testData.combinedPrecision;
        }
        else if (currentTest == 3)
        {
            if(float.IsInfinity(testData.frequency)) { testData.frequency = 0;  }
            testDataString += testData.leftEyePrecision
                            + ","
                            + testData.rightEyePrecision
                            + ","
                            + testData.combinedPrecision
                            + ","
                            + testData.speed
                            + ","
                            + testData.frequency;
        }
        return testDataString;
    }

    string TestCSV()
    {
        string testCSV = "";
        if(currentTest == 1)
        {
            testCSV = "Latency";
        }
        else if(currentTest == 2)
        {
            testCSV = "Left Eye Precision (Test 2)" + "," +
                      "Right Eye Precision (Test 2)" + "," +
                      "Combined Eye Precision (Test 2)";
        }
        else if(currentTest == 3)
        {
            testCSV = "Left Eye Precision (Test 3)" + "," +
                      "Right Eye Precision (Test 3)" + "," +
                      "Combined Eye Precision (Test 3)" + "," +
                      "Speed" + "," +
                      "Frequency";
        }
        return testCSV;
    }

    string ToCSVFriendlyString(Vector3 vector)
    {
        string output = "(" + vector.x + " ; " + vector.y + " ; " + vector.z + ")";
        return output;
    }

    void NextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index != 3)
        {
            SceneManager.LoadScene(5);
        }
        else
        {
            SceneManager.LoadScene(4);
        }
    }
}

public class FrequencyTracker
{
    int frequency;
    float leftAngle;
    float rightAngle;
    bool turningLeft;
    bool turningRight;
    bool testBeginning = true;
    Transform headsetT;

    public FrequencyTracker(float leftAngle, float rightAngle, Transform headsetT)
    {
        this.leftAngle = leftAngle / 180;
        this.rightAngle = rightAngle / 180;
        this.headsetT = headsetT;
        turningLeft = false;
        turningRight = false;
    }

    public void checkRotation()
    {
        float rotation = headsetT.rotation.y;
        //Debug.Log("Rotation is" + rotation);
        if (testBeginning)
        {
            if (rotation <= leftAngle)
            {
                turningRight = true;
                incrementFrequency();
            }
            else if (rotation >= rightAngle)
            {
                turningLeft = true;
                incrementFrequency();
            }
            testBeginning = false;
        }
        else if (turningLeft)
        {
            if (rotation <= leftAngle)
            {
                turningRight = true;
                turningLeft = false;
                incrementFrequency();
            }
        }
        else if (turningRight)
        {
            if (rotation >= rightAngle)
            {
                turningLeft = true;
                turningRight = false;
                incrementFrequency();
            }
        }
    }

    void incrementFrequency()
    {
        frequency++;
    }

    public int getFrequency()
    {
        return frequency;
    }
}
