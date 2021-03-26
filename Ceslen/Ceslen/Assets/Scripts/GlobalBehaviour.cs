using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalBehaviour : MonoBehaviour
{
    public event TickHandler eSeconds;
    public event TickHandler eMinutes;
    public event TickHandler eHours;

    public EventArgs e = null;
    public delegate void TickHandler(GlobalBehaviour m, EventArgs e);
    private int called = 0;
    private int seconds = 0;
    private int minutes = 0;
    private int hours = 0;
    // Start is called before the first frame update
    void Start()
    {
        eSeconds += new TickHandler((sender, e) => eUpdate("Second"));
        eMinutes += new TickHandler((sender, e) => eUpdate("Minute"));
        eHours += new TickHandler((sender, e) => eUpdate("Hour"));
    }

    private void eUpdate(string type)
    {
        //Debug.Log($"[EVENT] {type} - {hours}:{minutes}:{seconds}");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        called++;
        if (called == 50)
        {
            eSeconds(this, e);
            called = 0;
            seconds++;
            if (seconds == 60)
            {
                eMinutes(this, e);
                seconds = 0;
                minutes++;
                if (minutes == 60)
                {
                    eHours(this, e);
                    minutes = 0;
                    hours++;
                }
            }
        }
    }
}
