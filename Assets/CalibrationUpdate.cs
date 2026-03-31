using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.localPosition = GameObject.Find("HoldFocusObject").transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
