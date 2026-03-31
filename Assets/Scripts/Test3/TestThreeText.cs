using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestThreeText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI testText;
    private string text = "Focus on the red circle while moving your lead left and right";
    private float currentTime;
    private float endTime = 3;
    // Start is called before the first frame update
    void Start()
    {
        testText.text = text;
        currentTime = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log("time" + currentTime);
        currentTime += Time.deltaTime;
        if (currentTime > endTime)
        {
            testText.text = "";
        }
    }
}
