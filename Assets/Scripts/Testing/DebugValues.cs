using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ViveSR.anipal.Eye;
using static UnityEngine.UI.Image;

public class DebugValues : MonoBehaviour
{
    [SerializeField] FocusManager focusManager;
    [SerializeField] GameObject test;
    [SerializeField] GameObject headset;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float speed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        VerboseData data = focusManager.GetVerboseData();

        if (Input.GetKey(KeyCode.W))
        {
            test.transform.position = test.transform.position + (headset.transform.forward) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            test.transform.position = test.transform.position + (headset.transform.forward * -1) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            test.transform.position = test.transform.position + (headset.transform.right * -1) * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.D))
        {
            test.transform.position = test.transform.position + headset.transform.right * speed * Time.deltaTime;
        }
        //Validity
        bool lvalid = data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
        bool rvalid = data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
        bool cvalid = data.combined.eye_data.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY);
        //left eye
        Vector3 lorigin = Camera.main.transform.TransformPoint(data.left.gaze_origin_mm * 0.001f);
        Vector3 ldirection = Camera.main.transform.TransformDirection(data.left.gaze_direction_normalized);
        //right eye
        Vector3 rorigin = Camera.main.transform.TransformPoint(data.right.gaze_origin_mm);
        Vector3 rdirection = Camera.main.transform.TransformDirection(data.right.gaze_direction_normalized);
        //center
        Vector3 corigin = Camera.main.transform.TransformPoint(data.combined.eye_data.gaze_origin_mm * 0.001f);
        Vector3 cdirection = Camera.main.transform.TransformDirection(data.combined.eye_data.gaze_direction_normalized);

        /*
        if (Vector3.Angle(headset.transform.forward - lorigin, ldirection-lorigin) == 0)
        {
            Debug.Log(test.transform.position);
        }
        if (Vector3.Angle(headset.transform.forward - rorigin, rdirection - rorigin) == 0)
        {
            Debug.Log(test.transform.position);
        }*/

        //Debug.Log(data.left.gaze_origin_mm + )
        if (Input.GetKey(KeyCode.Space))
        {
            if (lvalid)
            {
                float precision = Vector3.Angle(test.transform.position - lorigin, ldirection) / 180;
                precision -= 0.5f;
                precision *= -2f;
                text.text = precision.ToString();
            }
            if(rvalid)
                Debug.DrawRay(rorigin, rdirection, Color.blue, 15f);
            if(cvalid)
                Debug.DrawRay(corigin, cdirection, Color.green, 15f);

            //Debug.DrawRay(rorigin + headset.transform.position, headset.transform.forward, Color.blue, 15f);
            /*
            percent += Time.deltaTime * 0.1f;
            //test.transform.position = Vector3.Lerp(headset.transform.position, headset.transform.forward * 10, percent);
            Vector3 target = headset.transform.TransformDirection(focusManager.GetLeftVectors().Item2);
            Vector3 origin = (focusManager.GetVerboseData().left.gaze_origin_mm / 1000) + headset.transform.position;
            Vector3 rtarget = headset.transform.TransformDirection(focusManager.GetCombinedVectors().Item2);
            Vector3 rorigin = (focusManager.GetVerboseData().combined.eye_data.gaze_origin_mm / 1000) + headset.transform.position;
            Vector3 rrorigin = (focusManager.GetVerboseData().right.gaze_origin_mm / 1000) + headset.transform.position;
            Debug.DrawRay(origin, target, Color.red, 10);
            Debug.DrawRay(rorigin, rtarget, Color.green, 10);
            Debug.DrawRay(rrorigin, rtarget, Color.blue, 10);
            //Debug.DrawRay((focusManager.GetVerboseData().combined.eye_data.gaze_origin_mm / 1000f) + headset.transform.position, target, Color.red, 10);
            float precision = Vector3.Angle(test.transform.position - origin, target - origin);
            //precision /= 180;
            text.text = precision.ToString();
            Debug.Log(rtarget);
            Debug.Log(target);
            //Debug.Log(gazeRay);

            /*
            Debug.Log("System origin: " + headset.transform.position);
            Debug.Log("Left gaze origin is: " + (frame.x + (VB.left.gaze_origin_mm.x / 1000)) + "," + (frame.y + (VB.left.gaze_origin_mm.y / 1000)) + "," + (frame.z + (VB.left.gaze_origin_mm.z / 1000)));
            Debug.Log("Right gaze origin is: " + (frame.x - (VB.right.gaze_origin_mm.x / 1000)) + "," + (frame.y - (VB.right.gaze_origin_mm.y / 1000)) + "," + (frame.z - (VB.right.gaze_origin_mm.z / 1000)));

            /*
        Debug.Log("Left origin: " + (frame.x+(VB.left.gaze_origin_mm.x/1000)) + "," + (frame.y+(VB.left.gaze_origin_mm.y/1000)) + "," + (frame.z+(VB.left.gaze_origin_mm.z/1000)));
        Debug.Log("Left origin: " + (frame.x - (VB.right.gaze_origin_mm.x / 1000)) + "," + (frame.y - (VB.right.gaze_origin_mm.y / 1000)) + "," + (frame.z - (VB.right.gaze_origin_mm.z / 1000)));
        Debug.Log("TestObj pos: " + test.transform.localPosition);
            */
        }
        if(Input.GetKey(KeyCode.R))
        {
            test.transform.position = headset.transform.position;
        }
    }
}
