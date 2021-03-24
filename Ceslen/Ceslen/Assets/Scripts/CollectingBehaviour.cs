using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectingBehaviour : MonoBehaviour
{
    private System.Random RND = new System.Random();
    private bool clean;
    public GameObject Master = null;
    private int collectRate = 0;
    private Dictionary<HexField.hexType, int> collectRateType = new Dictionary<HexField.hexType, int>();

    private List<GameObject> connectedPuppet = new List<GameObject>();
    private List<GameObject> connectedPath = new List<GameObject>();
    private List<GameObject> connectedField = new List<GameObject>();

    public List<GameObject> ConnectedPuppet => connectedPuppet;
    public List<GameObject> ConnectedPath => connectedPath;
    public List<GameObject> ConnectedField => connectedField;

    public bool AddPuppet(GameObject connectedPuppet, bool refresh = false)
    {
        bool re = false;
        if (connectedPuppet != null && !this.connectedPuppet.Contains(connectedPuppet))
        {
            this.connectedPuppet.Add(connectedPuppet);
            re = true;
        }
        if(refresh) refreshCollectRate();
        return re;
    }
    public bool AddPuppet(GameObject[] connectedPuppet, bool refresh = false)
    {
        bool re = false;
        for (int i = 0; i < connectedPuppet.Length; i++)
        {
            if (connectedPuppet[i] == null || this.connectedPuppet.Contains(connectedPuppet[i]))
                continue;

            this.connectedPuppet.Add(connectedPuppet[i]);
            re = true;
        }
        if(refresh) refreshCollectRate();
        return re;
    }
    public bool AddPuppet(List<GameObject> connectedPuppet, bool refresh = false)
    {
        bool re = false;
        foreach (var item in connectedPuppet)
        {
            if (item == null || this.connectedPuppet.Contains(item))
                continue;

            this.connectedPuppet.Add(item);
            re = true;
        }
        if(refresh) refreshCollectRate();
        return re;
    }

    public bool AddPath(GameObject connectedPath, bool refresh = false)
    {
        bool re = false;
        if (connectedPuppet != null && !this.connectedPath.Contains(connectedPath))
        {
            this.connectedPath.Add(connectedPath);
            re = true;
        }
            
        if(refresh) refreshCollectRate();
        return re;
    }
    public bool AddPath(GameObject[] connectedPath, bool refresh = false)
    {
        bool re = false;
        for (int i = 0; i < connectedPath.Length; i++)
        {
            if (connectedPath[i] == null || this.connectedPath.Contains(connectedPath[i]))
                continue;

            this.connectedPath.Add(connectedPath[i]);
            re = true;
        }
        if(refresh) refreshCollectRate();
        return re;
    }
    public bool AddPath(List<GameObject> connectedPath, bool refresh = false)
    {
        bool re = false;
        foreach (var item in connectedPath)
        {
            if (item == null || this.connectedPath.Contains(item))
                continue;

            this.connectedPath.Add(item);
            re = true;
        }
        if(refresh) refreshCollectRate();
        return re;
    }

    public bool AddField(GameObject connectedField, bool refresh = false)
    {
        bool re = false;
        if (connectedField != null && !this.connectedField.Contains(connectedField))
        {
            this.connectedField.Add(connectedField);
            re = true;
        }
        if (refresh) refreshCollectRate();
        return re;
    }
    public bool AddField(GameObject[] connectedField, bool refresh = false)
    {
        bool re = false;
        for (int i = 0; i < connectedField.Length; i++)
        {
            if (connectedField[i] == null || this.connectedField.Contains(connectedField[i]))
                continue;

            this.connectedField.Add(connectedField[i]);
            re = true;
        }
        if (refresh) refreshCollectRate();
        return re;
    }
    public bool AddField(List<GameObject> connectedField, bool refresh = false)
    {
        bool re = false;
        foreach (var item in connectedField)
        {
            if (item == null || this.connectedField.Contains(item))
                continue;

            this.connectedField.Add(item);
            re = true;
        }
        if (refresh) refreshCollectRate();
        return re;
    }

    private void refreshCollectRate()
    {
        var col = new Color((float)RND.NextDouble(), (float)RND.NextDouble(), (float)RND.NextDouble(), 1f);
        foreach (GameObject item in connectedPath)
        {
            AddField(item.GetComponent<Puppet>().TouchedTrigger.GetComponent<PathTrigger>().ConnectedField);
            item.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = col;
            var cb = item.GetComponent<CollectingBehaviour>();
            cb.Master = gameObject;
        }
        foreach (var item in connectedPuppet)
        {
            var puppet = item.GetComponent<Puppet>();
            AddField(puppet.TouchedTrigger.GetComponent<PuppetTrigger>().ConnectedField);
            collectRate += puppet.CollectRate;
            item.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = col;
            var cb = item.GetComponent<CollectingBehaviour>();
            cb.Master = gameObject;
        }

        collectRateType.Clear();
        foreach (HexField.hexType hex in (HexField.hexType[])HexField.hexType.GetValues(typeof(HexField.hexType)))
            collectRateType.Add(hex, 0);
        foreach (var item in connectedField)
            collectRateType[item.GetComponent<HexField>().HexType] += 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        Master = gameObject;
        clean = false;

        GameObject trigger = transform.GetComponent<Puppet>().TouchedTrigger;
        if (trigger.tag == "PuppetTrigger")
        {
            AddPuppet(gameObject);
            //A jelenlegi triggerhez csatlakozattott út triggereknek az út object-je.
            AddPath(trigger.GetComponent<PuppetTrigger>().ConnectedPathTrigger.OfType<GameObject>().ToList().Select(x => x.GetComponent<PathTrigger>().Path).ToList());
        }
        else
        {
            AddPath(gameObject);
            var cPuppet = trigger.GetComponent<PathTrigger>().ConnectedPuppetTrigger.ToList();
            cPuppet.ForEach(tPuppet => AddPath(tPuppet.GetComponent<PuppetTrigger>().ConnectedPathTrigger.OfType<GameObject>().ToList().Select(tPath => tPath.GetComponent<PathTrigger>().Path).ToList()));
            AddPuppet(cPuppet.Select(tPuppet => tPuppet.GetComponent<PuppetTrigger>().Puppet).ToList());
        }

        bool re = false;
        do
        {
            re = false;
            connectedPath.ToList().ForEach(path => { if (path != gameObject && AddPath(path.GetComponent<CollectingBehaviour>().Master.GetComponent<CollectingBehaviour>().ConnectedPath)) re = true; });
            connectedPath.ToList().ForEach(path => { if (path != gameObject && AddPuppet(path.GetComponent<CollectingBehaviour>().Master.GetComponent<CollectingBehaviour>().ConnectedPuppet)) re = true; });

            connectedPuppet.ToList().ForEach(puppet => { if (puppet != gameObject && AddPath(puppet.GetComponent<CollectingBehaviour>().Master.GetComponent<CollectingBehaviour>().ConnectedPath)) re = true; });
            connectedPuppet.ToList().ForEach(puppet => { if (puppet != gameObject && AddPuppet(puppet.GetComponent<CollectingBehaviour>().Master.GetComponent<CollectingBehaviour>().ConnectedPuppet)) re = true; });
        } while (re);

        refreshCollectRate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //TODO Add resource at ticks
        if (Master == gameObject)
        {

        }
        else
        {
            if (!clean)
            {
                connectedPath.Clear();
                connectedPuppet.Clear();
                clean = true;
            }
        }
    }
}
