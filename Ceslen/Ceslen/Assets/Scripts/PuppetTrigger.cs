using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuppetTrigger : MonoBehaviour
{
    public GameObject Puppet = null;
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
            if (this.connectedField.Contains(connectedField[i]))//csak akkor adja hozzá ha még nem szerepel a meglévők között
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
        /* 
         * Generáláskor több trigger is kerül egy helyre majd van utánna egy frame-se várakoztatás, akkor itt a collison meghívodik
         * és az egymásra helyezett triggerekből csak a legújabb trigger marad fent amit InstanceId alapján határozok meg
         */
        if (collision.gameObject.tag == "PuppetTrigger")//ha egy azonos object-es "ütküzik"
        {
            AddField(collision.gameObject.GetComponent<PuppetTrigger>().ConnectedField);//olvassa át a másik triggerből a saját field-jeit (azokat mikkel nem redelkezik még)
            if (collision.gameObject.GetInstanceID() < transform.gameObject.GetInstanceID()) //Ha a jelenlegi triggernek nagyobb az ID-ja (azaz újabb) akkor törli a régebbit
                Destroy(collision.gameObject, 0.0f);
        }

        if (collision.gameObject.name.Contains("City") || collision.gameObject.name.Contains("Town")) //Ha várossal vagy falu bábúval "ütközik" akkor hozzá adja magát a bábúhoz (használva a PlaceModel.cs-ben)
            collision.gameObject.GetComponent<Puppet>().TouchedTrigger = gameObject;
    }
}
