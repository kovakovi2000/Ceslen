using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathTrigger : MonoBehaviour
{
    public GameObject Path = null;
    public GameObject[] connectedField = new GameObject[3];
    public GameObject[] connectedPuppet = new GameObject[2];
    private byte fEmpty = 0;
    private byte pEmpty = 0;

    public GameObject[] ConnectedField => connectedField;
    public GameObject[] ConnectedPuppet => connectedPuppet;
    public byte FEmpty => fEmpty;
    public byte PEmpty => pEmpty;

    public void AddPuppet(GameObject[] connectedPuppet)
    {
        for (int i = 0; i < connectedPuppet.Length && pEmpty < this.connectedPuppet.Length; i++)
        {
            if (this.connectedPuppet.Contains(connectedPuppet[i]))
                continue;

            this.connectedPuppet[pEmpty] = connectedPuppet[i];
            pEmpty++;
        }
    }

    public void AddField(GameObject[] connectedField)
    {
        for (int i = 0; i < connectedField.Length && fEmpty < this.connectedField.Length; i++)
        {
            if (this.connectedField.Contains(connectedField[i]))
                continue;

            this.connectedField[fEmpty] = connectedField[i];
            fEmpty++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PathTrigger")
        {
            AddField(collision.gameObject.GetComponent<PathTrigger>().ConnectedField);
            if (collision.gameObject.GetInstanceID() < transform.gameObject.GetInstanceID())
                Destroy(collision.gameObject, 0.0f);
        }

        if (collision.gameObject.name.Contains("Road"))
            collision.gameObject.GetComponent<Puppet>().TouchedTrigger = gameObject;
    }
}
