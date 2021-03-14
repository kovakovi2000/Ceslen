using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMoveCamera : MonoBehaviour
{
    private int maxDown = int.MaxValue;
    private int maxUp = int.MinValue;
    private int maxLeft = int.MaxValue;
    private int maxRight = int.MinValue;

    private int offsetDown = -1;
    private int offsetUp = -59;
    private int offsetLeft = 65;
    private int offsetRight = -65;

    GameObject mCamera;
    public GameObject inputHandler;
    InputHandler IH;
    bool m = false;
    Vector2 mStart;
    Vector3 cStart;
    bool changed = false;

    public int MaxDown { get => maxDown; set => maxDown = value; }
    public int MaxUp { get => maxUp; set => maxUp = value; }
    public int MaxLeft { get => maxLeft; set => maxLeft = value; }
    public int MaxRight { get => maxRight; set => maxRight = value; }

    public int OffsetDown { get => offsetDown; }
    public int OffsetUp { get => offsetUp; }
    public int OffsetLeft { get => offsetLeft; }
    public int OffsetRight { get => offsetRight; }
    public bool Changed { get => changed; set => changed = value; }

    public void Offset()
    {
        maxDown += offsetDown;
        maxUp += offsetUp;
        maxLeft += offsetLeft;
        MaxRight += offsetRight;
    }

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
            if (v3.z > maxUp) v3.z = maxUp;//242
            if (v3.z < maxDown) v3.z = maxDown;//0
            if (v3.x < maxLeft) v3.x = maxLeft;//40
            if (v3.x > maxRight) v3.x = maxRight;//260
            mCamera.transform.position = v3;
            changed = true;
        }
    }
}
