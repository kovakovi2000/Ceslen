using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector2 Sides = Vector2.zero;
    public KeyButton LeftClick = new KeyButton();
    public KeyButton MiddleClick = new KeyButton();
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("LClick"))
        {
            LeftClick.pressing = true;
            LeftClick.pressed++;
        }
        if (Input.GetButtonUp("LClick"))
        {
            LeftClick.pressing = false;
        }


        if (Input.GetButtonDown("MClick"))
        {
            MiddleClick.pressing = true;
            MiddleClick.pressed++;
        }
        if (Input.GetButtonUp("MClick"))
        {
            MiddleClick.pressing = false;
        }


        Sides = Vector2.zero;
        if (Input.mousePosition.y > Screen.height - 5 && !MiddleClick.pressing)
            Sides.y = 3f;
        else if (Input.mousePosition.y < 5 && !MiddleClick.pressing)
            Sides.y = -3f;

        if (Input.mousePosition.x > Screen.width - 5 && !MiddleClick.pressing)
            Sides.x = 3f;
        else if (Input.mousePosition.x < 5 && !MiddleClick.pressing)
            Sides.x = -3f;
    }
}

public class KeyButton
{
    public bool pressing = false;
    public int pressed = 0;
}