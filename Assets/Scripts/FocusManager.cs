using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;

namespace ViveSR.anipal.Eye
{
    public class FocusManager : MonoBehaviour
    {
        private static bool eye_callback_registered = false;
        private FocusInfo FocusInfo;
        private static FocusInfo focusInfo;
        private static EyeData eyeData = new EyeData();
        private readonly float MaxDistance = 20;
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
        private static Ray testRay;
        private VerboseData currentData;
        private float latency = 0.1f;
        private float lastUpdateTime = 0;
        private float currentTime;
        public TextMeshProUGUI eyeDegreeText;

        Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal = Vector3.zero;
        Vector3 GazeOriginLeftLocal, GazeDirectionLeftLocal = Vector3.zero;
        Vector3 GazeOriginRightLocal, GazeDirectionRightLocal = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            //eyeDegreeText.gameObject.SetActive(true);
            
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false;
                return;
            }
            ViveSR.anipal.Eye.SRanipal_Eye.GetVerboseData(out currentData, eyeData);
        }

        // Update is called once per frame
        void Update()
        {
            HandleEyeCallback();
            currentTime += Time.deltaTime;
            if(currentTime - lastUpdateTime > latency)
            {
                ViveSR.anipal.Eye.SRanipal_Eye.GetVerboseData(out currentData, eyeData); //Update data
                lastUpdateTime = currentTime;
            }


            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay;
                bool eye_focus;
                if (eye_callback_registered)
                    eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, eyeData);
                else
                    eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance);

                //Debug.Log(GazeRay);
            }

            GazeOriginCombinedLocal = Vector3.zero;
            GazeDirectionCombinedLocal = Vector3.zero;
            GazeOriginLeftLocal = Vector3.zero;
            GazeDirectionLeftLocal = Vector3.zero;
            GazeOriginRightLocal = Vector3.zero;
            GazeDirectionRightLocal = Vector3.zero;
            if (eye_callback_registered == true)
            {
                SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData);
                SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out GazeOriginLeftLocal, out GazeDirectionLeftLocal, eyeData);
                SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOriginRightLocal, out GazeDirectionCombinedLocal, eyeData);
            }
            else
            {
                SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal);
                SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out GazeOriginLeftLocal, out GazeDirectionCombinedLocal);
                SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOriginRightLocal, out GazeDirectionRightLocal);
            }

            //eyeDegreeText.text = GazeOriginCombinedLocal.ToString();
            //gaze_direction_normalized = 0;
        }

        public (Vector3,Vector3) GetLeftVectors()
        {
            return (GazeOriginLeftLocal, GazeDirectionLeftLocal);
        }

        public (Vector3, Vector3) GetRightVectors()
        {
            return (GazeOriginRightLocal, GazeDirectionRightLocal);
        }

        public (Vector3, Vector3) GetCombinedVectors()
        {
            return (GazeOriginCombinedLocal, GazeDirectionCombinedLocal);
        }

        public Vector3 GetRay(GazeIndex index)
        {
            SRanipal_Eye.Focus(index, out testRay, out focusInfo);
            Debug.DrawRay(testRay.origin, testRay.direction * 1000, Color.red, 10);
            return eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;
        }

        public VerboseData GetVerboseData()
        {
            return currentData;
        }

        void HandleEyeCallback()
        {
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

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
        }

        public static GameObject Focus()
        {
            if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out testRay, out focusInfo)) { }
            else if (SRanipal_Eye.Focus(GazeIndex.LEFT, out testRay, out focusInfo)) { }
            if (SRanipal_Eye.Focus(GazeIndex.RIGHT, out testRay, out focusInfo)) { }
            else return null;
            return focusInfo.collider.gameObject;
        }

        public static string FocusName()
        {
            GameObject focusCheck = Focus();
            if (focusCheck is null)
                return "";
            else
                return focusCheck.name;
        }

        private void OnDisable()
        {
            Release();
        }

        void OnApplicationQuit()
        {
            Release();
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
        }

        public bool GetFocus()
        {
            return (FocusName() == "FocusObject");
        }
    }
}
