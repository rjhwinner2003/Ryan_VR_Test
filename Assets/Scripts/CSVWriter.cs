using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Diagnostics;


public class CSVWriter : MonoBehaviour
{
    TextWriter tw;


    [SerializeField] TextMeshProUGUI leftText;
    [SerializeField] TextMeshProUGUI rightText;

   

    

    [System.Serializable]
    public class ExportedData
    {
        public ExportedData(string sec, string velLeft, string velRight, string pitLeft, string pitRight)
        {
            second = sec;
            eyeVelocityLeft = velLeft;
            eyeVelocityRight = velRight;
            eyePitchLeft = pitLeft;
            eyePitchRight = pitRight;
        }

        public string second,
            eyeVelocityLeft,
            eyeVelocityRight,
            eyePitchLeft,
            eyePitchRight;
    }
   
    [System.Serializable]

    public class DataList
    {
        public List<ExportedData> data = new List<ExportedData>();
        
    }




    // Start is called before the first frame update
    void Start()
    {
        leftText.text = "0";
        rightText.text = "0";
       
    }

    // Update is called once per frame
    void Update()
    {
          //nothing to see here... anymore lol
    }

    

    public void WriteCSV(string filename, DataList dL)
    {
        tw = new StreamWriter(filename, false);
        tw.WriteLine("Time" + "," + "Left Eye Yaw" + "," + "Right Eye Yaw" + "," + "Left Pitch" + "," + "Right Pitch");

        tw.Close();

        tw = new StreamWriter(filename, true);
        
        for(int i = 0; i < dL.data.Count; i++)
        {
            tw.WriteLine(dL.data[i].second + ", " + dL.data[i].eyeVelocityLeft + ", " + dL.data[i].eyeVelocityRight + "," + dL.data[i].eyePitchLeft + "," + dL.data[i].eyePitchRight);
        }

        tw.Close();
    }



}
