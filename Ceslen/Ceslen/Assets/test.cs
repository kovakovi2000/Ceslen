using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject tr1;
    public GameObject tr2;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3((tr1.transform.position.x + tr2.transform.position.x) / 2, 1, (tr1.transform.position.z + tr2.transform.position.z) / 2);
        transform.LookAt(tr1.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
