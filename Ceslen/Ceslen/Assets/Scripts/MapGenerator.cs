#define _DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Külső függgvény lista keverésére
public static class IListExtensions
{
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

public class MapGenerator : MonoBehaviour
{
    [Range(30, 300)] public int GridX = 100; //magassága a táblának
    [Range(30, 300)] public int GridY = 100; //szélessége a táblának
    [Range(0.0f, 40.0f)] public float MarginPresent = 0.0f; //Keep it under 40f //LandCore a pályának a szélétől számítva kőzép írányba százalékosan
    [Range(0.0f, 1000.0f)] public float MarginSensivity = 50.0f; //Azt szabályozza hogy milyen valoszinuséggel alakulhat ki Land a MarginPresent értékeken kivül eső kordinátákon
    public bool DoIlandOnlyOnSea = true; //Ha ez igaz akkor leellenőrzi az ilandcore körül hogy van-e viz mindenhol és csak akkor rakja le
    public int IlandCount = 7; //Hány LandCore-t hozzon létre
    [Range(0.0f, 0.99999f)] public float IlandAvgSize = 0.85f; //keep it under 1.0f //Mekkora legyen átlagosan egy szíget
    [Range(0.0f, 0.99999f)] public float GrowMultiple = 0.9f; //keep it under 1.0f //Mennyire legyen visszafogva a szíget nővekedése (minnél nagyobb a szám annál jobban nő)
    public bool RemoveLakes = true; //A lyukakak és őblőket távolítja el amitől egybefüggübb lesz a térkép
    [Range(0, 6)] public int LakeSensivity = 0; //keep it under 6 //Hány környező Land-nek kell lennie ahhoz hogy a vizsgált Lake-et Land-re változzoztassa
    [Range(0, 30)] public int LakesRecheck = 0; //Hányszor nézze át a Land környezetét Lake után keresve

    //várjon-e generálás közbe új elemek lehelyezésekor hogy vizuálisabb legyen a generálás (SG_ = SlowGenerate)
    public bool SG_Iland = true;
    public bool SG_IlandHexagon = true;
    public bool SG_IlandHexagonLayer = true;
    public bool SG_LakeRemove = true;
    public bool SG_LakeRemoveLayer = true;
    public bool SG_PuppetTriggers = true;
    public bool SG_PathTriggers = true;

    public GameObject Hexagon; //Prefab ami tartalmazza az ősszes tipusú Hexagont
    public GameObject PathTrigger; //Prefab ami tarlamazza az útak leraksi helyét
    public GameObject PuppetTrigger; //Prefab ami tarlamazza a bábuk leraksi helyét
    private static List<List<Cor>> GridElementList = new List<List<Cor>>(); //Egy 2D list ami hexagonokat tárolja + Generálási információkat

    public Camera MainCamera;
    public Camera MiniMapCamera;
    public RenderTexture MiniMapOverLay;

    private static System.Random RND = new System.Random();
    
    private class Cor
    {
        private GameObject gameObject; //Prefab ami tartalmazza az ősszes tipusú Hexagont
        private HexField hexField; //Prefab társ script class
        private Vector2Int cordinate; //GridElementList-be tárolt indexek értéke is egyeben, tovább a pálya kordináta
        public int Update = 0; //TODO 
        //ez az érték fogja késöbb tárolni hogy egy mező modosítva lett. (Mivel jelenleg a landtype-ja határozza meg)

        public Cor(GameObject gameObject, Vector2Int vector2Int)//Construktor
        {
            this.gameObject = gameObject;
            this.hexField = gameObject.GetComponent<HexField>();
            this.cordinate = vector2Int;
        }

        public string CordinateToString() //Szőveggé alakítja a adott elem kordínátáján, nagyban debug fügvény
        {
            return $"x={cordinate.x} | y={cordinate.y}";
        }

        //Getter-ek Setter-ek
        public GameObject GameObject { get => gameObject; set => gameObject = value; }
        public HexField HexField { get => hexField; set => hexField = value; }
        public Vector2Int Cordinate { get => cordinate; }
    }

