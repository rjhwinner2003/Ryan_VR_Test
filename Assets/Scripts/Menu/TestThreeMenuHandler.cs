using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestThreeMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField durationInput;
    [SerializeField] TextMeshProUGUI errorText;

    private void Start()
    {
        SetDuration();
    }

    public void SetDuration()
    {
        if (float.TryParse(durationInput.text, out float num))
        {
            PlayerPrefs.SetFloat("TestThreeDuration", num);
            errorText.text = "";
        }
        else
        {
            errorText.text = "Test Three Duration Must Be A Number";
        }
    }
}
