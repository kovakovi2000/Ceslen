using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapVisibility : MonoBehaviour
{
    public GameObject MainCamera;
    SideMoveCamera SMC;
    Transform MCT;
    public GameObject ModelRay;
    PlaceModel PM;
    GameObject[] physicsGameObject;
    GameObject[] renderGameObject;

    bool lPM_HoldingPuppet = false;
    // Start is called before the first frame update
    void Start()
    {
        //Eltárólom a töbszöri felhasználás miatt ezeket az értékeket
        int hexCount = transform.GetChild(0).transform.childCount; //Azokat amik vizualitásuknál fogva okoznak laggot (note: a colide kell benne mindig hogy a click-et lehessen ray-el megfoni)
        int puppetTriggerCount = transform.GetChild(1).transform.childCount; //Bábuk triggerek száma
        int pathTriggerCount = transform.GetChild(2).transform.childCount; //Utak triggerek száma
        
        renderGameObject = new GameObject[hexCount];
        for (int i = 0; i < hexCount; i++)
            renderGameObject[i] = transform.GetChild(0).GetChild(i).GetChild(0).gameObject;

        physicsGameObject = new GameObject[puppetTriggerCount + pathTriggerCount];
        for (int i = 0; i < puppetTriggerCount; i++)
            physicsGameObject[i] = transform.GetChild(1).GetChild(i).gameObject;

        for (int i = 0; i < pathTriggerCount; i++)
            physicsGameObject[puppetTriggerCount + i] = transform.GetChild(2).GetChild(i).gameObject;

        foreach (var item in renderGameObject)
            item.SetActive(false);
        foreach (var item in physicsGameObject)
            item.SetActive(false);

        SMC = MainCamera.GetComponent<SideMoveCamera>();
        MCT = MainCamera.transform;
        PM = ModelRay.GetComponent<PlaceModel>();
        Debug.LogWarning("Map visibility done!");

        lPM_HoldingPuppet = PM.HoldingPuppet;
    }

    private bool InView(Transform t)
    {
        if (t.position.z > (MCT.position.z + -10) && 
            t.position.z < (MCT.position.z + 70) &&
            t.position.x < (MCT.position.x + 75) &&
            t.position.x > (MCT.position.x - 75))
            return true;

        return false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SMC.Changed)
            foreach (var item in renderGameObject)
            {
                if (InView(item.transform))
                    item.SetActive(true);
                else
                    item.SetActive(false);
            }



        if (lPM_HoldingPuppet != PM.HoldingPuppet)
        {
            lPM_HoldingPuppet = PM.HoldingPuppet;
            if (PM.HoldingPuppet)
                foreach (var item in physicsGameObject)
                {
                    if (InView(item.transform))
                        item.SetActive(true);
                    else
                        item.SetActive(false);
                }
            else
                foreach (var item in physicsGameObject)
                    item.SetActive(false);
        }
    }
}