    private class GenerateHexagon //Maga a LandSkape kigenerása
    {

        public static IEnumerator GenerateGrid(
            Transform transform,
            int xMax,
            int yMax,
            GameObject HexagonObject,
            GameObject PathTriggerObject,
            GameObject PuppetTriggerObject,
            bool DoIlandOnlyOnSea,
            int IlandCount,
            float MarginPresent,
            float MarginSensivity,
            float IlandAvgSize,
            float GrowMultiple,
            bool RemoveLakes,
            int LakeSensivity,
            int LakesRecheck,
            Camera MainCamera,
            Camera MiniMapCamera,
            RenderTexture MiniMapOverLay,
            bool SlowGenerateIland = false,
            bool SlowGenerateIlandHexagon = false,
            bool SlowGenerateIlandHexagonLayer = false,
            bool SlowGenerateLakeRemove = false,
            bool SlowGenerateLakeRemoveLayer = false,
            bool SlowGeneratePuppetTriggers = false,
            bool SlowGeneratePathTriggers = false
            )
        {
            //Egy csak Sea-ből álló megadott méretű területet generál
            #region CreateGrid
            for (int x = 0; x < xMax; x++)
            {
                List<Cor> sublist = new List<Cor>(); //Egy allista mely a sorokat tartalmazza
                for (int y = 0; y < yMax; y++)
                {
                    Cor temp = null;
                    if (x % 2 == 0)
                        temp = new Cor(Instantiate(HexagonObject, new Vector3(x * 8.83f, 0, y * 10.2f), HexagonObject.transform.rotation), new Vector2Int(x, y));
                    else
                        temp = new Cor(Instantiate(HexagonObject, new Vector3(x * 8.83f, 0, y * 10.2f + 5.1f), HexagonObject.transform.rotation), new Vector2Int(x, y));
                    temp.GameObject.transform.parent = transform.GetChild(0);
                    sublist.Add(temp);
                }
                GridElementList.Add(sublist);
            }
            #if _DEBUG
                Debug.Log("Grid Generation done!"); yield return null;
            #endif
            #endregion

            //Létrehozza a szígeteket
            #region GenerateIlands
            var trys = 0;
            for (int i = 0; i < IlandCount; i++)
            {
                var IstandCore = GetHexBy2dV(GetRandomHexOnGrid(xMax, yMax, MarginPresent), GridElementList); //Egy Random kiválasztott Hexagon a pályán a Margin szabálya szerint
                if (trys > 30)//Egy biztonsági elleőrzés ami amiatt kell ha végtelen ciklusba esne a DoIlandOnlyOnSea miatt
                {
                    Debug.LogError("F*CK I CAN'T GENERATE THE REQUERD LAND COUNT!");
                    break;
                }
                if (DoIlandOnlyOnSea && GetHexAround(IstandCore.Cordinate, GridElementList).Where(x => x.HexField.HexType != HexField.hexType.Sea).Count() != 0)
                {
                    trys++;
                    i--;
                    continue;
                }
                trys = 0;
                MakePreLand(IstandCore, HexField.hexType.Feild); //A kiválasztott Hexagon átalakítom Land-é;
                //Debug.Log($"Generated origin: {IstandCore.CordinateToString()}");

                var runbreak = 0; //Számláló hogy hányat lépett el a Core-tól
                var ilandAvgSize = IlandAvgSize; //Helyi változóba helyezem mivel késöbb modosítani fogom, viszont az eredetire is szügség lesz még
                var ilandLayer = GetHexAround(IstandCore.Cordinate, GridElementList); //Jelenleg viszgát Hexagon réteg a Core köröl
                while (ilandLayer.Count > 0 && runbreak != IlandAvgSize) //Addig változztatja a Hexagon-okat ameddig nem marad átrakható terület a szabályok szerint vagy el nem éri a maximum lépést a Core-től
                {
                    var iLandSurround = new List<Cor>(); //A következő Hexagon réteget tárolja el
                    ilandLayer.Shuffle();//Összekeveri a listát hogy a nőveljük a randomizálást
                    for (int ILLC = 0; ILLC < ilandLayer.Count - 1; ILLC++) //A Generált rész körüli Hexagon rész tesztelése (legújabb layer)
                    {
                        //Csak a NoNLand területekből ilandAvgSize valószinüségéve + a környező Landek nővelik a valószinuségét újabb Land kialakulásnak
                        if (ilandLayer[ILLC].Update != 1 &&
                                ilandAvgSize + 
                                (GetHexAround(ilandLayer[ILLC].Cordinate, GridElementList).Where(x => x.Update == 1).Count() / 10) > UnityEngine.Random.Range(0f, 1f) &&
                                MarginMultiple(ilandLayer[ILLC].Cordinate.x, ilandLayer[ILLC].Cordinate.y, xMax, yMax, MarginPresent, MarginSensivity) < UnityEngine.Random.Range(0f, 1f)
                                )
                        {
                            //Debug.Log($"{ilandAvgSize} > {rnd} at [{ILLC}]: {ilandLayer[ILLC].CordinateToString()}");
                            MakePreLand(ilandLayer[ILLC], HexField.hexType.Wood); //A kiválasztott Hexagon átalakítom Land-é;
                            iLandSurround.AddRange(GetHexAround(ilandLayer[ILLC].Cordinate, GridElementList)); //Az új landünk kőrűl a Hexagonokat hozzádjuk a következő réteghez

                            if (SlowGenerateIlandHexagon)
                                yield return null;
                        }
                    }
                    ilandLayer.Clear(); //A jelenleg viszgált réteget kiüritjuk
                    ilandLayer.AddRange(iLandSurround.Where(x => x.Update != 1 && ilandAvgSize > UnityEngine.Random.Range(0f, 1f)).ToList()); //Hozzáadjuk a jelenleg viszgált listához a következő réteget
                    ilandAvgSize *= GrowMultiple; // csökkentjük a Land kialakulásának valószinüségét a GrowMultiple változóval
                    runbreak++; //Nőveljuk a Core-tól megtett lépések számát
                    if (SlowGenerateIlandHexagonLayer)
                        yield return null;
                }
                if (SlowGenerateIland)
                    yield return null;
            }
            Debug.Log("Ilands Generation done!");
            yield return null;
            #endregion

            //Eltávolítja a tavakat és kisímítja a tengerpartokat
            #region RemoveLakes
            if (RemoveLakes)
            {
                var IslandBeach = new List<Cor>();
                for (int i = 0; i < LakesRecheck; i++)
                {
                    //Bejárja az egész pályát és olyan elemeket keres ami Közvetlen Land mellett lévő Sea
                    for (int x = 0; x < xMax; x++)
                    {
                        for (int y = 0; y < yMax; y++)
                        {
                            if (GridElementList[x][y].Update == 1)
                            {
                                foreach (var item in GetHexAround(GridElementList[x][y].Cordinate, GridElementList).Where(item => item.Update == 0).ToList())
                                {
                                    //Elkerüli a duplikációt
                                    if (!IslandBeach.Contains(item))
                                        IslandBeach.Add(item);
                                }
                            }
                        }
                    }
                    IslandBeach.Shuffle(); //Ősszekeveri a találatokat hogy amikor bejárja a listát ne szabályszerű legyen a kirajzolás ezzel is nővelve a pálya randomitását
                    foreach (var item in IslandBeach)
                    {
                        //Az éppen vizsgált Sea körül a Land szám <= mit LakeSensivity akkor az adott Sea-ből land lesz
                        if (GetHexAround(item.Cordinate, GridElementList).Where(x => x == null || x.Update == 0).Count() <= LakeSensivity)
                        {
                            MakePreLand(item, HexField.hexType.Wood);
                            if (SlowGenerateLakeRemove)
                                yield return null;
                        }
                    }
                    if (SlowGenerateLakeRemoveLayer)
                        yield return null;
                }
            }
            #if _DEBUG
                Debug.Log("Lakes Remove done!"); yield return null;
#endif
            #endregion

            //TODO generate field types
            #region FieldType

            //Azt egészet vissza alakítja vizzé, hogy ne befolyásolja a type-ok kiosztását
            for (int x = 0; x < xMax; x++)
                for (int y = 0; y < yMax; y++)
                    GridElementList[x][y].HexField.UpdateHexType(HexField.hexType.Sea);
            yield return null; //KELL MERT KÜLÖNBEN BUGGOS
            int war = 0;
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    
                    var item = GridElementList[x][y];
                    if (item.Update == 1)
                    {
                        var t = GenerateType(GetHexAround(item.Cordinate, GridElementList));
                        if (t == HexField.hexType.Sea)
                            war++;
                        item.HexField.UpdateHexType(t);
                        item.Update = 2;
                        //yield return null;
                    }
                }
            }
            if(war != 0)
                Debug.LogWarning("HexTypeGeneration Warings: " + war);
            #endregion

