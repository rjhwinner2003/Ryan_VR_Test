using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System.Linq;

/*
* This manager loads all patients into the dropdown. The patient filename itself is loaded 
* elsewhere, in NameAndStartMenuHandler.cs
*/
public class DropdownManager : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown dropDown;
    // Start is called before the first frame update
    void Start()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        string path = "PatientResults\\";
        DirectoryInfo dir = new DirectoryInfo(path);
        DirectoryInfo[] info = dir.GetDirectories("*.*");
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (DirectoryInfo d in info)
        {
            TMP_Dropdown.OptionData item = new TMP_Dropdown.OptionData(d.Name);
            options.Add(item);
        }
        dropDown.AddOptions(options);
    }
}
