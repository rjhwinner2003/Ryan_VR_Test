using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestOneMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField degreeInput;
    [SerializeField] TMP_InputField iterationInput;
    [SerializeField] TextMeshProUGUI errorText;

    private void Start()
    {
        SetDegrees();
        SetIterations();
    }

    public void SetDegrees()
    {
        if(float.TryParse(degreeInput.text, out float num))
        {
            PlayerPrefs.SetFloat("TestOneDegreesFromCenter", num);
            errorText.text = "";
        }
        else
        {
            errorText.text = "Test One Degree Value Must Be A Number";
        }
    }

    public void SetIterations()
    {
        if (int.TryParse(iterationInput.text, out int num))
        {
            Debug.Log("Setting iterations" + num);
            PlayerPrefs.SetInt("TestOneIterationsPerHalf", num);
            errorText.text = "";
        }
        else
        {
            errorText.text = "Test One Iterations Must Be A Number";
        }
    }
}
