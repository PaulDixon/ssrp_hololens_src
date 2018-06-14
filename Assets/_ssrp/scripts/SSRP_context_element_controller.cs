
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SSRP_context_element_controller : MonoBehaviour {
    PersistantManager boss;
    public Text ui_id;
    public Text ui_type;
    public GameObject ui_isPattern;
    public Text ui_status;
    private SSRP_StatusCode statusCode = new SSRP_StatusCode();
    private bool canRenderText = false;
    private bool canRenderScroll = false;
    public bool isClosed = true;
    public bool isPanel = false;
    public bool allowedToRender = false;
    public int maxColSize = 6;

    private GameObject contextEntityViewGameObject; // we will be instatiating metaData Prefabs
    private RectTransform entity_canvas;           // Link to the attribute canvas
    private Vector2 entity_canvas_init_dimension;// Initial Attribute Rectangle position,height,width x,y,center, top,bottom,left,right
    private bool canLocateBaseGO = false;
    public GameObject ViewPortContentTarget;      // this has to be defined here because Unity & my brain


    // MetaData which will be spawned into Attribute
    public GameObject attributGameObjectPrefab; // Attribute Canvas Prefabs 
    private List<GameObject> prefabChildList = new List<GameObject>();
    private RectTransform childDataPrefab_canvas;
    private Vector2 childPrefab_canvas_init_dimension;
    private bool canRenderChildData = false;
    private bool childUpdateRequired = false;
    GameObject scrollview;// = contextEntityViewGameObject.transform.Find("scrollView").gameObject;
    RectTransform scrollview_canvas;//= scrollview.transform.GetComponent<RectTransform>();
    private SSRP_ContextElement data = new SSRP_ContextElement();
    private SSRP_ContextElement data_prev = new SSRP_ContextElement();

    // Use this for initialization
    void Start()
    {
        boss = PersistantManager.Instance;
        canRenderScroll = false;
        isClosed = true;
        testUIRender();
        testParentLink();
        testPrefabLink();
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
            boss.hud.addText("Context Canvas won't initialize?");
        }
    }

    // Update is called once per frame
    void Update() {

        if (canRenderScroll && scrollview != null)
        { scrollview.SetActive(!isClosed); }



    }
    public void forceDebugString()
        {
            
        }
    

    public void importData(SSRP_ContextElement element)
    {
        childUpdateRequired = false;
        if (!ArrayList.Equals(element.attributes, data.attributes))
        {
            childUpdateRequired = true;
        }
        statusCode = new SSRP_StatusCode();
        data = element;

        refresh();
    }

    public void importData(SSRP_contextResponse response)
    {
        boss = PersistantManager.Instance;
        boss.hud.addText("Adding data to Context panel");
        childUpdateRequired = true;
        statusCode = response.statusCode;
        data = response.contextElement;
        testUIRender();
        testParentLink();
        testPrefabLink();

        refresh();
    }

    void refresh()
    {

     //   Debug.LogFormat("basicUI Elements Available{0}, root canvas set :{1}, child prefab linked: {2}, childUpdateRequired:{3}, contextElement:{4}", canRenderText, canLocateBaseGO, canRenderChildData,childUpdateRequired, data.description());
        if (data != null)
        {
            if (canRenderText)
            {
                ui_id.text = data.id;
                ui_status.enabled = false;
                Toggle toggle = ui_isPattern.GetComponent<Toggle>();
                toggle.isOn = false;
                if (data.isPattern ==  "true" || data.isPattern == "True" || data.isPattern == "TRUE" || data.isPattern == "1")
                {
                    toggle.isOn = true;
                }
                ui_type.text = data.type;
                if (statusCode == null || statusCode.code == "")
                {
                   
                }
                else
                {
                    ui_status.text = "statusCode :" + statusCode.code + ": " + statusCode.reasonPhrase;
                    ui_status.enabled = true ;
                }
                    
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

    private void addChildren()
    {
        //data.attributes = new SSRP_attribute[] { };
        Vector2 newCanvasDimensions = new Vector2(0f, 0f);
        int m = data.attributes.Length;
        int i = 0;
        

        if (canRenderChildData)
        {
            if (m == 0)
            {
                //newCanvasDimensions = childPrefab_canvas_init_dimension;
            }
            else
            {
                
                Vector3 newCanvasPosition = new Vector3(0f, 0f, 0f);
                Vector2 origin = new Vector2(0f, 0f);    // new Vector2(0f, -126f);
                Vector2 offset = new Vector2(10, -10f);
                Vector2 dim = new Vector2(210f, 126f);
                Vector2 step = new Vector2(0f, 0f);
                Vector2 stepOffset = new Vector2(0, 0);
                Vector2 pos = new Vector2(0f, 0f);
                int rowNo = 0;
                int colCount = 0;

                // rough GRID LAY OUT FOR ATTRIBUTESs - this should be have one level of recursion and to resize 6 boxes, into 2 rows of 3, and not 1 row of 5 and one row of 1. 
                
                
                float scrollbarSize = 25;
                if (maxColSize > 0)
                {
                    float rows = 0;
                    float cols = maxColSize;
                    if (m <= maxColSize)
                    {
                        cols = m;
                        rows = 1;
                    }else
                    {
                        rows = Mathf.Floor(m / maxColSize) + 1;
                    }
                    
                    float height = Mathf.Ceil((dim.y + Mathf.Abs(offset.y)) * rows) + scrollbarSize;
                    float width = offset.x + (dim.x * cols) + scrollbarSize;

                    try
                    {
                        scrollview = transform.Find("popup").gameObject;
                        scrollview_canvas = scrollview.transform.GetComponent<RectTransform>();

                        scrollview_canvas.sizeDelta = new Vector2(width, height);
                    }
                    catch
                    {
                        Debug.LogFormat("couldn't link to scrollview Canvas");
                    }
                }


               
                
                
               
                
                // loop through list of Child data and generate their UI
                for (i = 0; i < m; i++)
                {
                    
                    if (maxColSize > 0 && colCount == maxColSize)
                    {
                        colCount = 0;
                        rowNo++;
                        
                    }
                    step.Set(colCount, -rowNo);
                    
                    stepOffset = Vector2.Scale(step,dim) ;
                    
                    SSRP_attribute childData = data.attributes[i];
                    pos = origin + offset + stepOffset;
                    GameObject data_MVC;

                    Debug.Log(scrollview);
                    if (scrollview == null || scrollview.transform == null)
                    {
                        return;
                    }
                    
                   
                    data_MVC = (GameObject)Instantiate(attributGameObjectPrefab, pos, Quaternion.identity);
                    data_MVC.transform.SetParent(scrollview.transform, false);
                    prefabChildList.Add(data_MVC);
                    SSRP_Attribute_Controller child_controller = data_MVC.GetComponent<SSRP_Attribute_Controller>();
               //     Debug.LogFormat("SSRP_attribute childData = [{0}]", childData.description());
                    child_controller.importData(childData);
                    colCount++;
                }


                //resize base canvas
                //newCanvasDimensions = new Vector2(200f, (m + 1) * childPrefab_canvas_init_dimension.y);
                //entity_canvas.sizeDelta = newCanvasDimensions;
            }
            
             
        }

        
    }

    /*
    public Vector2 GetCanvasDimension()
    {
        //Vector2 dim = new Vector2();
        return entity_canvas.sizeDelta;
    }
    // */

    private void removeChildren()
    {
        if (canLocateBaseGO)
        {
            //entity_canvas.sizeDelta = entity_canvas_init_dimension;


            foreach (GameObject child in prefabChildList)
            {
                //joint.useSpring = false;
                Destroy(child);
            }
            prefabChildList = new List<GameObject>();
        }

    }

    private void testUIRender()
    {
        canRenderText = false;
        canRenderScroll = false;
        try
        {
            
            if (ui_id != null && ui_type != null && ui_status != null && ui_isPattern != null)
            {
                canRenderText = true;
                //  Debug.LogFormat("canRenderText:{0}", canRenderText);

            }
            else { Debug.LogWarningFormat("UI_404 can not RenderText:{0}", canRenderText); }
        }
        catch { Debug.LogWarningFormat("UI_404 can not RenderText:{0}", canRenderText); }

        try
        {
            // scrollview = contextEntityViewGameObject.transform.Find("scrollView").gameObject;
            if (scrollview != null)
            { 
             scrollview_canvas = scrollview.transform.GetComponent<RectTransform>();
            }
            canRenderScroll = true;
        }
        catch { Debug.LogWarningFormat("UI_404 can not Render scroll scrollview:{0}, scrollview_canvas:{1}", scrollview, scrollview_canvas); }

    }

    private void testPrefabLink()
    {
        canRenderChildData = false;
        try
        {

            if (attributGameObjectPrefab != null)
            {

                childDataPrefab_canvas = attributGameObjectPrefab.transform.GetComponent<RectTransform>();
                childPrefab_canvas_init_dimension = childDataPrefab_canvas.sizeDelta;
                canRenderChildData = true;
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
        canLocateBaseGO = false;
        try
        {
            canLocateBaseGO = true;
            entity_canvas = transform.GetComponent<RectTransform>();
            
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
        SSRP_attribute test_att01 = new SSRP_attribute("att_1", "att_type", "att value", metaData_list);
        SSRP_attribute test_att02 = new SSRP_attribute("att_2", "float", "344.324432543", metaData_list);
        SSRP_attribute test_att03 = new SSRP_attribute("att_3", "bool", "false", metaData_list);
        SSRP_attribute test_att04 = new SSRP_attribute("att_4", "int", "3", metaData_list);
        SSRP_attribute test_att05 = new SSRP_attribute("att_5", "att_type", "att value", metaData_list);
        SSRP_attribute[] attList = new SSRP_attribute[] { test_att01, test_att02, test_att03, test_att04, test_att05 };
        SSRP_ContextElement testdata= new SSRP_ContextElement("34234324243","sun","true", attList);
         importData(testdata);
    }

    private void addChildren_old_scrollview()
    {
        //data.attributes = new SSRP_attribute[] { };
        Vector2 newCanvasDimensions = new Vector2(0f, 0f);
        int m = data.attributes.Length;
        int i = 0;

        /*
        // debug purposes 
        string childDescription = "";
        foreach (SSRP_attribute childObj in data.attributes)
        {
            childDescription += childObj.description() + ",\n";
        }
        Debug.LogFormat("Element Controller :  canLocateBaseGO:{1} and canRenderMeta:{2}, add {0} Children, attributes = [{3}]", m, canLocateBaseGO, canRenderChildData, childDescription);
        // */

        if (canLocateBaseGO && canRenderChildData)
        {
            if (m == 0)
            {
                //newCanvasDimensions = childPrefab_canvas_init_dimension;
            }
            else
            {

                Vector3 newCanvasPosition = new Vector3(0f, 0f, 0f);
                Vector2 origin = new Vector2(0f, -126f);
                Vector2 offset = new Vector2(10, -10f);
                Vector2 dim = new Vector2(210f, 126f);
                Vector2 step = new Vector2(0f, 0f);
                Vector2 stepOffset = new Vector2(0, 0);
                Vector2 pos = new Vector2(0f, 0f);
                int rowNo = 0;
                int colCount = 0;

                // rough GRID LAY OUT FOR ATTRIBUTESs - this should be have one level of recursion and to resize 6 boxes, into 2 rows of 3, and not 1 row of 5 and one row of 1. 

                // scrollview.transform
                float scrollbarSize = 25;
                if (maxColSize > 0)
                {
                    float rows = 0;
                    float cols = maxColSize;
                    if (m <= maxColSize)
                    {
                        cols = m;
                        rows = 1;
                    }
                    else
                    {
                        rows = Mathf.Floor(m / maxColSize) + 1;
                    }

                    float height = Mathf.Ceil((dim.y + Mathf.Abs(offset.y)) * rows) + scrollbarSize;
                    float width = offset.x + (dim.x * cols) + scrollbarSize;
                    //       Debug.LogFormat("We have {0} attribute and a maxColSize of {1}, giving us a height:{2} and width:{3}", m, maxColSize, height, width);
                    try
                    {
                        scrollview = contextEntityViewGameObject.transform.Find("scrollView").gameObject;
                        scrollview_canvas = scrollview.transform.GetComponent<RectTransform>();

                        scrollview_canvas.sizeDelta = new Vector2(width, height);
                    }
                    catch
                    {
                        Debug.LogFormat("couldn't link to scrollview Canvas");
                    }
                }






                // loop through list of Child data and generate their UI
                for (i = 0; i < m; i++)
                {

                    if (maxColSize > 0 && colCount == maxColSize)
                    {
                        colCount = 0;
                        rowNo++;

                    }
                    step.Set(colCount, -rowNo);

                    stepOffset = Vector2.Scale(step, dim);

                    SSRP_attribute childData = data.attributes[i];
                    pos = origin + offset + stepOffset;
                    GameObject data_MVC;

                    data_MVC = (GameObject)Instantiate(attributGameObjectPrefab, pos, Quaternion.identity);
                    data_MVC.transform.SetParent(ViewPortContentTarget.transform, false);
                    prefabChildList.Add(data_MVC);
                    SSRP_Attribute_Controller child_controller = data_MVC.GetComponent<SSRP_Attribute_Controller>();
                    //     Debug.LogFormat("SSRP_attribute childData = [{0}]", childData.description());
                    child_controller.importData(childData);
                    colCount++;
                }


                //resize base canvas
                //newCanvasDimensions = new Vector2(200f, (m + 1) * childPrefab_canvas_init_dimension.y);
                //entity_canvas.sizeDelta = newCanvasDimensions;
            }


        }


    }
}
