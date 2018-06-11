using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SSRP_Attribute_Controller : MonoBehaviour {
    PersistantManager boss;
    // Basic Attribute Canvas and details
    public Text ui_name;
    public Text ui_type;
    public Text ui_value;
    private bool canRenderText = false;
    public bool isClosed = false;

    public GameObject attributeViewGameObject; // we will be instatiating metaData Prefabs
    //private Canvas attributeRootCanvas;
    private RectTransform attribute_canvas;           // Link to the attribute canvas
    private Vector2 attribute_canvas_init_dimension;// Initial Attribute Rectangle position,height,width x,y,center, top,bottom,left,right
    private bool canLocateBaseGO = false;
    private Vector2 CanvasDimension;

    // MetaData which will be spawned into Attribute
    public GameObject metaDataGameObjectPrefab; // MetaData Canvas Prefabs 
    private List<GameObject> prefabChildList = new List<GameObject>();
    private RectTransform childDataPrefab_canvas;
    private Vector2 childPrefab_canvas_init_dimension;
    private bool canRenderChildData = false;
    private bool childUpdateRequired = false;
    

    private SSRP_attribute data = new SSRP_attribute();
    private SSRP_attribute data_prev = new SSRP_attribute();


    

    // Use this for initialization
    void Start() {
        boss = PersistantManager.Instance;
        //testDataGeneration();
        try
        {
            Canvas mycanvas = GetComponent<Canvas>();
            mycanvas.enabled = true;
            BoxCollider mycolider = GetComponent<BoxCollider>();
            mycolider.enabled = true;
            // testDataGeneration();
        }
        catch
        {
            boss.hud.addText("attribute Canvas won't initialize?");
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

   

    public void importData(SSRP_attribute attr)
    {
        childUpdateRequired = false;
        if (!ArrayList.Equals(attr.metadatas, data.metadatas))
        {
            childUpdateRequired = true;
        }
        data = attr;
       
        refresh();
    }

    void refresh()
    {
        testUIRender();
        testParentLink();
        testPrefabLink();

        if (data != null)
        {
            //Debug.Log(data.description());
            if (canRenderText)
            {
                ui_name.text = data.name;
                ui_value.text = "VALUE:" + data.value;
                ui_type.text = data.type;
            }

            if (childUpdateRequired)
            { 
                childManagement(); 
            }

        }
        else
        {
            removeChildren();
        }
        
    }

    private void childManagement()
    {
        removeChildren();
        addChildren();
    }

    private void removeChildren()
    {
        if(canLocateBaseGO)
        {
            //attribute_canvas.sizeDelta = attribute_canvas_init_dimension;
           

            foreach (GameObject child in prefabChildList)
            {
                //joint.useSpring = false;
                Destroy(child);
            }
            prefabChildList = new List<GameObject>();
        }
        
    }

    private void addChildren()
    {
       // data.metadatas = new SSRP_Metadata[] { };
        Vector2 newCanvasDimensions = new Vector2(0f, 0f);
        int m = data.metadatas.Length;
        int i = 0;
        
        /*
        // for Debug purposes
        string childDescription = "";
        foreach(SSRP_Metadata childObj in data.metadatas)
        {
            childDescription += childObj.description() + ", ";
        }
        Debug.LogFormat("AttributeController:  canLocateBaseGO:{1} and canRenderMeta:{2}, add {0} Children, metaData = [{3}]", m, canLocateBaseGO, canRenderChildData, childDescription);
        // */
        
       

        if (canLocateBaseGO && canRenderChildData )
        {
            if (m == 0 || isClosed) 
            {
                //newCanvasDimensions = childPrefab_canvas_init_dimension;  
            }
            else
            { 
                Vector3 newCanvasPosition = new Vector3(0f, 0f, 0f);
                Vector2 origin = new Vector2(0f, 0f);
                Vector2 offset = new Vector2(10f, 0f);
                Vector2 step = new Vector2(0f, -50f);
                Vector2 pos = new Vector2(0f, 0f);

                // set start offset position for child-spawn based on human judgement and some actual dimensions
                offset = new Vector2(10f, -65f);


                // loop through list of Child data and generate their UI
                for (i = 0; i < m; i++)
                {

                    SSRP_Metadata metaData = data.metadatas[i];
                    pos = origin + offset + (step * i);

                    GameObject metaData_MVC;

                    metaData_MVC = (GameObject)Instantiate(metaDataGameObjectPrefab, pos, Quaternion.identity);
                    metaData_MVC.transform.SetParent(attributeViewGameObject.transform, false);
                    prefabChildList.Add(metaData_MVC);
                    SSRP_metaData_controller metaData_controller = metaData_MVC.GetComponent<SSRP_metaData_controller>();
                    metaData_controller.importData(metaData);
                }
                //set background dimensions
               // newCanvasDimensions = new Vector2(200f, (m + 1) * childPrefab_canvas_init_dimension.y);
               
            }
            //attribute_canvas.sizeDelta = newCanvasDimensions;

        }

    }
    /*
    public Vector2 GetCanvasDimension() 
    {
        //Vector2 dim = new Vector2();
        return attribute_canvas.sizeDelta;
    }
    // */

    public bool add_meta(SSRP_Metadata meta)
    {
        return false;
    }

    public bool remove_meta(SSRP_Metadata meta)
    {
        return false;
    }

    public bool remove_meta(string _name)
    {
        return false;
    }

    public bool remove_meta(int _pos)
    {
        return false;
    }

    public int hasMeta(SSRP_Metadata meta)
    {
        return -1;
    }

    public int hasMeta(string _name)
    {
        return -1;
    }

    public SSRP_Metadata fetchMeta(int _pos)
    {
        SSRP_Metadata retObj = new SSRP_Metadata();
        return retObj;
    }

    public SSRP_Metadata fetchMeta(string _name)
    {
        SSRP_Metadata retObj = new SSRP_Metadata();
        return retObj;
    }

    private void testUIRender()
    {
        try
        {
            canRenderText = false;
            if (ui_name != null && ui_type != null && ui_value != null)
            {
                canRenderText = true;
              //  Debug.LogFormat("canRenderText:{0}", canRenderText);

            }
            else { Debug.LogWarningFormat("UI_404 canRenderText:{0}", canRenderText); }
        }
        catch { Debug.LogWarningFormat("UI_404 canRenderText:{0}", canRenderText); }
    }

    private void testPrefabLink()
    {

        try
        {
            
            if (metaDataGameObjectPrefab  != null)
            {
                canRenderChildData = true;
                childDataPrefab_canvas = metaDataGameObjectPrefab.transform.GetComponent<RectTransform>();
                //childPrefab_canvas_init_dimension = childDataPrefab_canvas.sizeDelta;
            }
            else { Debug.LogWarningFormat("metaDataGameObjectPrefab is missing canRenderMeta:{0}", canRenderChildData); }
        }
        catch
        {
            Debug.LogWarningFormat("metaDataGameObjectPrefab is missing canRenderMeta:{0}", canRenderChildData);
        }
        
    }
    private void testParentLink()
    {
        try
        {
          
            if (attributeViewGameObject != null)
            {
                canLocateBaseGO = true;
                attribute_canvas = attributeViewGameObject.transform.GetComponent<RectTransform>();
               // attribute_canvas_init_dimension = attribute_canvas.sizeDelta;


            }
            else { Debug.LogWarningFormat("attributeViewGameObject is missing canLocateBaseGO:{0}", canLocateBaseGO); }
        }
        catch { Debug.LogWarningFormat("attributeViewGameObject is missing canLocateBaseGO:{0}", canLocateBaseGO); }
        
    }

    private void testDataGeneration()
    {
        SSRP_Metadata metaData_00 = new SSRP_Metadata("Faux_name", "faux_type", "faux value");
        SSRP_Metadata metaData_01 = new SSRP_Metadata("meta1", "int", "one");
        SSRP_Metadata metaData_02 = new SSRP_Metadata("meta2", "int", "two");
        SSRP_Metadata metaData_03 = new SSRP_Metadata("meta3", "int", "three");
        SSRP_Metadata metaData_04 = new SSRP_Metadata("meta4", "int", "four");
        SSRP_Metadata[] metaData_list = new SSRP_Metadata[] { metaData_00, metaData_01, metaData_02, metaData_03, metaData_04 };
        SSRP_attribute testdata = new SSRP_attribute("Faux_name", "faux_type", "faux value", metaData_list);
        importData(testdata);
    }
}