            //Minimap csinál egy kép előszőr a kigenerát pályáról, majd egy másik renderTexture-re vált ami egy overlay az előző kép felett a UI-ban.
            #region MiniMap
            MiniMapCamera.enabled = true;
            yield return null;
            MiniMapCamera.enabled = false;

            MiniMapCamera.nearClipPlane = 201;
            MiniMapCamera.farClipPlane = 205;
            MiniMapCamera.targetTexture = MiniMapOverLay;
            MiniMapCamera.enabled = true;
            #endregion

            #region Triggers

            //Kigenerálja az össze bábú lehetséges lehelyézési pontját
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    GridElementList[x][y].HexField.CreateTriggers(transform.GetChild(1), PuppetTriggerObject);
                    if (SlowGeneratePuppetTriggers)
                        yield return null;
                }
            }
            #if _DEBUG
                Debug.Log("PuppetTriggers Generation done!"); yield return null;
            #endif
            yield return null; //Fontos mert ha nincs itt nem hivodik meg még a collision és összebuggolhat a SlowGeneratePathTriggers és amiatt is hogy még nem lettek a felesleges PuppetTriggerek eltüntetve
            var Triggers = GameObject.FindGameObjectsWithTag("PuppetTrigger");
            //bejárja a meglévő bábú lehelyezési pontokat
            foreach (GameObject tr1 in Triggers)
            {
                foreach (GameObject tr2 in Triggers)
                {
                    if (tr1 == tr2 || (tr1.transform.position - tr2.transform.position).sqrMagnitude > 35.6f) //azonos bábú helyeket átúgorja hogy önmagával ne legyen párba
                        continue;

                    //A két vizsgált bábú hely közé lehelyet egy út helyet
                    GameObject pathtrigger = Instantiate(PathTriggerObject, new Vector3((tr1.transform.position.x + tr2.transform.position.x) / 2, 1, (tr1.transform.position.z + tr2.transform.position.z) / 2), new Quaternion());
                    pathtrigger.transform.LookAt(tr1.transform.position);//elforgatja hogy jó írányba nézzen
                    pathtrigger.transform.position += new Vector3(0.0f, -0.1f, 0.0f);//egy kicsit lejebb helyezi hogy ne lebegjen
                    PathTrigger pt = pathtrigger.GetComponent<PathTrigger>();
                    pt.AddPuppet(new GameObject[] { tr1,tr2 }); //Hozzáadja a 2 bábú helyet ami által létre lett hozva
                    //hozzáadja azokat a mezőket amiből nyersanyakot kaphat
                    pt.AddField(tr1.GetComponent<PuppetTrigger>().ConnectedField); 
                    pt.AddField(tr2.GetComponent<PuppetTrigger>().ConnectedField);
                    //Áthelyezi a megfelelő gameobject alá
                    pathtrigger.transform.parent = transform.GetChild(2);

                    if (SlowGeneratePathTriggers)
                        yield return null;
                }
            }
            yield return null;
            var Paths = GameObject.FindGameObjectsWithTag("PathTrigger");
            //Bejárjuk az össze kigenerált út helyet
            foreach (var item in Paths)
            {
                var pt = item.GetComponent<PathTrigger>().ConnectedPuppet; //kikérjuk a az úthelyhez tartozó bábú helyket
                for (int i = 0; i < pt.Length; i++)//majd bejárjuk
                    pt[i].GetComponent<PuppetTrigger>().AddPath(new GameObject[] { item }); //és hozzá adjuk ezekhez a bábú helyekhez a a jelenlegi út helyeket
            }
            #if _DEBUG
                Debug.Log("PathTriggers Generation done!"); yield return null;
            #endif
            #endregion

            #region CameraBorder
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    if (MainCamera.GetComponent<SideMoveCamera>().MaxDown > GridElementList[x][y].GameObject.transform.position.z)
                        MainCamera.GetComponent<SideMoveCamera>().MaxDown = (int)GridElementList[x][y].GameObject.transform.position.z;

                    if (MainCamera.GetComponent<SideMoveCamera>().MaxUp < GridElementList[x][y].GameObject.transform.position.z)
                        MainCamera.GetComponent<SideMoveCamera>().MaxUp = (int)GridElementList[x][y].GameObject.transform.position.z;


                    if (MainCamera.GetComponent<SideMoveCamera>().MaxLeft > GridElementList[x][y].GameObject.transform.position.x)
                        MainCamera.GetComponent<SideMoveCamera>().MaxLeft = (int)GridElementList[x][y].GameObject.transform.position.x;

                    if (MainCamera.GetComponent<SideMoveCamera>().MaxRight < GridElementList[x][y].GameObject.transform.position.x)
                        MainCamera.GetComponent<SideMoveCamera>().MaxRight = (int)GridElementList[x][y].GameObject.transform.position.x;
                }
            }
            Debug.Log($"MaxDown: {MainCamera.GetComponent<SideMoveCamera>().MaxDown} | MaxUp: {MainCamera.GetComponent<SideMoveCamera>().MaxUp} | MaxLeft: {MainCamera.GetComponent<SideMoveCamera>().MaxLeft} | MaxRight: {MainCamera.GetComponent<SideMoveCamera>().MaxRight}");
            MainCamera.GetComponent<SideMoveCamera>().Offset();

            #if _DEBUG
                MainCamera.backgroundColor = Color.magenta;
                Debug.Log("Camera border done!"); yield return null;
            #endif
            #endregion

            #if _DEBUG
                Debug.LogWarning("Generation done!");
                int MapElementCount = 0;
                for (int i = 0; i < transform.childCount; i++)
                    MapElementCount += transform.GetChild(i).childCount;
                Debug.Log($"Generated total object count: {UnityEngine.Object.FindObjectsOfType<GameObject>().Count()} | MapElement: {MapElementCount}");
            #endif

            transform.GetComponent<MapVisibility>().enabled = true;
            MainCamera.GetComponent<SideMoveCamera>().enabled = true;
        }

        private static HexField.hexType GenerateType(List<Cor> HexAround)
        {
            HexField.hexType re;
            do
            {
                re = (HexField.hexType)(RND.Next((Enum.GetValues(typeof(HexField.hexType)).Length - 1)) + 1);
            } while (HexAround.Where(x => x.HexField.HexType == re).Count() > 0);
            return re;

            /*
            Dictionary<HexField.hexType, float> chnc = new Dictionary<HexField.hexType, float>();

            float OverAll = 0f;
            foreach (HexField.hexType item in Enum.GetValues(typeof(HexField.hexType)))
            {
                float Value = HexAround.Where(x => x.HexField.HexType == item).Count() > 0 || item == HexField.hexType.Sea ? 0.0000001f : 1f;
                chnc.Add(item, Value);
                OverAll += Value;
            }
            
            float next;
            do
            {
                next = (float)RND.NextDouble();
            } while (next == 0f || next == 1f);

            float rnd = OverAll * next;
            float cOld = 0f;
            float cNew = 0f;

            foreach (var item in chnc)
            {
                cOld = cNew;
                cNew += item.Value;
                if (cOld < rnd && rnd < cNew)
                    return item.Key;
            }

            Debug.LogError("SHIT");
            return HexField.hexType.Sea;
            */
        }

        private static float MarginMultiple(int x, int y, int xMax, int yMax, float MarginPresent, float MarginSensivity)
        {
            float p = 0.0f;
            int pX = (int)(xMax / 100.0f * MarginPresent);
            int pY = (int)(yMax / 100.0f * MarginPresent);

            if (x < pX)
                p += x / (pX / 100.0f);
            else if(x > xMax - pX)
                p += (x - (xMax - pX)) / (pX / 100.0f);

            if (y < pY)
                p += y / (pY / 100.0f);
            else if (y > yMax - pY)
                p += (y - (yMax - pY)) / (pY / 100.0f);

            return (p / MarginSensivity);
        }

        //Adott Hexagont átalakít Land-é a megadott Type alapján
        private static void MakePreLand(Cor HexObject, HexField.hexType type)
        {
            HexObject.Update = 1;
            HexObject.HexField.Land = true;
            HexObject.HexField.UpdateHexType(type);
        }

        //Random pontot választ egy 2D táblán a Margin szabályai szerint
        private static Vector2Int GetRandomHexOnGrid(int xMax, int yMax, float MarginPresent)
        {
            int pX = (int)(xMax / 100.0f * MarginPresent);
            int pY = (int)(yMax / 100.0f * MarginPresent);
            return new Vector2Int(UnityEngine.Random.Range(pX, xMax - pX), UnityEngine.Random.Range(pY, yMax - pY));
        }

        //A teljes Hexagon listából visszadja az elemet egy 2D int vector alapján
        private static Cor GetHexBy2dV(Vector2Int vector2Int, List<List<Cor>> corlist)
        {
            return corlist[vector2Int.x][vector2Int.y];
        }

        //Visszaad egy Hexagon listát mely egy adott Hexagon küröl helyezkednek el
        private static List<Cor> GetHexAround(Vector2Int vector2Int, List<List<Cor>> corlist, bool fillnonexistwithnull = false)
        {
            List<Cor> temp = new List<Cor>();
            if (Try(() => temp.Add(corlist[vector2Int.x - 1][vector2Int.y])) == null && fillnonexistwithnull) temp.Add(null);
            if (Try(() => temp.Add(corlist[vector2Int.x - 1][vector2Int.y + 1])) == null && fillnonexistwithnull) temp.Add(null);

            if (Try(() => temp.Add(corlist[vector2Int.x + 1][vector2Int.y])) == null && fillnonexistwithnull) temp.Add(null);
            if (Try(() => temp.Add(corlist[vector2Int.x + 1][vector2Int.y + 1])) == null && fillnonexistwithnull) temp.Add(null);

            if (Try(() => temp.Add(corlist[vector2Int.x][vector2Int.y - 1])) == null && fillnonexistwithnull) temp.Add(null);
            if (Try(() => temp.Add(corlist[vector2Int.x][vector2Int.y + 1])) == null && fillnonexistwithnull) temp.Add(null);
            temp.Shuffle();
            return temp;
        }

        //könnyebben meghívható trycatch
        #region EasyTry
        private delegate void VoidDelegate();
        private static System.Exception Try(VoidDelegate v)
        {
            try { v(); }
            catch (System.Exception ex) { return ex; }
            return null;
        }
        #endregion
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateHexagon.GenerateGrid(
            transform,
            GridX, 
            GridY, 
            Hexagon,
            PathTrigger,
            PuppetTrigger,
            DoIlandOnlyOnSea,
            IlandCount, 
            MarginPresent,
            MarginSensivity,
            IlandAvgSize, 
            GrowMultiple, 
            RemoveLakes, 
            LakeSensivity, 
            LakesRecheck,
            MainCamera,
            MiniMapCamera,
            MiniMapOverLay,
            SG_Iland,
            SG_IlandHexagon,
            SG_IlandHexagonLayer,
            SG_LakeRemove,
            SG_LakeRemoveLayer,
            SG_PuppetTriggers,
            SG_PathTriggers
            ));
    }

    // Update is called once per frame
    void Update()
    {
        //var q = Objects[0, 0].transform.rotation;
        //Debug.Log($"Object[0,0] rotation: {q.x}, {q.y}, {q.z}, {q.w}");
    }

}
