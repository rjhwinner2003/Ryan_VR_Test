using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Valve.Newtonsoft.Json.Bson;

public class ActiveTestHandler : MonoBehaviour
{
    [SerializeField] Toggle TestOne;
    [SerializeField] Toggle TestTwo;
    [SerializeField] Toggle TestThree;
    bool testOneActive = true;
    bool testTwoActive = true;
    bool testThreeActive = true;

    private void Start()
    {
        PlayerPrefs.SetInt("TestOne", 1);
        PlayerPrefs.SetInt("TestTwo", 1);
        PlayerPrefs.SetInt("TestThree", 1);
    }

    public void testOneButton()
    {
        if(TestOne.isOn)
            PlayerPrefs.SetInt("TestOne", 1);
        else
            PlayerPrefs.SetInt("TestOne", 0);
    }
    public void testTwoButton()
    {
        if (TestTwo.isOn)
            PlayerPrefs.SetInt("TestTwo", 1);
        else
            PlayerPrefs.SetInt("TestTwo", 0);
    }
    public void testThreeButton()
    {
        if (TestThree.isOn)
            PlayerPrefs.SetInt("TestThree", 1);
        else
            PlayerPrefs.SetInt("TestThree", 0);
    }
}
