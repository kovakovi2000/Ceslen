using UnityEngine;

public class PlaceModel : MonoBehaviour
{
    public LayerMask clickMask;
    public GameObject Button;
    public GameObject InputHandler;
    public GameObject PreViewerHold;
    private GameObject Puppet = null;
    private bool holdingPuppet = false;
    private bool GotLocation = false;

    public bool HoldingPuppet { get => holdingPuppet; }

    void LateUpdate()
    {
        if (!holdingPuppet && Button.GetComponent<PressingUIButton>().Pressing && InputHandler.GetComponent<InputHandler>().LeftClick.pressing)
        {
            if (PreViewerHold.transform.childCount > 0)
            {
                Puppet = PreViewerHold.transform.GetChild(0).gameObject;
                holdingPuppet = true;
            }
        }

        if (holdingPuppet && !InputHandler.GetComponent<InputHandler>().LeftClick.pressing)
        {
            holdingPuppet = false;
            if (!GotLocation)
            {
                Puppet.transform.parent = PreViewerHold.transform;
                Puppet.transform.position = PreViewerHold.transform.position;
                Puppet.transform.localScale = new Vector3(1f, 1f, 1f);
                Puppet.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            }
            Puppet = null;
        }



        if (holdingPuppet)
        {
            if (Button.GetComponent<PressingUIButton>().Hover)
            {
                Puppet.transform.parent = PreViewerHold.transform;
                Puppet.transform.position = PreViewerHold.transform.position;
                Puppet.GetComponent<Puppet>().TouchedTrigger = null;
                Puppet.transform.localScale = new Vector3(1f, 1f, 1f);
                Puppet.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                Puppet.transform.parent = transform;
                PreViewerHold.GetComponent<PreViewModel>().SetModelNone();
                Puppet.transform.rotation = new Quaternion();
                Puppet.transform.Rotate(0f,90f,0f);
                Puppet.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 150.0f, clickMask))
                {
                    Vector3 hitpoint = hit.point;
                    GameObject TouchedTrigger = Puppet.GetComponent<Puppet>().TouchedTrigger;
                    if (TouchedTrigger != null)
                    {
                        var TriggerPos = TouchedTrigger.transform.position;
                        TriggerPos.y = hitpoint.y;
                        if (Vector3.Distance(TriggerPos, hitpoint) < 5.0f)
                        {
                            Puppet.transform.position = TouchedTrigger.transform.position;
                            Puppet.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, 1f);
                            Puppet.transform.rotation = TouchedTrigger.transform.rotation;
                            GotLocation = true;
                        }
                        else
                            TouchedTrigger = null;
                    }

                    if (TouchedTrigger == null)
                    {
                        Puppet.transform.position = hitpoint;
                        Puppet.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 0.2f);
                        GotLocation = false;
                    }

                }
            }
        }
    }
}
