using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingBehaviour : MonoBehaviour
{
    private bool chainMaster = false;
    private Dictionary<HexField.hexType, int> collectRate;

    private List<GameObject> connectedPuppet = new List<GameObject>();
    private List<GameObject> connectedPath = new List<GameObject>();

    public List<GameObject> ConnectedPuppet => connectedPuppet;
    public List<GameObject> ConnectedPath => connectedPath;

    public void AddPuppet(GameObject[] connectedPuppet)
    {
        for (int i = 0; i < connectedPuppet.Length; i++)
        {
            if (this.connectedPuppet.Contains(connectedPuppet[i]))
                continue;

            this.connectedPuppet.Add(connectedPuppet[i]);
        }
        refreshCollectRate();
    }
    public void AddPuppet(List<GameObject> connectedPuppet)
    {
        foreach (var item in connectedPuppet)
        {
            if (this.connectedPuppet.Contains(item))
                continue;

            this.connectedPuppet.Add(item);
        }
        refreshCollectRate();
    }

    public void AddPath(GameObject[] connectedPath)
    {
        for (int i = 0; i < connectedPath.Length; i++)
        {
            if (this.connectedPath.Contains(connectedPath[i]))
                continue;

            this.connectedPath.Add(connectedPath[i]);
        }
        refreshCollectRate();
    }
    public void AddPath(List<GameObject> connectedPath)
    {
        foreach (var item in connectedPath)
        {
            if (this.connectedPath.Contains(item))
                continue;

            this.connectedPath.Add(item);
        }
        refreshCollectRate();
    }

    private void refreshCollectRate()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PLACED");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
