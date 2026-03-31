using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckPatientDirection : MonoBehaviour
{
    public TextMeshProUGUI testThreeText;
    public GameObject sphere;
    // Start is called before the first frame update
    void Start()
    {
        if(testThreeText.gameObject.activeSelf)
        {
            sphere.SetActive(true);
            sphere.transform.position = new Vector3(0, Camera.main.transform.position.y, 1);
            sphere.isStatic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
