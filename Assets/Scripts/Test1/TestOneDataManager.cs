using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViveSR.anipal.Eye;

public class TestOneDataManager : MonoBehaviour
{
    [SerializeField] TestOneMovement testOneMovement;
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
        public float latency;
    }
    List<TestData> testData = new List<TestData>();
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CollectData();
    }

    void CollectData()
    {
        TestData temp = new TestData();
        VerboseData thisVerboseData = focusManager.GetVerboseData();
        temp.leftGazeDirectionNormalized = thisVerboseData.left.gaze_direction_normalized;
        temp.rightGazeDirectionNormalized = thisVerboseData.right.gaze_direction_normalized;
        temp.leftGazeOriginMM = thisVerboseData.left.gaze_origin_mm;
        temp.rightGazeOriginMM = thisVerboseData.right.gaze_origin_mm;
        //temp.latency = testOneMovement.GetLatency();
        testData.Add(temp);
        currentTime += Time.deltaTime;
        if (testOneMovement.GetTestOver() && !saved)
            Save();
        if (saved)
            NextScene();
    }

    void NextScene()
    {
        if (PlayerPrefs.GetInt("TestTwo") == 1)
            SceneManager.LoadScene(2);
        else if (PlayerPrefs.GetInt("TestThree") == 1)
            SceneManager.LoadScene(3);
        else
            Application.Quit();
    }

    void Save()
    {
        string path = "PatientResults\\" + PlayerPrefs.GetString("Name") + "\\Test Results " + System.DateTime.Today.ToString("MM-dd-yyyy");
        string filename = path + "\\Test 1.csv";
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
                          "Latency (Time in seconds from object appearing till object in focus)");
            for(int i = 0; i < testData.Count; i++)
            {
                string line = ToCSVFriendlyString(testData[i].leftGazeDirectionNormalized) + "," +
                              ToCSVFriendlyString(testData[i].rightGazeDirectionNormalized) + "," +
                              ToCSVFriendlyString(testData[i].leftGazeOriginMM) + "," +
                              ToCSVFriendlyString(testData[i].rightGazeOriginMM) + "," +
                              testData[i].latency;
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
