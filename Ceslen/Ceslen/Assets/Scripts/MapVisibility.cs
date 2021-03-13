using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapVisibility : MonoBehaviour
{
    GameObject[] physicsGameObject;
    GameObject[] renderGameObject;
    // Start is called before the first frame update
    void Start()
    {
        int hexCount = transform.GetChild(0).transform.childCount;
        int puppetTriggerCount = transform.GetChild(1).transform.childCount;
        int pathTriggerCount = transform.GetChild(2).transform.childCount;

        Debug.LogWarning("Map visibility done!");
        renderGameObject = new GameObject[hexCount];
        for (int i = 0; i < hexCount; i++)
            renderGameObject[i] = transform.GetChild(0).GetChild(i).GetChild(0).gameObject;

        physicsGameObject = new GameObject[puppetTriggerCount + pathTriggerCount];
        for (int i = 0; i < puppetTriggerCount; i++)
            physicsGameObject[i] = transform.GetChild(1).GetChild(i).gameObject;

        for (int i = 0; i < pathTriggerCount; i++)
            physicsGameObject[puppetTriggerCount + i] = transform.GetChild(2).GetChild(i).gameObject;

        foreach (var item in physicsGameObject)
        {
            item.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
