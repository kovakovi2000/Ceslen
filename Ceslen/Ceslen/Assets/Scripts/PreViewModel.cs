using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreViewModel : MonoBehaviour
{
    public GameObject CityObject; //public Vector3 CityOffset;
    public GameObject TownObject; //public Vector3 TownOffset;
    public GameObject RoadObject; //public Vector3 RoadOffset;

    public enum pwModel { None, City, Town, Road };
    private pwModel PWModel = pwModel.None;
    private pwModel current = pwModel.None;

    
    public void SetModelNone() => PWModel = pwModel.None;
    //Ezek a HUD/ObhectSelector-on belül OnClick()-en hívodnak meg
    public void SetModelCity() => PWModel = pwModel.City;
    public void SetModelTown() => PWModel = pwModel.Town;
    public void SetModelRoad() => PWModel = pwModel.Road;

    void FixedUpdate()
    {
        if (transform.childCount > 0)
            transform.GetChild(0).Rotate(new Vector3(0f, 1f, 0f));

        if (current != PWModel)
        {
            if (transform.childCount > 0)
                Destroy(transform.GetChild(0).gameObject);

            GameObject go = null;
            switch (PWModel)
            {
                case pwModel.City:
                    go = Instantiate(CityObject, transform.position, new Quaternion());
                    break;
                case pwModel.Town:
                    go = Instantiate(TownObject, transform.position, new Quaternion());
                    break;
                case pwModel.Road:
                    go = Instantiate(RoadObject, transform.position, new Quaternion());
                    break;
                default:
                    break;
            }
            if (go != null)
            {
                go.transform.parent = transform;
                go.GetComponent<Puppet>().Model = PWModel;
            }
            current = PWModel;
        }
    }
}
