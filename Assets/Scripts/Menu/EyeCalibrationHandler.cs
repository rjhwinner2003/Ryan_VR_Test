using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR.anipal.Eye
{
    public class EyeCalibrationHandler : MonoBehaviour
    {
        public void RunCalibration()
        {
            SRanipal_Eye.LaunchEyeCalibration();
        }
    }
}
