using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    private string nick;
    public Color color = Color.red;
    public Dictionary<HexField.hexType, int> collected = new Dictionary<HexField.hexType, int>();
    public static GameObject Displays;
    private static NumberDisplay clay;
    private static NumberDisplay mineral;
    private static NumberDisplay sheep;
    private static NumberDisplay wheat;
    private static NumberDisplay wood;

    public int AddCollected(HexField.hexType htpye, int amount = 0)
    {
        if (collected.ContainsKey(htpye))
            collected[htpye] += amount;
        else
            collected.Add(htpye, amount);
        switch (htpye)
        {
            case HexField.hexType.Clay:
                clay.Amount = collected[htpye];
                break;
            case HexField.hexType.Feild:
                sheep.Amount = collected[htpye];
                break;
            case HexField.hexType.Mineral:
                mineral.Amount = collected[htpye];
                break;
            case HexField.hexType.Wheat:
                wheat.Amount = collected[htpye];
                break;
            case HexField.hexType.Wood:
                wood.Amount = collected[htpye];
                break;
            default:
                break;
        }
        return collected[htpye];
    }

    // Start is called before the first frame update
    void Start()
    {
        nick = gameObject.name;

        Displays = GameObject.FindGameObjectWithTag("CDisplay");
        clay = Displays.transform.GetChild(0).GetComponent<NumberDisplay>();
        mineral = Displays.transform.GetChild(1).GetComponent<NumberDisplay>();
        sheep = Displays.transform.GetChild(2).GetComponent<NumberDisplay>();
        wheat = Displays.transform.GetChild(3).GetComponent<NumberDisplay>();
        wood = Displays.transform.GetChild(4).GetComponent<NumberDisplay>();

        Debug.Log($"[USER] {nick} - ENABLED");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
