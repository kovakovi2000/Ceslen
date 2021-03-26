using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberDisplay : MonoBehaviour
{
    private int amount = 0;
    private Text display;
    public int Amount {
        get => amount;
        set
        {
            amount = value;
            display.text = FormatNum(amount);
        }
    }

    private static string FormatNum(int amount)
    {
        if (amount > 999)
            return $"{(amount / 1000f).ToString("F1")}K";
        return amount.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        display = gameObject.GetComponent<Text>();
    }
}
