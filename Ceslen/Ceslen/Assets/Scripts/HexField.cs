using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexField : MonoBehaviour
{
    public enum hexType { Sea, Clay, Desert, Feild, Mineral, Wheat, Wood }; //Land tipusok
    public hexType HexType = hexType.Sea; //Default Land tipus
    public bool Land = false; //A tenger nem számít Land-nek

    public GameObject SeaObject;
    public GameObject ClayObject;
    public GameObject DesertObject;
    public GameObject FeildObject;
    public GameObject MineralObject;
    public GameObject WheatObject;
    public GameObject WoodObject;


    // Start is called before the first frame update
    void Start()
    {
        CreateHex();
    }

    //Létrehozza a Hexagon a HexType-nak megfelelően
    private void CreateHex()
    {
        GameObject gameObject = Instantiate(GetCurrentHexPrefab(), transform.position, transform.rotation);
        gameObject.transform.parent = transform;
    }

    //Ha már létezik az elem ezzel meg lehet változtatni a tipusát úgy hogy az előzőt kitőrli és egy újat hoz létre
    public void UpdateHexType(hexType type)
    {
        if (HexType == type) //Ha ezzel nem válzotna a tipusa a Hexagonnak akkor nem végez további müveleteket
            return;
        else
            HexType = type;

        if (type == hexType.Sea) //Ha tenger tipusi akkor automatikusan NoNLand-nek sorolja be
            Land = false;
        else
            Land = true;

        GameObject ExistGameObject = null;
        if (transform.childCount > 1 || null != (ExistGameObject = AnyChildIsHex(transform)))//Megkeresi ha van már Child object-je ami Hexagon
            Destroy(ExistGameObject, 0.0f); //Törli a megtalált Hexagont

        CreateHex(); //Létrekozza az új Hexagon az új tipussal
    }

    private GameObject AnyChildIsHex(Transform transform) //Visszadja az első talált Hexagont a PreFab-en belöl (Elvileg nem lehet 1-nél több)
    {
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).tag == "Map Element")
                return transform.GetChild(i).gameObject;
        return null;
    }

    private GameObject GetCurrentHexPrefab() //Visszadja az adott Hexagon tipusához tartozó GameObject Modelt
    {
        switch (HexType)
        {
            case hexType.Sea:
                return SeaObject;
            case hexType.Clay:
                return ClayObject;
            case hexType.Desert:
                return DesertObject;
            case hexType.Feild:
                return FeildObject;
            case hexType.Mineral:
                return MineralObject;
            case hexType.Wheat:
                return WheatObject;
            case hexType.Wood:
                return WoodObject;
            default:
                return null;
        }
    }

    public void CreateTriggers(Transform Parent, GameObject PuppetTriggerObject)
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject gameObject = Instantiate(PuppetTriggerObject, new Vector3(0f,1f,0f)+ transform.position + ((Quaternion.AngleAxis(30.0f + (60.0f * i), Vector3.up) * Vector3.forward).normalized * 5.886f), transform.rotation);
            gameObject.transform.Rotate(0, 90, 0);
            gameObject.GetComponent<PuppetTrigger>().AddField(new GameObject[] { transform.gameObject });
            gameObject.transform.parent = Parent;
        }
    }
}
