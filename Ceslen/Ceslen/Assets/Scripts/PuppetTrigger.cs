using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuppetTrigger : MonoBehaviour
{
    private GameObject[] connectedField = new GameObject[3];
    private GameObject[] connectedPath = new GameObject[3];
    private byte fEmpty = 0;
    private byte pEmpty = 0;

    public GameObject[] ConnectedField => connectedField;
    public GameObject[] ConnectedPath => connectedPath;
    public byte FEmpty => fEmpty;
    public byte PEmpty => pEmpty;

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

    public void AddPath(GameObject[] connectedPath)
    {
        for (int i = 0; i < connectedPath.Length && this.connectedPath.Length < 3; i++)
        {
            if (this.connectedPath.Contains(connectedPath[i]))
                continue;

            this.connectedPath[pEmpty] = connectedPath[i];
            pEmpty++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PuppetTrigger")
        {
            AddField(collision.gameObject.GetComponent<PuppetTrigger>().ConnectedField);
            if (collision.gameObject.GetInstanceID() < transform.gameObject.GetInstanceID())
                Destroy(collision.gameObject, 0.0f);
        }

        if (collision.gameObject.name.Contains("City") || collision.gameObject.name.Contains("Town"))
            collision.gameObject.GetComponent<Puppet>().TouchedTrigger = gameObject;
    }
}
