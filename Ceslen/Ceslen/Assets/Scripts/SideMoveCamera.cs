using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMoveCamera : MonoBehaviour
{
    GameObject mCamera;
    public GameObject inputHandler;
    InputHandler IH;
    bool m = false;
    Vector2 mStart;
    Vector3 cStart;

    // Start is called before the first frame update
    void Start()
    {
        mCamera = Camera.main.gameObject;
        IH = inputHandler.GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 v3 = -Vector3.one;
        if (!m)
        {
            m = true;
            mStart = Input.mousePosition;
            cStart = mCamera.transform.position;
        }

        if (IH.MiddleClick.pressing)
        {
            Vector2 mNow = new Vector2(mStart.x - Input.mousePosition.x, mStart.y - Input.mousePosition.y);
            mNow *= 0.1f;
            v3 = new Vector3(cStart.x + mNow.x, cStart.y, cStart.z + mNow.y);
        }
        else
        {
            Vector3 v3cam = mCamera.transform.position;
            v3 = new Vector3(v3cam.x + IH.Sides.x, v3cam.y, v3cam.z + IH.Sides.y);
            m = false;
        }


        if (v3 != -Vector3.one)
        {
            if (v3.z > 242) v3.z = 242;
            if (v3.z < 0) v3.z = 0;
            if (v3.x < 40) v3.x = 40;
            if (v3.x > 260) v3.x = 260;
            mCamera.transform.position = v3;
        }
    }
}
