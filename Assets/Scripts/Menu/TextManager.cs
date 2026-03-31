using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class TextManager : MonoBehaviour
{
    public TextMeshProUGUI testOneText;
    public TextMeshProUGUI testTwoText;
    public TextMeshProUGUI testThreeText;
    public TextMeshProUGUI timer;

    


    float timerStart;
    float timerEnd;

    // Start is called before the first frame update
    void Start()
    {
       

        if (PlayerPrefs.GetInt("TestOne") == 1)
        {
            testOneText.gameObject.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("TestTwo") == 1)
        {
            testTwoText.gameObject.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("TestThree") == 1)
        {
            testThreeText.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            timerStart = Time.time;
            timerEnd = timerStart + 3;
            timer.gameObject.SetActive(true);
        }
        if(timer.gameObject.activeSelf)
        {
            int timeLeft = (int)(timerEnd - Time.time);
            timer.text = "Test will begin in " + timeLeft;
            if (timeLeft <= 0)
            {
                NextScene();
            }
        }
    }

    void NextScene()
    {
        if (testOneText.gameObject.activeSelf)
        {
            PlayerPrefs.SetInt("TestOne", 0);
            SceneManager.LoadScene(1);
        }
        if (testTwoText.gameObject.activeSelf)
        {
            PlayerPrefs.SetInt("TestTwo", 0);
            SceneManager.LoadScene(2);
        }
        if (testThreeText.gameObject.activeSelf)
        {
            PlayerPrefs.SetInt("TestThree", 0);
            SceneManager.LoadScene(3);
        }
    }
}
