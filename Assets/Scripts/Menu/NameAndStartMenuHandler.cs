using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class NameAndStartMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField newName;
    [SerializeField] TMP_Dropdown patients;
    [SerializeField] TextMeshProUGUI errorText;

    public void OnNewName()
    {
        bool isFileName = !string.IsNullOrEmpty(newName.text) &&
                          newName.text.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        bool fileAlreadyExists = Directory.Exists("PatientResults\\" + newName.text);
        if (!isFileName)
        {
            errorText.text = "Patient Name Must Not Contain < > : \" / \\ | ? *";
        }
        else if(fileAlreadyExists)
        {
            errorText.text = "This patient name already exists";
        }
        else
        {
            //Create directory for new patient
            Directory.CreateDirectory("PatientResults\\" + newName.text);
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            newOption.text = newName.text;
            patients.AddOptions(new List<TMP_Dropdown.OptionData> { newOption });
        }
    }

    public void OnStart()
    {
        SceneManager.LoadScene(5);
    }

    public void OnPatientSelect()
    {
        PlayerPrefs.SetString("Name", patients.options[patients.value].text);
    }
}
