using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingBehaviour : MonoBehaviour
{
    private System.Random RND = new System.Random();
    private int id;
    private bool chainMaster = true;
    private Dictionary<HexField.hexType, int> collectRate;

    public List<GameObject> connectedPuppet = new List<GameObject>();
    public List<GameObject> connectedPath = new List<GameObject>();

    public List<GameObject> ConnectedPuppet => connectedPuppet;
    public List<GameObject> ConnectedPath => connectedPath;

    public void AddPuppet(GameObject connectedPuppet, bool refresh = false)
    {
        if (!this.connectedPuppet.Contains(connectedPuppet))
            this.connectedPuppet.Add(connectedPuppet);
        if(refresh) refreshCollectRate();
    }
    public void AddPuppet(GameObject[] connectedPuppet, bool refresh = false)
    {
        for (int i = 0; i < connectedPuppet.Length; i++)
        {
            if (this.connectedPuppet.Contains(connectedPuppet[i]))
                continue;

            this.connectedPuppet.Add(connectedPuppet[i]);
        }
        if(refresh) refreshCollectRate();
    }
    public void AddPuppet(List<GameObject> connectedPuppet, bool refresh = false)
    {
        foreach (var item in connectedPuppet)
        {
            if (this.connectedPuppet.Contains(item))
                continue;

            this.connectedPuppet.Add(item);
        }
        if(refresh) refreshCollectRate();
    }

    public void AddPath(GameObject connectedPath, bool refresh = false)
    {
        if (!this.connectedPath.Contains(connectedPath))
            this.connectedPath.Add(connectedPath);
        if(refresh) refreshCollectRate();
    }
    public void AddPath(GameObject[] connectedPath, bool refresh = false)
    {
        for (int i = 0; i < connectedPath.Length; i++)
        {
            if (this.connectedPath.Contains(connectedPath[i]))
                continue;

            this.connectedPath.Add(connectedPath[i]);
        }
        if(refresh) refreshCollectRate();
    }
    public void AddPath(List<GameObject> connectedPath, bool refresh = false)
    {
        foreach (var item in connectedPath)
        {
            if (this.connectedPath.Contains(item))
                continue;

            this.connectedPath.Add(item);
        }
        if(refresh) refreshCollectRate();
    }

    private void refreshCollectRate()
    {
        var col = new Color((float)RND.NextDouble(), (float)RND.NextDouble(), (float)RND.NextDouble(), 1f);
        foreach (var item in connectedPath)
            if (item != null) item.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = col;
        foreach (var item in connectedPuppet)
            if (item != null) item.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = col;

    }
    // Start is called before the first frame update
    void Start()
    {
        //TODO Setup puppet that already exist
        Debug.Log("PLACED");


        /* Valamiért mindig hiba és hiba és hiba, újra írom...
        id = gameObject.GetInstanceID();
        GameObject trigger = transform.GetComponent<Puppet>().TouchedTrigger;
        Dictionary<GameObject, bool> localPath = new Dictionary<GameObject, bool>();
        Dictionary<GameObject, bool> localPuppet = new Dictionary<GameObject, bool>();
        if (trigger.tag == "PuppetTrigger")
        {
            localPuppet.Add(gameObject, false);
            foreach (var pathTrigger in trigger.GetComponent<PuppetTrigger>().ConnectedPath)
            {
                if (pathTrigger == null)
                    continue;
                GameObject Path = pathTrigger.GetComponent<PathTrigger>().Path;
                if (Path != null)
                    localPath.Add(Path, false);
            }
        }
        else
        {
            localPath.Add(gameObject, false);
            foreach (var PuppetTrigger in trigger.GetComponent<PathTrigger>().ConnectedPuppet)
            {
                if (PuppetTrigger == null)
                    continue;
                GameObject Puppet = PuppetTrigger.GetComponent<PuppetTrigger>().Puppet;
                if (Puppet != null)
                    localPuppet.Add(Puppet, false);
            }
        }
        Debug.Log($"localPath: {localPath.Count} | localPuppet: {localPuppet.Count}");
        do
        {
            List<GameObject> updatePath = new List<GameObject>();
            List<GameObject> updatePuppet = new List<GameObject>();
            foreach (var item in localPath)
            {
                if (item.Value)
                    continue;
                AddPath(item.Key);
                foreach (var cPuppetTrigger in item.Key.GetComponent<Puppet>().TouchedTrigger.GetComponent<PathTrigger>().ConnectedPuppet)
                {
                    GameObject cPuppet = cPuppetTrigger.GetComponent<PuppetTrigger>().Puppet;
                    if (!localPuppet.ContainsKey(cPuppet))
                        localPuppet.Add(cPuppet, false);
                }
                updatePath.Add(item.Key);
            }

            foreach (var item in localPuppet)
            {
                if (item.Value)
                    continue;
                AddPuppet(item.Key);
                foreach (var cPathTrigger in item.Key.GetComponent<Puppet>().TouchedTrigger.GetComponent<PuppetTrigger>().ConnectedPath)
                {
                    GameObject cPath = cPathTrigger.GetComponent<PathTrigger>().Path;
                    if (!localPath.ContainsKey(cPath))
                        localPath.Add(cPath, false);
                }
                updatePuppet.Add(item.Key);
            }

            foreach (var item in updatePath)
                localPath[item] = true;
            foreach (var item in updatePuppet)
                localPuppet[item] = true;
        } while (localPath.ContainsValue(false) || localPuppet.ContainsValue(false));
        */
        refreshCollectRate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //TODO Add resource at ticks
    }
}
