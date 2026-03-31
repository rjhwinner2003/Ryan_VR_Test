using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestOneText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI testText;
    [SerializeField] private GameObject testingManager;
    private float currentTime;
    private float endTime = 3;
    private float halfTime;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("SecondHalf", 0);
        testText.text = "Focus on the red circle";
        currentTime = 0;
        halfTime = 30 / 2; //Initialize to testDuration/2 in playerprefs
        //testingManager.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        if (PlayerPrefs.GetInt("SecondHalf") == 1)
        {
            testText.text = "Focus opposite the red circle";
            PlayerPrefs.SetInt("SecondHalf", 0);
            endTime = currentTime + 3;
        }
        else if (currentTime > (endTime))
        {
            testText.text = "";
            //testingManager.SetActive(true);
        }
    }
}
