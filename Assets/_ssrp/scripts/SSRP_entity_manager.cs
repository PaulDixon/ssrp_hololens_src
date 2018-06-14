using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSRP_entity_manager : MonoBehaviour
{

    PersistantManager boss;
    // Entity manager
    // Rendering gui
    //public GameObject contextEntityViewGameObject; // we will be instatiating metaData Prefabs
    private RectTransform entity_canvas;           // Link to the attribute canvas
    private Vector2 entity_canvas_init_dimension;// Initial Attribute Rectangle position,height,width x,y,center, top,bottom,left,right
    private List<GameObject> prefabChildList = new List<GameObject>();

    public List<int> lodDistances = new List<int>() { 0, 15, 25, 50, 100, 400 };
    public List<List<SSRP_contextResponse>> distanceSorted;
    private bool canLocateEntityPrefab = false;
    public Vector3 gpsPos = new Vector3(64.751465f, 20.954414f);


    // Use this for initialization
    void Start()
    {
        boss = PersistantManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void generateEmptySortingList()
    {
        distanceSorted = null;
        distanceSorted = new List<List<SSRP_contextResponse>>();
        foreach (int bound in lodDistances)
        {
            distanceSorted.Add(new List<SSRP_contextResponse>());
        }
    }


    public void importEntity(SSRP_contextResponse[] _loraSensors_unordered_List = null)
    {
        boss = PersistantManager.Instance;
        generateEmptySortingList();

        int m = lodDistances.Count - 1;
        int i = 0;

        foreach (SSRP_contextResponse response in _loraSensors_unordered_List)
        {

            // Find Filter out LAT, LON, Marker_name
            string sLat = response.getAttributeValue("LAT");
            string sLon = response.getAttributeValue("LON");

            response.marker_name = response.getAttributeValue("marker_name");     // used to reference the QR / Vuforia Marker identity.
            float lat = float.Parse(sLat);

            float lon = float.Parse(sLon);
            response.gpsPos = new Vector3(lat, lon);
            
            
            response.distToViewer = (double)Vector3.Distance(gpsPos, response.gpsPos);

            int lod = 0;
            int maxValue;
            int minValue;
            for (i = 0; i < m; i++)
            {
                maxValue = lodDistances[i + 1];
                minValue = lodDistances[0];

                if (response.distToViewer < maxValue && response.distToViewer > minValue)
                {
                    lod = i;
                    // Debug.LogFormat("TRUE - viewer gpsPos: [{0},{1}]Lat:{2}, lon:{3}, distToViewer:{4} fits in lod_{5}:{6}", gpsPos.x, gpsPos.y, response.gpsPos.x, response.gpsPos.y, response.distToViewer, i, lodDistances[i]);
                    Debug.LogFormat("TRUE -  distToViewer:{0} fits in lod_{1}:{2}", response.distToViewer, i, lodDistances[i]);
                    boss.hud.addText("TRUE -  distToViewer:" + response.distToViewer + " fits in lod_" + i + ":" + lodDistances[i]);
                    break;
                    //distanceSorted[i].Add(response);
                }


            }

            distanceSorted[lod].Add(response);
            
        }





        List<SSRP_contextResponse> localMarkers = distanceSorted[0];
        // push nearest Lod to the marker generator
        

        // update hud SensorCount
        string sensorBreakDown_str = "";
        int distanceBounds = lodDistances.Count;
        distanceBounds--;
        int counter = distanceBounds;
        for (counter = distanceBounds - 1; -1 < counter; counter--)
        {

            sensorBreakDown_str += "[" + lodDistances[counter] + "m] " + distanceSorted[counter].Count + " (s)\n";
        }

        boss.hud.sensorBreakDown(sensorBreakDown_str);
        boss.targetManager.import(localMarkers);

    }

    /*


    private void emptyEntity()
    {
        if (contextEntityViewGameObject)
        {

            foreach (GameObject child in prefabChildList)
            {
                //joint.useSpring = false;
                Destroy(child);
            }
            prefabChildList = new List<GameObject>();
        }

    }
    
    private void AddEntity()
    {
        //data.attributes = new SSRP_attribute[] { };
        Vector2 newCanvasDimensions = new Vector2(0f, 0f);
        int m = distanceSorted[0].Count;
        int i = 0;
    }

    public void renderEntities()
    {

        if (contextEntityViewGameObject)
        {
            Vector3 pos = new Vector3(0f, 0f, 0f);

            // loop through list of Child data and generate their UI
            foreach (SSRP_contextResponse entityReponse in distanceSorted[0])
            {


                GameObject data_MVC;

                data_MVC = (GameObject)Instantiate(contextEntityViewGameObject, pos, Quaternion.identity);
                //data_MVC.transform.SetParent(contextEntityViewGameObject.transform, false);
                prefabChildList.Add(data_MVC);
                SSRP_context_element_controller child_controller = data_MVC.GetComponent<SSRP_context_element_controller>();

                child_controller.importData(entityReponse);



            }





        }
    }

    
    
    public void open(string needle, bool forceSpawn = false)
    {
        // We should only instatiate and render the Sensor panel if near-by
        // UNLESS we are have testing purposes
        if (forceSpawn)
        {

            foreach (List<SSRP_contextResponse> distanceBracket in distanceSorted)
            {
                foreach (SSRP_contextResponse entity in distanceBracket)
                {
                    if (entity.marker_name == needle)
                    {
                        // render at testLocation
                    }
                }
            }
        }
        else
        {
            foreach (SSRP_contextResponse entity in distanceSorted[0])
            {
                if (entity.marker_name == needle)
                {
                    // render at testMarker
                }
            }
        }



    }
  
  //  */


}
