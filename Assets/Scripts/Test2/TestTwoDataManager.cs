using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViveSR.anipal.Eye;

public class TestTwoDataManager : MonoBehaviour
{
    [SerializeField] TestTwoManager testTwoManager;
    [SerializeField] GameObject headTracker;
    [SerializeField] FocusManager focusManager;
    float currentTime = 0;
    bool saved = false;

    [System.Serializable]
    public class TestData
    {
        public Vector3 leftGazeDirectionNormalized;
        public Vector3 rightGazeDirectionNormalized;
        public Vector3 leftGazeOriginMM;
        public Vector3 rightGazeOriginMM;
        public float leftEyePrecision;
        public float rightEyePrecision;
    }
    List<TestData> testData = new List<TestData>();
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        TestData temp = new TestData();
        VerboseData currentData = new VerboseData();
        currentData = focusManager.GetVerboseData();
        temp.leftGazeDirectionNormalized = currentData.left.gaze_direction_normalized;
        temp.rightGazeDirectionNormalized = currentData.right.gaze_direction_normalized;
        temp.leftGazeOriginMM = currentData.left.gaze_origin_mm;
        temp.rightGazeOriginMM = currentData.right.gaze_origin_mm;
        float leftAngle = Vector3.Angle(temp.leftGazeDirectionNormalized, testTwoManager.GetTargetVector());
        float rightAngle = Vector3.Angle(temp.rightGazeDirectionNormalized, testTwoManager.GetTargetVector());
        Debug.Log("Left Angle is" + leftAngle);
        leftAngle /= 180;
        rightAngle /= 180;
        temp.leftEyePrecision = 1 - leftAngle;
        temp.rightEyePrecision = 1 - rightAngle;
        testData.Add(temp);
        currentTime += Time.deltaTime;
        if (testTwoManager.GetTestOver() && !saved)
            Save();
        if (saved)
            NextScene();
    }

    void NextScene()
    {
        if (PlayerPrefs.GetInt("TestThree") == 1)
            SceneManager.LoadScene(3);
        else
            Application.Quit();
    }

    void Save()
    {
        string path = "PatientResults\\" + PlayerPrefs.GetString("Name") + "\\Test Results " + System.DateTime.Today.ToString("MM-dd-yyyy");
        string filename = path + "\\Test 2.csv";
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
                          "Left Eye Precision" + "," +
                          "Right Eye Precision");
            for (int i = 0; i < testData.Count; i++)
            {
                string line = ToCSVFriendlyString(testData[i].leftGazeDirectionNormalized) + "," +
                              ToCSVFriendlyString(testData[i].rightGazeDirectionNormalized) + "," +
                              ToCSVFriendlyString(testData[i].leftGazeOriginMM) + "," +
                              ToCSVFriendlyString(testData[i].rightGazeOriginMM) + "," +
                              testData[i].leftEyePrecision + "," +
                              testData[i].rightEyePrecision;
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
