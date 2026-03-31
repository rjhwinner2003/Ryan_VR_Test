using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using ViveSR.anipal.Eye;

public class TestOneMovement : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager = new SaveManager();

    [SerializeField] private GameObject leftFocusObject;
    [SerializeField] private GameObject rightFocusObject;
    [SerializeField] private GameObject leftAngleObject; //Object used to record left angle. Only used in Start()
    [SerializeField] private GameObject rightAngleObject; //Object used to record right angle. Only used in Start()
    [SerializeField] private GameObject centerFocusObject;
    [SerializeField] private GameObject leftFocusHand; 
    [SerializeField] private GameObject rightFocusHand; 
    [SerializeField] private Material inactiveMaterial; 
    [SerializeField] private Material activeMaterial; 
    private float angleToMove;
    private float maxIterations;
    private float currentIteration = 0;
    private bool secondHalf = false;
    private bool inCooldown = false;
    private float cooldownTime = 1;
    private int leftIterations = 0;
    private int rightIterations = 0;
    private int maxLeftIterations = 0;
    private int maxRightIterations = 0;
    private float currentTime = 0;
    private float lastTime = 0;

    float latency = 0.0f;
    bool testOver = false;
    bool runningCooldown = false;

    public bool GetSecondHalf() { return secondHalf; }
    // Start is called before the first frame update
    void Start()
    {
        angleToMove = PlayerPrefs.GetFloat("TestOneDegreesFromCenter");
        maxIterations = PlayerPrefs.GetInt("TestOneIterationsPerHalf");
        maxLeftIterations = (int)(maxIterations / 2);
        maxRightIterations = (int)(maxIterations / 2);
        maxRightIterations += (int)maxIterations % 2;
        latency = 0;
        InitializeAngles();
    }

    // Update is called once per frame
    void Update()
    {
        HandleTesting();
        SetLatency();
        if (inCooldown && FocusCenter() && !runningCooldown)
        {
            Invoke("Cooldown", Random.Range(2, 5));
            runningCooldown = true;
        }
        currentTime += Time.deltaTime;
      
        SetHandColor();
    }

    void SetHandColor()
    {
        if(inCooldown)
        {
            rightFocusHand.GetComponent<SkinnedMeshRenderer>().material = inactiveMaterial;
            leftFocusHand.GetComponent<SkinnedMeshRenderer>().material = inactiveMaterial;
        }
        else if (FindVisibleFocusObject().name == "RightFocusObject")
        {
            rightFocusHand.GetComponent<SkinnedMeshRenderer>().material = activeMaterial;
            leftFocusHand.GetComponent<SkinnedMeshRenderer>().material = inactiveMaterial;
        }
        else if(FindVisibleFocusObject().name == "LeftFocusObject")
        {
            rightFocusHand.GetComponent<SkinnedMeshRenderer>().material = inactiveMaterial;
            leftFocusHand.GetComponent<SkinnedMeshRenderer>().material = activeMaterial;
        }
    }

    void SetLatency()
    {
        //this function throws an error for some reason
        //TODO: Figure out what data is changed by this.
        //saveManager.currentData.latency = latency;
    }

    public bool GetTestOver()
    {
        return testOver;
    }

    void HandleTesting()
    {
        if(currentIteration > maxIterations && !secondHalf)
        {
            secondHalf = true;
            PlayerPrefs.SetInt("SecondHalf", 1);
            currentIteration = 0;
            leftIterations = 0;
            rightIterations = 0;
        }
        else if(currentIteration > maxIterations && secondHalf)
        {
            testOver = true;
        }

        if (!secondHalf) 
        {
            if (FocusManager.FocusName() == FindVisibleFocusObject().name)
            { 
                MoveFocusObjects();
            }
        }
        else if (secondHalf)
        {
            if (FocusManager.FocusName() == FindOppositeFocusObject().name)
            {
                MoveFocusObjects();
            }
        }
    }

    public GameObject FindOppositeFocusObject()
    {
        if (leftFocusObject.GetComponent<MeshRenderer>().enabled == false)
            return leftFocusObject;
        else if (rightFocusObject.GetComponent<MeshRenderer>().enabled == false)
            return rightFocusObject;
        return centerFocusObject;
    }

    public GameObject FindVisibleFocusObject()
    {
        if (leftFocusObject.GetComponent<MeshRenderer>().enabled == true)
            return leftFocusObject;
        else if (rightFocusObject.GetComponent<MeshRenderer>().enabled == true)
            return rightFocusObject;
        return centerFocusObject;
    }

    public GameObject FindCenterFocusObject()
    {
        return centerFocusObject;
    }

    void InitializeAngles()
    {
        leftAngleObject.transform.Rotate(Vector3.up, -angleToMove);
        rightAngleObject.transform.Rotate(Vector3.up, angleToMove);
        if(Random.Range(0,2) == 0)
        {
            leftFocusObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            rightFocusObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    /*
     * Toggles active focus object
     */
    void MoveFocusObjects()
    {
        if (!inCooldown)
        {
            latency = currentTime - lastTime;
            currentIteration++;
            if (Random.Range(0, 2) == 0)
            {
                Swap();
            }
            else
            {
                if (leftFocusObject.GetComponent<MeshRenderer>().enabled == true)
                    leftIterations++;
                else
                    rightIterations++;
            }
            inCooldown = true;
            leftFocusObject.SetActive(false);
            rightFocusObject.SetActive(false);
        }
    }

    void Swap()
    {
        if (leftFocusObject.GetComponent<MeshRenderer>().enabled == false && leftIterations < maxLeftIterations)
        {
            leftFocusObject.GetComponent<MeshRenderer>().enabled = true;
            rightFocusObject.GetComponent<MeshRenderer>().enabled = false;
            leftIterations++;
        }
        else
        {
            if (rightIterations < maxRightIterations)
            {
                rightFocusObject.GetComponent<MeshRenderer>().enabled = true;
                leftFocusObject.GetComponent<MeshRenderer>().enabled = false;
                rightIterations++;
            }
        }
    }

    void Cooldown()
    {
        inCooldown = false;
        leftFocusObject.SetActive(true);
        rightFocusObject.SetActive(true);
        runningCooldown = false;
        lastTime = currentTime;
    }

    bool FocusCenter()
    {
        if (FocusManager.FocusName() == FindCenterFocusObject().name)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
