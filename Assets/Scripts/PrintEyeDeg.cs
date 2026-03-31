using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ViveSR.anipal.Eye;
using static UnityEngine.UI.Image;
using System.Diagnostics;

/// <summary>
/// Example usage for eye tracking callback
/// Note: Callback runs on a separate thread to report at ~120hz.
/// Unity is not threadsafe and cannot call any UnityEngine api from within callback thread.
/// </summary>
public class PrintEyeDeg : MonoBehaviour
{
    private static EyeData eyeData = new EyeData();
    private static bool eye_callback_registered = false;

    public TextMeshProUGUI output;  //gaze angles between the two eye directions
    public TextMeshProUGUI routput; //right eye output
    public TextMeshProUGUI loutput; //left eye output

    public static string updateString;
    public static string rOutputString;
    public static string lOutputString;

    private static float updateSpeed = 0;
    private static float lastTime, currentTime;

    static CSVWriter.ExportedData ed;

    private static float timer = 0, prevTime = 0;

    private static int frequency = 50;
    private static int totalEntries = 0;

    private float totalTime;

    private static CSVWriter.DataList myDataList = new CSVWriter.DataList();

    CSVWriter writer = new CSVWriter();

    void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

        timer += Time.deltaTime;

        if((int)timer != (int)prevTime)
        {
            //casting to an int. Will be true after every second.
            totalEntries = 0;
        }

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }

        routput.text = rOutputString;
        loutput.text = lOutputString;
        output.text = updateString;

        prevTime = timer;
    }

    private void OnDisable()
    {
        Release();
    }

    private void OnApplicationQuit()
    {
        writer.WriteCSV(Application.dataPath + "/2025Experiment/test" + frequency.ToString() + "hz.csv", myDataList);

        // StopRecording();
    }

    /// <summary>
    /// Release callback thread when disabled or quit
    /// </summary>
    private static void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    /// <summary>
    /// Required class for IL2CPP scripting backend support
    /// </summary>
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    /// <summary>
    /// Eye tracking data callback thread.
    /// Reports data at ~120hz
    /// MonoPInvokeCallback attribute required for IL2CPP scripting backend
    /// </summary>
    /// <param name="eye_data">Reference to latest eye_data</param>
    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData eye_data)
    {        
        eyeData = eye_data;
        // do stuff with eyeData...

        lastTime = currentTime;
        currentTime = eyeData.frame_sequence;

        //old calculations that will not work in theads
        //unity functions cannot be called in threads for some reason.
        //Vector3 lorigin = Camera.main.transform.TransformPoint(eye_data.verbose_data.left.gaze_origin_mm);
        //Vector3 ldirection = Camera.main.transform.TransformDirection(eye_data.verbose_data.left.gaze_direction_normalized);
        //
        ////right eye
        //Vector3 rorigin = Camera.main.transform.TransformPoint(eye_data.verbose_data.right.gaze_origin_mm);
        //Vector3 rdirection = Camera.main.transform.TransformDirection(eye_data.verbose_data.right.gaze_direction_normalized);

        //angle calculations done without the use of Unity-created functions.
        float lAngle = Vector3.Angle(eye_data.verbose_data.left.gaze_origin_mm, eye_data.verbose_data.left.gaze_direction_normalized);
        float rAngle = Vector3.Angle(eye_data.verbose_data.right.gaze_origin_mm, eye_data.verbose_data.right.gaze_direction_normalized);

        float lPitch = (float)ConvertRadiansToDegrees(Math.Tan((eye_data.verbose_data.left.gaze_direction_normalized.y) / eye_data.verbose_data.left.gaze_direction_normalized.z));
        float rPitch = (float)ConvertRadiansToDegrees(Math.Tan((eye_data.verbose_data.right.gaze_direction_normalized.y) / eye_data.verbose_data.right.gaze_direction_normalized.z));

        ed = new CSVWriter.ExportedData(timer.ToString(), (lAngle - 145).ToString(), (rAngle - 145).ToString(), lPitch.ToString(), rPitch.ToString());

        UnityEngine.Debug.Log("Left angle: " + (lAngle - 145).ToString() + " | " + "Right angle: " + (rAngle - 145).ToString());

        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }

        rOutputString = "right " + (rAngle - 145).ToString();
        lOutputString = "left " + (lAngle - 145).ToString();

        /*
        //EXPERIMENTAL! FOR FREQUENCIES GREATER THAN 120 HZ! MAKE SURE TO TAKE BACK OUT LATER!
        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }

        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }
        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }

        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }

        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }

        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }

        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }

        if (totalEntries < frequency)
        {
            myDataList.data.Add(ed);
            totalEntries++;
        }
        */
        //totalEntries++;
        updateSpeed = currentTime - lastTime;

        updateString = timer.ToString();
    }

    public static double ConvertRadiansToDegrees(double radians)
    {
        double degrees = (180 / Math.PI) * radians;
        return (degrees);
    }

}