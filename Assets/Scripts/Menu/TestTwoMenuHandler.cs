using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestTwoMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField degreeInput;
    [SerializeField] TMP_InputField durationInput;
    [SerializeField] TextMeshProUGUI errorText;

    private void Start()
    {
        SetDegrees();
        SetDuration();
    }

    public void SetDegrees()
    {
        if (float.TryParse(degreeInput.text, out float num))
        {
            PlayerPrefs.SetFloat("TestTwoDegreesFromCenter", num);
            errorText.text = "";
        }
        else
        {
            errorText.text = "Test Two Degree Value Must Be A Number";
        }
    }

    public void SetDuration()
    {
        if (float.TryParse(durationInput.text, out float num))
        {
            PlayerPrefs.SetFloat("TestTwoDuration", num);
            errorText.text = "";
        }
        else
        {
            errorText.text = "Test Two Duration Must Be A Number";
        }
    }
}
